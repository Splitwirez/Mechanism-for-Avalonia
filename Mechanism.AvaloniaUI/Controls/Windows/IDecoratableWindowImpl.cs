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


        void SetExtendedTitlebar(double height);
        bool GetCanExtendTitlebar();


        void SetShowIcon(bool show);
        bool GetCanControlIcon();


        void SetShowTitle(bool show);
        bool GetCanControlTitle();
    }
}
