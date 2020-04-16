using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Mechanism.AvaloniaUI.Sample
{
    public class ThemeDemoWindow : Window
    {
        public ThemeDemoWindow()
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
