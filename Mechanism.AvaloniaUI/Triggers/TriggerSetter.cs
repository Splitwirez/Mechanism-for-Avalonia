using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Avalonia.Data;
using System.Collections.ObjectModel;
using Avalonia.Styling;
using System.Collections.Specialized;

namespace Mechanism.AvaloniaUI.Core
{
    public class TriggerSetter : Visual
    {
        public static readonly StyledProperty<string> TargetNameProperty =
            AvaloniaProperty.Register<TriggerSetter, string>(nameof(TargetName));
        
        public string TargetName
        {
            get => GetValue(TargetNameProperty);
            set => SetValue(TargetNameProperty, value);
        }

        
        public static readonly StyledProperty<string> TargetPropertyProperty =
            AvaloniaProperty.Register<TriggerSetter, string>(nameof(TargetProperty));
        
        public string TargetProperty
        {
            get => GetValue(TargetPropertyProperty);
            set => SetValue(TargetPropertyProperty, value);
        }


        public static readonly StyledProperty<object> ValueProperty =
            AvaloniaProperty.Register<TriggerSetter, object>(nameof(Value));
        
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}