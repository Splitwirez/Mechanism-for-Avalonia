using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.Windows
{
    public interface IDecoratableWindowImpl
    {
        DecoratableWindow Window { get; set; }

        void SetBlur(bool enable);

        bool GetCanBlur();

        event EventHandler<EventArgs> CanBlurChanged;
    }
}
