using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform;
using Avalonia.Styling;
using Mechanism.AvaloniaUI.Controls.CommandBar;
using Mechanism.AvaloniaUI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mechanism.AvaloniaUI.Sample
{
    public class MainWindow : DecoratableWindow
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            //Closing += MainWindow_Closing;
            HasSystemDecorations = true;
        }

        /*private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }*/

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var commandBar = this.Find<CommandBar>("CommandBar");
            this.Find<RadioButton>("CommandBarLeftRadioButton").Checked += (sneder, args) => commandBar.HorizontalItemsAlignment = Controls.ChildrenHorizontalAlignment.Left;
            this.Find<RadioButton>("CommandBarRightRadioButton").Checked += (sneder, args) => commandBar.HorizontalItemsAlignment = Controls.ChildrenHorizontalAlignment.Right;

            this.Find<Button>("FileListmakerDialogButton").Click += FileListmakerDialogButton_Click;
            this.Find<Button>("DefaultThemeButton").Click += (sneder, args) => SetTheme(null);
            this.Find<Button>("AeroThemeButton").Click += (sneder, args) => SetTheme("avares://Mechanism.AvaloniaUI.Themes.Aero.NormalColor/Themes/Aero.NormalColor.xaml");
            this.Find<Button>("SlateThemeButton").Click += (sneder, args) => SetTheme("avares://Mechanism.AvaloniaUI.Themes.Slate/Themes/Slate.xaml");
            this.Find<Button>("JadeThemeButton").Click += (sneder, args) => SetTheme("avares://Mechanism.AvaloniaUI.Themes.Jade/Themes/Jade.xaml");
            //new ThemeDemoWindow().Show();
        }

        void SetTheme(string styleIncludeUri)
        {
            if (styleIncludeUri != null)
            {
                var uri = new Uri(styleIncludeUri);
                (App.Current as App).SetTheme(uri);
            }
            else
                (App.Current as App).SetTheme(null);
        }

        private void FileListmakerDialogButton_Click(object sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            /*OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowAsync(this);*/
        }
    }
}
