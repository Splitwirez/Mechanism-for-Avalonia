﻿using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls
{
    public enum ThemeControlStyle
    {
        None,
        Button,
        NavigationButton,
        ToolTip,
        Menu,
        MenuItem,
        TextBox,
        CaptionButton,
        PaneFrame,
        TabBody,
        TabItem,
        ControlBar
    }

    public class ThemeControlDecorator : ContentControl
    {
        public static readonly StyledProperty<ThemeControlStyle> ControlStyleProperty =
        AvaloniaProperty.Register<ThemeControlDecorator, ThemeControlStyle>(nameof(ControlStyle), defaultValue: ThemeControlStyle.NavigationButton);

        public ThemeControlStyle ControlStyle
        {
            get { return GetValue(ControlStyleProperty); }
            set { SetValue(ControlStyleProperty, value); }
        }

        public static readonly StyledProperty<bool> IsVisuallyPointerOverProperty =
        AvaloniaProperty.Register<ThemeControlDecorator, bool>(nameof(IsVisuallyPointerOver), defaultValue: false);

        public bool IsVisuallyPointerOver
        {
            get { return GetValue(IsVisuallyPointerOverProperty); }
            set { SetValue(IsVisuallyPointerOverProperty, value); }
        }

        public static readonly StyledProperty<bool> IsVisuallyPressedProperty =
        AvaloniaProperty.Register<ThemeControlDecorator, bool>(nameof(IsVisuallyPressed), defaultValue: false);

        public bool IsVisuallyPressed
        {
            get { return GetValue(IsVisuallyPressedProperty); }
            set { SetValue(IsVisuallyPressedProperty, value); }
        }

        public static readonly StyledProperty<bool> IsVisuallyEnabledProperty =
        AvaloniaProperty.Register<ThemeControlDecorator, bool>(nameof(IsVisuallyEnabled), defaultValue: true);

        public bool IsVisuallyEnabled
        {
            get { return GetValue(IsVisuallyEnabledProperty); }
            set { SetValue(IsVisuallyEnabledProperty, value); }
        }

        public static readonly StyledProperty<bool?> IsVisuallyCheckedProperty =
        AvaloniaProperty.Register<ThemeControlDecorator, bool?>(nameof(IsVisuallyChecked)/*, defaultValue: false*/);

        public bool? IsVisuallyChecked
        {
            get { return GetValue(IsVisuallyCheckedProperty); }
            set { SetValue(IsVisuallyCheckedProperty, value); }
        }

        static ThemeControlDecorator()
        {
            IsVisuallyCheckedProperty.Changed.AddClassHandler<ThemeControlDecorator>(new Action<ThemeControlDecorator, AvaloniaPropertyChangedEventArgs>((sender, args) => sender.UpdatePseudoClasses((bool?)args.NewValue)));
        }

        public ThemeControlDecorator()
        {
            UpdatePseudoClasses(IsVisuallyChecked);
        }

        void UpdatePseudoClasses(bool? isVisuallyChecked)
        {
            PseudoClasses.Set(":indeterminate", !isVisuallyChecked.HasValue);
        }
    }
}
