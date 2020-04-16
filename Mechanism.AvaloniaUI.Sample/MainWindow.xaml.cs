using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform;
using Avalonia.Styling;
using Mechanism.AvaloniaUI.Controls.Windows;
using System;
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
            this.Find<Button>("FileListmakerDialogButton").Click += FileListmakerDialogButton_Click;
            this.Find<Button>("AeroThemeButton").Click += (sneder, args) => ShowThemeWindow("Mechanism.AvaloniaUI.Themes.Aero.NormalColor", $"avares://Mechanism.AvaloniaUI.Themes.Aero.NormalColor/Themes/Aero.NormalColor.xaml");
            this.Find<Button>("SlateThemeButton").Click += (sneder, args) => ShowThemeWindow("Mechanism.AvaloniaUI.Themes.Slate", $"avares://Mechanism.AvaloniaUI.Themes.Slate/Themes/Slate.xaml");
            this.Find<Button>("JadeThemeButton").Click += (sneder, args) => ShowThemeWindow("Mechanism.AvaloniaUI.Themes.Jade", $"avares://Mechanism.AvaloniaUI.Themes.Jade/Themes/Jade.xaml");
            //new ThemeDemoWindow().Show();
        }

        void ShowThemeWindow(string space, string styleIncludeUri)
        {
            var win = new ThemeDemoWindow();
            //(assets.Open(new Uri(uri)));
            //assets.op
            //System.IO.Stream stream = assets.Open(new System.Uri(styleIncludeUri));
            //win.Styles.Add(new StyleInclude(new Uri(styleIncludeUri), );
            //AvaloniaXamlLoader loader = new AvaloniaXamlLoader();


            /*var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            using (var stream = new StreamReader(assets.Open(new Uri(styleIncludeUri))))
            {
                var xaml = stream.ReadToEnd();
                var style = AvaloniaXamlLoader.Parse<IStyle>(xaml);
                win.Styles.Add(style);
            }*/
            win.Styles.Add(new StyleInclude(new Uri("Mechanism.AvaloniaUI.Themes.Aero.NormalColor", UriKind.Absolute))
            {
                Source = new Uri("avares://Themes/Aero.NormalColor.xaml", UriKind.Relative)
            });
            win.Show();
        }

        private void FileListmakerDialogButton_Click(object sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            /*OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowAsync(this);*/
        }
    }
}
