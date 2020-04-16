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
            this.Find<Button>("AeroThemeButton").Click += (sneder, args) => ShowThemeWindow("Windows Aero", "avares://Mechanism.AvaloniaUI.Themes.Aero.NormalColor/Themes/Aero.NormalColor.xaml");
            this.Find<Button>("SlateThemeButton").Click += (sneder, args) => ShowThemeWindow("Slate", "avares://Mechanism.AvaloniaUI.Themes.Slate/Themes/Slate.xaml");
            this.Find<Button>("JadeThemeButton").Click += (sneder, args) => ShowThemeWindow("Jade", "avares://Mechanism.AvaloniaUI.Themes.Jade/Themes/Jade.xaml");
            //new ThemeDemoWindow().Show();
        }

        void ShowThemeWindow(string name, string styleIncludeUri)
        {
            /*var win = new ThemeDemoWindow();
            win.Title = win.Title.Replace("%THEMENAME%", name);
            if (win.Styles.Count > 0)
                win.Styles.RemoveAt(0);

            win.Styles.Add(new StyleInclude(uri)
            {
                Source = uri
            });
            win.Show();*/
            var uri = new Uri(styleIncludeUri);
            if (Application.Current.Styles.Count == 4)
                (Application.Current.Styles[3] as StyleInclude).Source = uri;
            else
                Application.Current.Styles.Add(new StyleInclude(uri)
                {
                    Source = uri
                });

            List<IStyle> styles = new List<IStyle>();
            //foreach (IStyle style in Application.Current.Styles)
            for (int i = 0; i < Application.Current.Styles.Count; i++)
            {
                styles.Add(Application.Current.Styles[0]);
                //Styles.RemoveAt(0);
            }
            Application.Current.Styles.Clear();
            Application.Current.Styles.AddRange(styles);
        }

        private void FileListmakerDialogButton_Click(object sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            /*OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowAsync(this);*/
        }
    }
}
