using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Mechanism.AvaloniaUI.Controls.Windows;
using System;

namespace Mechanism.AvaloniaUI.Sample
{
    public class SampleStyleableWindow : StyleableWindow
    {
        public SampleStyleableWindow()
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

        private void TitlebarPlacementComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TitlebarPlacement = (TitlebarPlacementMode)(sender as ComboBox).SelectedIndex;
        }

        private void TitlebarVisibilityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TitlebarVisibility = (TitlebarVisibilityMode)(sender as ComboBox).SelectedIndex;
        }

        private void HorizontalCaptionAlignmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HorizontalCaptionAlignment = (HorizontalAlignment)(sender as ComboBox).SelectedIndex;
        }

        IBrush _prevBackground = null;
        private void SetBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            if (_prevBackground == null)
            {
                _prevBackground = Background;
                Background = new LinearGradientBrush()
                {
                    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                    EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                    GradientStops = new GradientStops()
                    {
                        new GradientStop(Colors.Red, 0),
                        new GradientStop(Colors.Orange, 0.2),
                        new GradientStop(Colors.Yellow, 0.4),
                        new GradientStop(Colors.Green, 0.6),
                        new GradientStop(Colors.Blue, 0.8),
                        new GradientStop(Colors.Purple, 1)
                    }
                };
            }
            else
            {
                Background = _prevBackground;
                _prevBackground = null;
            }
        }
    }
}
