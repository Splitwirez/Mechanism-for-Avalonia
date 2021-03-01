using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Mechanism.AvaloniaUI.Sample.Views
{
    public class TestView : UserControl
    {
        public TestView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}