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

        public event EventHandler<EventArgs> CanBlurChanged;
    }
}
