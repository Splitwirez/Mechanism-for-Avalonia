using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Mechanism.AvaloniaUI.Sample
{
    public class LayerDictionaryToLayerStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            /*IEnumerable<bool> vals = values.OfType<bool>();
            foreach (bool val in vals)
            {
                retVal += val.ToString() + ",";
            }
            retVal.Trim(',');
            return retVal;*/
            if (value is Dictionary<string, bool> val)
            {
                foreach (string key in val.Keys.Where(x => val[x]))
                {
                    retVal += key + ",";
                }
            }

            if (retVal.Length > 0)
            retVal = retVal.Substring(0, retVal.Length - 1);
            //Debug.WriteLine("retVal: " + retVal);
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}