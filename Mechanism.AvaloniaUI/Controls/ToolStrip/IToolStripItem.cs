using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public interface IToolStripItem
    {
        IControlTemplate Template { get; set; }

        ICommand Command { get; set; }

        object CommandParameter { get; set; }

        string DisplayName { get; set; }

        bool AllowDuplicates { get; set; }

        ToolStrip Owner { get; set; }
    }
}
