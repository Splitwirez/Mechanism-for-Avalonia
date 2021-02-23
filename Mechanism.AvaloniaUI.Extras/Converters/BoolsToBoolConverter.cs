using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Mechanism.AvaloniaUI.Extras.Converters
{
    public enum BoolsCombinationMode
    {
        Any,
        All,
        None
    }

    public class BoolsToBoolConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            var vals = values.OfType<bool>();
            if (vals.Count() == 0)
                return false;
            else
            {
                BoolsCombinationMode mode = BoolsCombinationMode.Any;
                if ((parameter != null) && Enum.TryParse(parameter.ToString(), out BoolsCombinationMode outMode))
                    mode = outMode;

                if (mode == BoolsCombinationMode.Any)
                    return vals.Any(x => x);
                else if (mode == BoolsCombinationMode.All)
                    return vals.All(x => x);
                else
                    return vals.All(x => !x);
            }
        }
    }
}
