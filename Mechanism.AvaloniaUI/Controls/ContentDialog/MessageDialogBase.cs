using Avalonia;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ContentDialog
{
    public class MessageDialogBase : TemplatedControl
    {
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<MessageDialogBase, string>(nameof(Title), defaultValue: string.Empty);

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<MessageDialogBase, string>(nameof(Message), defaultValue: string.Empty);

        public string Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
    }
}
