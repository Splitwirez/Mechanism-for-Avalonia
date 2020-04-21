using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Mechanism.AvaloniaUI.Controls.CommandBar;
using Mechanism.AvaloniaUI.Controls.ContentDialog;
using Mechanism.AvaloniaUI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

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

            this.Find<Button>("ShowContentDialogButton").Click += ShowContentDialogButton_Click;
            this.Find<Button>("ShowContentDialog2Button").Click += ShowContentDialog2Button_Click;
            this.Find<Button>("ShowContentDialogWithActionsButton").Click += ShowContentDialogWithActionsButton_Click;
            this.Find<Button>("ShowFileListmakerDialogButton").Click += FileListmakerDialogButton_Click;

            this.Find<Button>("ShowDecoratableWindowWithCustomDecorationsButton").Click += ShowDecoratableWindowWithCustomDecorationsButton_Click;
            this.Find<Button>("ShowDecoratableWindowWithSystemDecorationsButton").Click += ShowDecoratableWindowWithSystemDecorationsButton_Click;

            this.Find<Button>("DefaultThemeButton").Click += (sneder, args) => SetTheme(null);
            this.Find<Button>("AeroThemeButton").Click += (sneder, args) => SetTheme("avares://Mechanism.AvaloniaUI.Themes.Aero.NormalColor/Themes/Aero.NormalColor.xaml");
            this.Find<Button>("SlateThemeButton").Click += (sneder, args) => SetTheme("avares://Mechanism.AvaloniaUI.Themes.Slate/Themes/Slate.xaml");
            this.Find<Button>("JadeThemeButton").Click += (sneder, args) => SetTheme("avares://Mechanism.AvaloniaUI.Themes.Jade/Themes/Jade.xaml");
            //new ThemeDemoWindow().Show();
        }

        private void ShowDecoratableWindowWithCustomDecorationsButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            new ExtendedTitlebarSampleWindow().Show();
        }

        private void ShowDecoratableWindowWithSystemDecorationsButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            new ExtendedTitlebarSampleWindow()
            {
                HasSystemDecorations = true
            }.Show();
        }

        private async void ShowContentDialogWithActionsButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            DialogActions.OkCancelButtons result = await ContentDialog.ShowWithActionEnum<DialogActions.OkCancelButtons>("ContentDialog with result", "This is a ContentDialog with a result value.");
            this.Find<TextBlock>("LastActionResultTextBlock").Text = "Last result text: " + result.ToString();
        }

        private void ShowContentDialog2Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ContentDialog.Show("ContentDialog", "This is a ContentDialog. Another one will be queued up if you wait one second before clicking OK.");
            Timer timer = new Timer(1000);
            timer.Elapsed += (sneder, args) =>
            {
                if (ContentDialog.IsShowingDialog)
                    ContentDialog.Show("ContentDialog", "This is another basic ContentDialog.");
                timer.Stop();
            };
            timer.Start();
        }

        private void ShowContentDialogButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ContentDialog.Show("ContentDialog", "This is a ContentDialog.");
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
