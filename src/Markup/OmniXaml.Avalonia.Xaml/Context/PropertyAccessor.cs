// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

namespace OmniXaml.Avalonia.Context
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Data;
    using global::Avalonia;
    using global::Avalonia.Controls;
    using global::Avalonia.Data;
    using global::Avalonia.Styling;

    internal static class PropertyAccessor
    {
        public static void SetValue(
            object instance,
            Property property,
            object value,
            ValueContext context)
        {
            var avaloniaProperty = FindAvaloniaProperty(instance, property);

            if (value is IBinding)
            {
                SetBinding(instance, property, avaloniaProperty, context, (IBinding)value);
            }
            else if (avaloniaProperty != null)
            {
                ((AvaloniaObject)instance).SetValue(avaloniaProperty, value);
            }
            else if (instance is Setter && property.PropertyName == "Value")
            {
                // TODO: Make this more generic somehow.
                var setter = (Setter)instance;
                var targetType = setter.Property.PropertyType;

                var assigment = new Assignment(setter, Property.RegularProperty<Setter>(s => setter.Value), value);
                var valueContext = new ValueContext(assigment, context.ObjectBuilderContext, context.TypeDirectory, context.BuildContext);
                var converted = context.ObjectBuilderContext.SourceValueConverter.GetCompatibleValue(valueContext, targetType);

                var finalAssigment = assigment.ReplaceValue(converted);
                finalAssigment.ExecuteAssignment();
            }
            else
            {
                context.Assignment.ExecuteAssignment();
            }
        }

        private static AvaloniaProperty FindAvaloniaProperty(object instance, Property member)
        {
            var registry = AvaloniaPropertyRegistry.Instance;
            var attached = member as AttachedProperty;
            var target = instance as AvaloniaObject;

            if (target == null)
            {
                return null;
            }

            if (attached == null)
            {
                return registry.FindRegistered(target, member.PropertyName);
            }
            else
            {
                var ownerType = attached.Owner;

                RuntimeHelpers.RunClassConstructor(ownerType.TypeHandle);

                return registry
                    .GetRegistered(target)
                    .FirstOrDefault(x => x.OwnerType == ownerType && x.Name == attached.PropertyName);
            }
        }

        private static void SetBinding(
            object instance,
            Property member,
            AvaloniaProperty property,
            ValueContext context,
            IBinding binding)
        {
            if (!(AssignBinding(instance, member, binding) ||
                  ApplyBinding(instance, property, context, binding)))
            {
                throw new InvalidOperationException(
                    $"Cannot assign to '{member.PropertyName}' on '{instance.GetType()}");
            }
        }

        private static bool AssignBinding(object instance, Property member, IBinding binding)
        {
            var property = instance.GetType()
                .GetRuntimeProperties()
                .FirstOrDefault(x => x.Name == member.PropertyName);

            if (property?.GetCustomAttribute<AssignBindingAttribute>() != null)
            {
                property.SetValue(instance, binding);
                return true;
            }

            return false;
        }

        private static bool ApplyBinding(
            object instance,
            AvaloniaProperty property,
            ValueContext context,
            IBinding binding)
        {
            if (property == null)
            {
                return false;
            }

            var control = instance as IControl;

            if (control != null)
            {
                if (property != Control.DataContextProperty)
                {
                    DelayedBinding.Add(control, property, binding);
                }
                else
                {
                    control.Bind(property, binding);
                }
            }
            else
            {
                // The target is not a control, so we need to find an anchor that will let us look
                // up named controls and style resources. First look for the closest IControl in
                // the TopDownValueContext.
                object anchor = context.BuildContext.AmbientRegistrator.Instances
                    .OfType<IControl>()
                    .LastOrDefault();

                // If a control was not found, then try to find the highest-level style as the XAML
                // file could be a XAML file containing only styles.
                if (anchor == null)
                {
                    anchor = context.BuildContext.AmbientRegistrator.Instances
                        .OfType<IStyle>()
                        .FirstOrDefault();
                }

                ((IAvaloniaObject)instance).Bind(property, binding, anchor);
            }

            return true;
        }
    }
}