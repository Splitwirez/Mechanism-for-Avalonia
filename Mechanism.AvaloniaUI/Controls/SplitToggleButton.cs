using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls
{
    public class SplitToggleButton : ToggleButton
    {
        public static readonly StyledProperty<object> FlyoutContentProperty =
            FlyoutButton.FlyoutContentProperty.AddOwner<SplitToggleButton>();

        public object FlyoutContent
        {
            get => GetValue(FlyoutContentProperty);
            set => SetValue(FlyoutContentProperty, value);
        }

        public static readonly StyledProperty<IDataTemplate> FlyoutContentTemplateProperty =
            FlyoutButton.FlyoutContentTemplateProperty.AddOwner<SplitToggleButton>();

        public IDataTemplate FlyoutContentTemplate
        {
            get => GetValue(FlyoutContentTemplateProperty);
            set => SetValue(FlyoutContentTemplateProperty, value);
        }

        public static readonly StyledProperty<IInputElement> FocusOnOpenElementProperty =
            FlyoutButton.FocusOnOpenElementProperty.AddOwner<SplitToggleButton>();

        public IInputElement FocusOnOpenElement
        {
            get => GetValue(FocusOnOpenElementProperty);
            set => SetValue(FocusOnOpenElementProperty, value);
        }

        public static readonly StyledProperty<bool> AutoCloseFlyoutProperty =
            FlyoutButton.AutoCloseFlyoutProperty.AddOwner<SplitToggleButton>();

        public bool AutoCloseFlyout
        {
            get => GetValue(AutoCloseFlyoutProperty);
            set => SetValue(AutoCloseFlyoutProperty, value);
        }


        public static readonly StyledProperty<bool> IsFlyoutOpenProperty =
            SplitButton.IsFlyoutOpenProperty.AddOwner<SplitToggleButton>();

        public bool IsFlyoutOpen
        {
            get => GetValue(IsFlyoutOpenProperty);
            set => SetValue(IsFlyoutOpenProperty, value);
        }

        public static readonly StyledProperty<bool> IsFlyoutSegmentVisibleProperty =
            SplitButton.IsFlyoutSegmentVisibleProperty.AddOwner<SplitToggleButton>();

        public bool IsFlyoutSegmentVisible
        {
            get => GetValue(IsFlyoutSegmentVisibleProperty);
            set => SetValue(IsFlyoutSegmentVisibleProperty, value);
        }

        public static readonly StyledProperty<bool> IsFlyoutSegmentEnabledProperty =
            SplitButton.IsFlyoutSegmentEnabledProperty.AddOwner<SplitToggleButton>();

        public bool IsFlyoutSegmentEnabled
        {
            get => GetValue(IsFlyoutSegmentEnabledProperty);
            set => SetValue(IsFlyoutSegmentEnabledProperty, value);
        }

        InputElement _buttonArea = null;
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            _buttonArea = e.NameScope.Get<InputElement>("PART_ButtonArea");
        }

        protected override void Toggle()
        {
            if (_buttonArea.IsPointerOver)
                base.Toggle();
        }
    }
}
