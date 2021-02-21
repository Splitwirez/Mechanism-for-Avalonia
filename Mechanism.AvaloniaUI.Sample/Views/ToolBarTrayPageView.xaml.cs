using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Mechanism.AvaloniaUI.Sample.Views
{
    public class ToolBarTrayPageView : UserControl
    {
        public ToolBarTrayPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
