using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Mechanism.AvaloniaUI.Core
{
    public class BorderPresence : AvaloniaObject, IEquatable<BorderPresence>
    {
        public static readonly AttachedProperty<BorderPresence> BorderPresenceProperty = AvaloniaProperty.RegisterAttached<BorderPresence, Control, BorderPresence>("BorderPresence", new BorderPresence());

        public static BorderPresence GetBorderPresence(IAvaloniaObject obj)
        {
            return obj.GetValue(BorderPresenceProperty);
        }

        public static void SetBorderPresence(IAvaloniaObject obj, BorderPresence value)
        {
            obj.SetValue(BorderPresenceProperty, value);
        }

        public static readonly StyledProperty<bool> LeftProperty = AvaloniaProperty.Register<BorderPresence, bool>(nameof(Left), defaultValue: true);

        public bool Left
        {
            get => GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        public static readonly StyledProperty<bool> TopProperty = AvaloniaProperty.Register<BorderPresence, bool>(nameof(Top), defaultValue: true);

        public bool Top
        {
            get => GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }

        public static readonly StyledProperty<bool> RightProperty = AvaloniaProperty.Register<BorderPresence, bool>(nameof(Right), defaultValue: true);

        public bool Right
        {
            get => GetValue(RightProperty);
            set => SetValue(RightProperty, value);
        }

        public static readonly StyledProperty<bool> BottomProperty = AvaloniaProperty.Register<BorderPresence, bool>(nameof(Bottom), defaultValue: true);

        public bool Bottom
        {
            get => GetValue(BottomProperty);
            set => SetValue(BottomProperty, value);
        }


        public BorderPresence() { }
        public BorderPresence(bool uniformValue) : this(uniformValue, uniformValue, uniformValue, uniformValue) { }
        public BorderPresence(bool top, bool bottom) : this(top, top, bottom, bottom) { }
        public BorderPresence(bool left, bool top, bool right, bool bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static BorderPresence Parse(string s)
        {
            string[] parts = null;
            if (bool.TryParse(s, out bool resultVal))
                return new BorderPresence(resultVal);
            else if (s.Contains(","))
                parts = s.Replace(" ", string.Empty).Split(',');
            else if (s.Contains(" "))
                parts = s.Split(' ');

            if ((parts.Length == 2) && bool.TryParse(parts[0], out bool top) && bool.TryParse(parts[1], out bool bottom))
                return new BorderPresence(top, bottom);
            else if ((parts.Length == 4) && bool.TryParse(parts[0], out bool topLeft) && bool.TryParse(parts[1], out bool topRight) && bool.TryParse(parts[2], out bool bottomRight) && bool.TryParse(parts[3], out bool bottomLeft))
                return new BorderPresence(topLeft, topRight, bottomRight, bottomLeft);

            throw new Exception("bad BorderPresence string!");
        }


        public bool Equals(BorderPresence other)
        {
            return
                (Left == other.Left) &&
                (Top == other.Top) &&
                (Right == other.Right) &&
                (Bottom == other.Bottom);
        }

        public override string ToString()
        {
            return Left + ", " + Top + ", " + Right + ", " + Bottom;
        }
    }

    public class BorderPresenceToThicknessesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BorderPresence val = (BorderPresence)value;
            string[] radiusStrings = parameter.ToString().Split(';');
            if (radiusStrings.Length == 2)
            {
                Thickness trueRadius = Thickness.Parse(radiusStrings[0]);
                Thickness falseRadius = Thickness.Parse(radiusStrings[1]);

                double retLeft = trueRadius.Left;
                double retTop = trueRadius.Right;
                double retRight = trueRadius.Right;
                double retBottom = trueRadius.Bottom;

                if (!val.Left)
                    retLeft = falseRadius.Left;

                if (!val.Top)
                    retTop = falseRadius.Top;

                if (!val.Right)
                    retRight = falseRadius.Right;

                if (!val.Bottom)
                    retBottom = falseRadius.Bottom;

                return new Thickness(retLeft, retTop, retRight, retBottom);
            }
            throw new InvalidOperationException("Invalid parameter value.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
