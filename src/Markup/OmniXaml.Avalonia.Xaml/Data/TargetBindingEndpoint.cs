// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

namespace OmniXaml.Avalonia.Data
{
    using global::Avalonia;

    public class TargetBindingEndpoint
    {
        public AvaloniaObject Object { get; }

        public AvaloniaProperty Property { get; }

        public TargetBindingEndpoint(AvaloniaObject obj, AvaloniaProperty property)
        {
            Object = obj;
            Property = property;
        }
    }
}