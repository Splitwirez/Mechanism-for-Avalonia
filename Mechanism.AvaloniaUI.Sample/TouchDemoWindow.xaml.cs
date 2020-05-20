using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Mechanism.AvaloniaUI.Sample
{
    public class TouchDemoWindow : Window
    {
        public TouchDemoWindow()
        {
            this.InitializeComponent();
/*#if DEBUG
            this.AttachDevTools();
#endif*/
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
