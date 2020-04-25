using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace Mechanism.AvaloniaUI.Sample
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.Find<RadioButton>("DefaultThemeRadioButton").IsChecked = true;
            var themesStackPanel = this.Find<StackPanel>("ThemesStackPanel");
            foreach (RadioButton rbtn in themesStackPanel.Children)
                rbtn.Checked += ThemeRadioButton_Checked;
        }

        public void ThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Uri uri = null;
            string tag = (sender as RadioButton).Tag.ToString();
            if (!tag.Equals("DEFAULT_THEME", StringComparison.OrdinalIgnoreCase))
                uri = new Uri("avares://Mechanism.AvaloniaUI.Themes." + tag + "/Themes/" + tag + ".xaml");

            App.SetTheme(uri);
        }

        public void ShowSampleStyleableWindowButton_Click(object sender, RoutedEventArgs e)
        {
            new SampleStyleableWindow().Show();
        }
    }
}
