using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Mechanism.AvaloniaUI.Sample
{
    public class SampleConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            return "CornerCurves: " + values[0].ToString() + "\nBorderPresence: " + values[1].ToString();
        }
    }
}
