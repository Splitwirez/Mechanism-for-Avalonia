using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.Windows
{
    public class DefaultDecoratableWindowImpl : IDecoratableWindowImpl
    {
        DecoratableWindow _window;
        public DecoratableWindow Window
        {
            get => _window;
            set => _window = value;
        }

        public bool GetCanBlur()
        {
            return false;
        }

        public void SetBlur(bool enable)
        {
            
        }

        public void SetExtendedTitlebar(double height)
        {

        }

        public bool GetCanExtendTitlebar()
        {
            return false;
        }

        public void SetShowIcon(bool show)
        {
            
        }

        public bool GetCanControlIcon()
        {
            return false;
        }

        public void SetShowTitle(bool show)
        {
            
        }

        public bool GetCanControlTitle()
        {
            return false;
        }

        public event EventHandler<EventArgs> CanBlurChanged;
    }
}
