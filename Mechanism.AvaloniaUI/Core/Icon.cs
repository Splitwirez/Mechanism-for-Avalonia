using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Core
{
    public class AttachedIcon
    {
        public static readonly StyledProperty<object> AttachedIconProperty = AvaloniaProperty.RegisterAttached<AttachedIcon, Control, object>(nameof(AttachedIcon), null);

        public static object GetAttachedIcon(IAvaloniaObject obj)
        {
            return obj.GetValue(AttachedIconProperty);
        }

        public static void SetAttachedIcon(IAvaloniaObject obj, object value)
        {
            obj.SetValue(AttachedIconProperty, value);
        }
    }
}
