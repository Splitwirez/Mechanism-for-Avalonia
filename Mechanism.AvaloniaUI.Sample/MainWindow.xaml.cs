using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Mechanism.AvaloniaUI.Controls.CommandBar;
using Mechanism.AvaloniaUI.Controls.ContentDialog;
using System;
using System.Timers;

namespace Mechanism.AvaloniaUI.Sample
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
/*#if DEBUG
            this.AttachDevTools();
#endif*/
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.Find<RadioButton>("DefaultThemeRadioButton").IsChecked = true;
            var themesStackPanel = this.Find<StackPanel>("ThemesStackPanel");
            foreach (RadioButton rbtn in themesStackPanel.Children)
                rbtn.Checked += ThemeRadioButton_Checked;

            var commandBar = this.Find<CommandBar>("CommandBar");
            this.Find<RadioButton>("CommandBarLeftRadioButton").Checked += (sneder, args) => commandBar.HorizontalItemsAlignment = Controls.ChildrenHorizontalAlignment.Left;
            this.Find<RadioButton>("CommandBarRightRadioButton").Checked += (sneder, args) => commandBar.HorizontalItemsAlignment = Controls.ChildrenHorizontalAlignment.Right;
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

        public void ShowSampleNativeIntegrationWindowButton_Click(object sender, RoutedEventArgs e)
        {
            new SampleNativeIntegrationWindow().Show();
        }

        public void ShowPretendPhoneWindowButton_Click(object sender, RoutedEventArgs e)
        {
            new TouchDemoWindow().Show();
        }
    }
}
