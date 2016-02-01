﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using Perspex;
using Perspex.Markup;

namespace Core2D.Perspex.Converters
{
    /// <summary>
    /// Converts a binding value object from <see cref="byte"/> to <see cref="string"/> and back from <see cref="string"/> to <see cref="byte"/>.
    /// </summary>
    public sealed class ByteToStringConverter : IValueConverter
    {
        /// <summary>
        /// Gets an instance of a <see cref="ByteToStringConverter"/>.
        /// </summary>
        public static readonly ByteToStringConverter Instance = new ByteToStringConverter();

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == PerspexProperty.UnsetValue || value.GetType() != typeof(byte))
            {
                return PerspexProperty.UnsetValue;
            }

            return value.ToString();
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == PerspexProperty.UnsetValue || value.GetType() != typeof(string))
            {
                return PerspexProperty.UnsetValue;
            }

            byte result;
            if (byte.TryParse((string)value, out result))
            {
                return result;
            }

            return PerspexProperty.UnsetValue;
        }
    }
}
