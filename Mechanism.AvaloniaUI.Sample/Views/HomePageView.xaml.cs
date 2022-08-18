using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Mechanism.AvaloniaUI.Sample.Views
{
    public class HomePageView : UserControl, IStyleable
    {
        public HomePageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

#if SHOW_TESTING_GROUNDS
            this.Find<HeaderedContentControl>("TestingGrounds").IsVisible = true;
#endif
        }
    }
}
