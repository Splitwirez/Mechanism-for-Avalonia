using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ContentDialog
{
    public class BasicMessageDialog : TemplatedControl
    {
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<BasicMessageDialog, string>(nameof(Title), defaultValue: string.Empty);

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<BasicMessageDialog, string>(nameof(Message), defaultValue: string.Empty);

        public string Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            Button okButton = e.NameScope.Get<Button>("PART_OkButton");
            okButton.Click += (sneder, args) => (Parent as ContentDialogFrame).IsVisible = false;
            okButton.Focus();
        }
    }
}
