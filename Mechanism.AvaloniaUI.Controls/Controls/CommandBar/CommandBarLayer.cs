using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls
{
    public class CommandBarLayer : Control
    {
        public static readonly StyledProperty<string> IdentifierProperty =
        AvaloniaProperty.Register<OverflowFlyoutItemsControl, string>(nameof(Identifier), defaultValue: null);

        public string Identifier
        {
            get => GetValue(IdentifierProperty);
            set => SetValue(IdentifierProperty, value);
        }

        /*public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<OverflowFlyoutItemsControl, bool>(nameof(IsVisible), defaultValue: false);

        public bool IsVisible
        {
            get { return GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }*/

        static CommandBarLayer()
        {
            IsVisibleProperty.Changed.AddClassHandler<CommandBarLayer>(new Action<CommandBarLayer, AvaloniaPropertyChangedEventArgs>((sneder, args) => IsLayerVisibleChanged?.Invoke(sneder, new EventArgs())));
        }

        public static event EventHandler<EventArgs> IsLayerVisibleChanged;
    }
}
