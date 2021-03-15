using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Mechanism.AvaloniaUI.Controls.ToolStrip;

namespace Mechanism.AvaloniaUI.Sample.Views
{
    public class AttachedIconPageView : UserControl
    {
        public AttachedIconPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
