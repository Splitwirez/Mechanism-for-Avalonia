using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mechanism.AvaloniaUI.Controls.Windows;

namespace Mechanism.AvaloniaUI.Sample
{
    public class ExtendedTitlebarSampleWindow : DecoratableWindow
    {
        public ExtendedTitlebarSampleWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
