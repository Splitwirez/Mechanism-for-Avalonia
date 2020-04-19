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
    public class SplitButton : Button
    {
        public static readonly StyledProperty<object> FlyoutContentProperty =
            FlyoutButton.FlyoutContentProperty.AddOwner<SplitButton>();

        public object FlyoutContent
        {
            get => GetValue(FlyoutContentProperty);
            set => SetValue(FlyoutContentProperty, value);
        }

        public static readonly StyledProperty<IDataTemplate> FlyoutContentTemplateProperty =
            FlyoutButton.FlyoutContentTemplateProperty.AddOwner<SplitButton>();

        public IDataTemplate FlyoutContentTemplate
        {
            get => GetValue(FlyoutContentTemplateProperty);
            set => SetValue(FlyoutContentTemplateProperty, value);
        }

        public static readonly StyledProperty<IInputElement> FocusOnOpenElementProperty =
            FlyoutButton.FocusOnOpenElementProperty.AddOwner<SplitButton>();

        public IInputElement FocusOnOpenElement
        {
            get => GetValue(FocusOnOpenElementProperty);
            set => SetValue(FocusOnOpenElementProperty, value);
        }

        public static readonly StyledProperty<bool> AutoCloseFlyoutProperty =
            FlyoutButton.AutoCloseFlyoutProperty.AddOwner<SplitButton>();

        public bool AutoCloseFlyout
        {
            get => GetValue(AutoCloseFlyoutProperty);
            set => SetValue(AutoCloseFlyoutProperty, value);
        }


        public static readonly StyledProperty<bool> IsFlyoutOpenProperty =
       AvaloniaProperty.Register<SplitButton, bool>(nameof(IsFlyoutOpen), defaultValue: false);

        public bool IsFlyoutOpen
        {
            get => GetValue(IsFlyoutOpenProperty);
            set => SetValue(IsFlyoutOpenProperty, value);
        }

        public static readonly StyledProperty<bool> IsFlyoutSegmentVisibleProperty =
        AvaloniaProperty.Register<SplitButton, bool>(nameof(IsFlyoutSegmentVisible), defaultValue: true);

        public bool IsFlyoutSegmentVisible
        {
            get => GetValue(IsFlyoutSegmentVisibleProperty);
            set => SetValue(IsFlyoutSegmentVisibleProperty, value);
        }

        public static readonly StyledProperty<bool> IsFlyoutSegmentEnabledProperty =
        AvaloniaProperty.Register<SplitButton, bool>(nameof(IsFlyoutSegmentEnabled), defaultValue: true);

        public bool IsFlyoutSegmentEnabled
        {
            get => GetValue(IsFlyoutSegmentEnabledProperty);
            set => SetValue(IsFlyoutSegmentEnabledProperty, value);
        }


        /*FlyoutButton _flyoutButton = null;
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            _flyoutButton = e.NameScope.Get<FlyoutButton>("PART_FlyoutButton");
        }*/
    }
}
