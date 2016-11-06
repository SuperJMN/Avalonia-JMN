// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

namespace OmniXaml.Avalonia.Converters
{
    using System;
    using System.Globalization;
    using global::Avalonia.Controls;

    public class ClassesTypeConverter : ITypeConverter
    {
        public bool CanConvertFrom(ValueContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public bool CanConvertTo(ValueContext context, Type destinationType)
        {
            return false;
        }

        public object ConvertFrom(ValueContext context, CultureInfo culture, object value)
        {
            return new Classes(((string)value).Split(' '));
        }

        public object ConvertTo(ValueContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
}