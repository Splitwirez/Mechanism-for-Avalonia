using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ContentDialog
{
    public class BasicMessageDialog : MessageDialogBase
    {
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            Button okButton = e.NameScope.Get<Button>("PART_OkButton");
            okButton.Click += (sneder, args) => (Parent as ContentDialogFrame).IsVisible = false;
            okButton.Focus();
        }
    }
}
