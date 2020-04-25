using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using System;
using System.Linq;

namespace Mechanism.AvaloniaUI.Sample
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            //Styles.CollectionChanged += (sender, e) => RefreshAllStyles();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static readonly IStyle DummyStyle = new Style();
        public static bool UseSystemDecorations = false;

        static Uri _lastThemeUri = null;
        public static void ResetTheme() => SetTheme(_lastThemeUri, true);
        public static void SetTheme(Uri uri) => SetTheme(uri, false);
        public static void SetTheme(Uri uri, bool resetWindows)
        {
            IClassicDesktopStyleApplicationLifetime desktop = null;
            bool isDesktop = false;
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktp)
            {
                desktop = desktp;
                isDesktop = true;
            }


            if (isDesktop && resetWindows)
            {
                //desktop.MainWindow.Close();
                var windows = desktop.Windows.ToList();
                foreach (Avalonia.Controls.Window w in windows)
                    w.Close();
            }

            //App.Current.Styles[3] = style;
            if (App.Current.Styles.Count == 4)
                App.Current.Styles.RemoveAt(3);
            //(App.Current.Styles[3] as StyleInclude).Source = uri;
            //else
            if (uri != null)
                App.Current.Styles.Add(new StyleInclude(uri)
                {
                    Source = uri
                });
            //(App.Current as App).RefreshAllStyles();

            if (isDesktop && resetWindows)
            {
                desktop.MainWindow = new SampleDecoratableWindow();
                desktop.MainWindow.Show();
            }

            _lastThemeUri = uri;
        }

        public void RefreshAllStyles()
        {
            /*foreach (var window in ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime).Windows)
            {
                window.Styles.Add(DummyStyle);
                window.Styles.Remove(DummyStyle);
            }*/
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow.Close();
                desktop.MainWindow = new SampleDecoratableWindow();
                desktop.MainWindow.Show();
            }
        }
    }
}
