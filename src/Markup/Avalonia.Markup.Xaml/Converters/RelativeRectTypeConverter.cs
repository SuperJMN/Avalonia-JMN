// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Globalization;

namespace Avalonia.Markup.Xaml.Converters
{
    using OmniXamlV1.TypeConversion;

    public class RelativeRectTypeConverter : ITypeConverter
    {
        public bool CanConvertFrom(IValueContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public bool CanConvertTo(IValueContext context, Type destinationType)
        {
            return false;
        }

        public object ConvertFrom(IValueContext context, CultureInfo culture, object value)
        {
            return RelativeRect.Parse((string)value, culture);
        }

        public object ConvertTo(IValueContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
}