using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Mechanism.AvaloniaUI.Sample.Views
{
    public class SwipeSheetPageView : UserControl
    {
        public SwipeSheetPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}