using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Mechanism.AvaloniaUI.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool retVal = true;
            if (parameter is bool valBool)
                retVal = valBool;
            else if ((parameter is string valStr) && bool.TryParse(valStr, out bool valBool2))
                retVal = valBool2;

            return (value == null) == retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
