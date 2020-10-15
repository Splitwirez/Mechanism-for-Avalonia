using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Mechanism.AvaloniaUI.Controls.BlenderBar;
using Mechanism.AvaloniaUI.Controls.CommandBar;
using Mechanism.AvaloniaUI.Controls.ContentDialog;
using Mechanism.AvaloniaUI.Controls.ToolStrip;
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

        public string Greetongs { get; set; } = "Henlo world!";
        TextBlock _lastItemTextBlock = null;
        TextBlock _lastSegmentTextBlock = null;
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

            this.Find<BlenderBar>("BlenderBar").SelectionChanged += (sneder, args) => 
            {
                if ((args.AddedItems != null) && (args.AddedItems.Count > 0) && (args.AddedItems[0] is BlenderBarItem item))
                    Console.WriteLine("Selected " + item.Header);
            };

            _lastItemTextBlock = this.Find<TextBlock>("LastItemTextBlock");
            _lastSegmentTextBlock = this.Find<TextBlock>("LastSegmentTextBlock");
        }

        public void TestSegmentedControlToolStripItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string val = "{x:Null}";
            if ((e.AddedItems != null) && (e.AddedItems.Count > 0))
            {
                if (e.AddedItems[0] is SegmentedControlToolStripItemSegment segment)
                    val = segment.DisplayName;
                else if (e.AddedItems[0] is ContentControl ctrl)
                    val = ctrl.ToString();
                else
                    val = e.AddedItems[0].ToString();
            }
            
            _lastSegmentTextBlock.Text = val;
        }

        public void WriteToConsoleCommand(object parameter)
        {
            Console.WriteLine("parameter: " + parameter);

            string val = "{x:Null}";
            if (parameter != null)
                val = parameter.ToString() + ", " + parameter.GetType().FullName;
            
            _lastItemTextBlock.Text = val;
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
            //new SampleStyleableWindow().Show();
        }

        public void ShowSampleNativeIntegrationWindowButton_Click(object sender, RoutedEventArgs e)
        {
            //new SampleNativeIntegrationWindow().Show();
        }

        public void ShowPretendPhoneWindowButton_Click(object sender, RoutedEventArgs e)
        {
            new TouchDemoWindow().Show();
        }
    }
}
