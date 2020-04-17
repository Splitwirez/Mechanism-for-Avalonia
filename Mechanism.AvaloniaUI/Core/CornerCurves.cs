using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using System;
using System.Globalization;

namespace Mechanism.AvaloniaUI.Core
{
    public class CornerCurves : Layoutable, IEquatable<CornerCurves>
    {
        public static readonly AttachedProperty<CornerCurves> CornerCurvesProperty = AvaloniaProperty.RegisterAttached<CornerCurves, Control, CornerCurves>(name: "CornerCurves", defaultValue: new CornerCurves(), defaultBindingMode: BindingMode.TwoWay);

        public static CornerCurves GetCornerCurves(IAvaloniaObject obj)
        {
            return obj.GetValue(CornerCurvesProperty);
        }

        public static void SetCornerCurves(IAvaloniaObject obj, CornerCurves value)
        {
            obj.SetValue(CornerCurvesProperty, value);
        }

        public static readonly StyledProperty<bool> TopLeftProperty = AvaloniaProperty.Register<CornerCurves, bool>(nameof(TopLeft), defaultValue: true);
        
        public bool TopLeft
        {
            get => GetValue(TopLeftProperty);
            set => SetValue(TopLeftProperty, value);
        }

        public static readonly StyledProperty<bool> TopRightProperty = AvaloniaProperty.Register<CornerCurves, bool>(nameof(TopRight), defaultValue: true);

        public bool TopRight
        {
            get => GetValue(TopRightProperty);
            set => SetValue(TopRightProperty, value);
        }

        public static readonly StyledProperty<bool> BottomRightProperty = AvaloniaProperty.Register<CornerCurves, bool>(nameof(BottomRight), defaultValue: true);

        public bool BottomRight
        {
            get => GetValue(BottomRightProperty);
            set => SetValue(BottomRightProperty, value);
        }

        public static readonly StyledProperty<bool> BottomLeftProperty = AvaloniaProperty.Register<CornerCurves, bool>(nameof(BottomLeft), defaultValue: true);

        public bool BottomLeft
        {
            get => GetValue(BottomLeftProperty);
            set => SetValue(BottomLeftProperty, value);
        }

        
        public CornerCurves() { }
        public CornerCurves(bool uniformValue) : this(uniformValue, uniformValue, uniformValue, uniformValue) { }
        public CornerCurves(bool top, bool bottom) : this(top, top, bottom, bottom) { }
        public CornerCurves(bool topLeft, bool topRight, bool bottomRight, bool bottomLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
        }

        public static CornerCurves Parse(string s)
        {
            string[] parts = null;
            if (bool.TryParse(s, out bool resultVal))
                return new CornerCurves(resultVal);
            else if (s.Contains(","))
                parts = s.Replace(" ", string.Empty).Split(',');
            else if (s.Contains(" "))
                parts = s.Split(' ');

            if ((parts.Length == 2) && bool.TryParse(parts[0], out bool top) && bool.TryParse(parts[1], out bool bottom))
                return new CornerCurves(top, bottom);
            else if ((parts.Length == 4) && bool.TryParse(parts[0], out bool topLeft) && bool.TryParse(parts[1], out bool topRight) && bool.TryParse(parts[2], out bool bottomRight) && bool.TryParse(parts[3], out bool bottomLeft))
                return new CornerCurves(topLeft, topRight, bottomRight, bottomLeft);

            throw new Exception("bad CornerCurves string!");
        }


        public bool Equals(CornerCurves other)
        {
            return
                (TopLeft == other.TopLeft) &&
                (TopRight == other.TopRight) &&
                (BottomRight == other.BottomRight) &&
                (BottomLeft == other.BottomLeft);
        }

        public override string ToString()
        {
            return TopLeft + ", " + TopRight + ", " + BottomRight + ", " + BottomLeft;
        }
    }

    public class CornerCurvesToCornerRadiiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CornerCurves val = (CornerCurves)value;
            string[] radiusStrings = parameter.ToString().Split(';');
            if (radiusStrings.Length == 2)
            {
                CornerRadius trueRadius = CornerRadius.Parse(radiusStrings[0]);
                CornerRadius falseRadius = CornerRadius.Parse(radiusStrings[1]);

                double retTopLeft = trueRadius.TopLeft;
                double retTopRight = trueRadius.TopRight;
                double retBottomRight = trueRadius.BottomRight;
                double retBottomLeft = trueRadius.BottomLeft;
                
                if (!val.TopLeft)
                    retTopLeft = falseRadius.TopLeft;

                if (!val.TopRight)
                    retTopRight = falseRadius.TopRight;

                if (!val.BottomRight)
                    retBottomRight = falseRadius.BottomRight;

                if (!val.BottomLeft)
                    retBottomLeft = falseRadius.BottomLeft;

                return new CornerRadius(retTopLeft, retTopRight, retBottomRight, retBottomLeft);
            }
            throw new InvalidOperationException("Invalid parameter value.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
