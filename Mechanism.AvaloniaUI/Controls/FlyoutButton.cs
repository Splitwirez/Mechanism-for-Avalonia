using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls
{
    public class FlyoutButton : ToggleButton, IStyleable
    {
        /*public static readonly StyledProperty<bool> IsFlyoutOpenProperty =
            AvaloniaProperty.Register<OverflowFlyoutItemsControl, bool>(nameof(IsFlyoutOpen), defaultValue: false);

        public bool IsFlyoutOpen
        {
            get => GetValue(IsFlyoutOpenProperty);
            set => SetValue(IsFlyoutOpenProperty, value);
        }*/

        Type IStyleable.StyleKey => typeof(FlyoutButton);

        public static readonly StyledProperty<object> FlyoutContentProperty =
            AvaloniaProperty.Register<FlyoutButton, object>(nameof(FlyoutContent));

        public object FlyoutContent
        {
            get => GetValue(FlyoutContentProperty);
            set => SetValue(FlyoutContentProperty, value);
        }

        public static readonly StyledProperty<IDataTemplate> FlyoutContentTemplateProperty =
            AvaloniaProperty.Register<FlyoutButton, IDataTemplate>(nameof(FlyoutContentTemplate));

        public IDataTemplate FlyoutContentTemplate
        {
            get => GetValue(FlyoutContentTemplateProperty);
            set => SetValue(FlyoutContentTemplateProperty, value);
        }

        public static readonly StyledProperty<IInputElement> FocusOnOpenElementProperty =
            AvaloniaProperty.Register<FlyoutButton, IInputElement>(nameof(FocusOnOpenElement));

        public IInputElement FocusOnOpenElement
        {
            get => GetValue(FocusOnOpenElementProperty);
            set => SetValue(FocusOnOpenElementProperty, value);
        }

        static FlyoutButton()
        {
            IsCheckedProperty.Changed.AddClassHandler<FlyoutButton>(new Action<FlyoutButton, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                if (e.NewValue is bool isChecked)
                {
                    if (isChecked)
                        sender.FocusOnOpenElement?.Focus();
                    else
                        sender.Focus();
                }
            }));
        }

        InputElement _buttonArea = null;
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            /*ContentPresenter flCnPresenter = e.NameScope.Get<ContentPresenter>("PART_FlyoutContentPresenter");
            if (flCnPresenter != null)
                flCnPresenter.PointerPressed += (sender, args) => args.Handled = true;*/
            _buttonArea = e.NameScope.Get<InputElement>("PART_ButtonArea");
        }

        protected override void Toggle()
        {
            if ((_buttonArea != null) && _buttonArea.IsPointerOver)
                base.Toggle();
        }
    }
}
