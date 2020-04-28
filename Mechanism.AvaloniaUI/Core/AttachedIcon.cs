using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Core
{
    public class AttachedIcon
    {
        /*[Obsolete]
        public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.RegisterAttached<AttachedIcon, Control, object>("Icon", null);

        [Obsolete]
        public static object GetIcon(IAvaloniaObject obj)
        {
            return obj.GetValue(IconProperty);
        }

        [Obsolete]
        public static void SetIcon(IAvaloniaObject obj, object value)
        {
            obj.SetValue(IconProperty, value);
        }*/

        public static readonly StyledProperty<IControlTemplate> IconProperty =
            AvaloniaProperty.RegisterAttached<AttachedIcon, Control, IControlTemplate>("Icon", null);

        public static object GetIcon(IAvaloniaObject obj)
        {
            return obj.GetValue(IconProperty);
        }

        public static void SetIcon(IAvaloniaObject obj, IControlTemplate value)
        {
            obj.SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<double> IconGapProperty = AvaloniaProperty.RegisterAttached<AttachedIcon, Control, double>("IconGap", 0.0);

        public static double GetIconGap(IAvaloniaObject obj)
        {
            return obj.GetValue(IconGapProperty);
        }

        public static void SetIconGap(IAvaloniaObject obj, double value)
        {
            obj.SetValue(IconGapProperty, value);
        }
    }
}
