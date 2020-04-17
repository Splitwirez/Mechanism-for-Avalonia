using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using System;

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

        public void SetTheme(Uri uri)
        {
            IClassicDesktopStyleApplicationLifetime desktop = null;
            bool isDesktop = false;
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktp)
            {
                desktop = desktp;
                isDesktop = true;
            }


            if (isDesktop)
                desktop.MainWindow.Close();

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

            if (isDesktop)
            {
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.Show();
            }
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
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.Show();
            }
        }
    }
}
