using Avalonia;
using Avalonia.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class ToolStripItemReference : AvaloniaObject
    {
        public static readonly StyledProperty<IToolStripItem> TargetItemProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, IToolStripItem>(nameof(TargetItem));

        public IToolStripItem TargetItem
        {
            get => GetValue(TargetItemProperty);
            set => SetValue(TargetItemProperty, value);
        }
    }

    public static class ToolStripItemExtensions
    {

        public static ToolStripItemReference ToReference(this IToolStripItem item)
        {
            return new ToolStripItemReference()
            {
                [!ToolStripItemReference.TargetItemProperty] = new Binding() { Source = item }
            };
        }
    }
}
