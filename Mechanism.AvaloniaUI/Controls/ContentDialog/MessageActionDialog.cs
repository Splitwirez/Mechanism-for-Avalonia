using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ContentDialog
{
    public class MessageActionDialog : MessageDialogBase
    {
        public MessageActionDialog(IContentDialogActionSet set) => DataContext = set;

        public void SelectAction(IContentDialogAction parameter)
        {
            ActionSelected?.Invoke(this, new DialogEventArgs(parameter.Value));
            (Parent as ContentDialogFrame).IsVisible = false;
        }

        public event EventHandler<DialogEventArgs> ActionSelected;
    }

    public class DialogEventArgs : EventArgs
    {
        public object Value { get; set; }
        private DialogEventArgs() { }
        public DialogEventArgs(object value) => Value = value;
    }
}
