using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Mechanism.AvaloniaUI.Sample.Views
{
    public class CommandBarPageView : UserControl
    {
        public CommandBarPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
