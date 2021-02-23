using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;

namespace Mechanism.AvaloniaUI.Sample.Views
{
    public class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var lightsToggleSwitch = this.Find<ToggleSwitch>("LightsToggleSwitch");
            lightsToggleSwitch.Checked += (sneder, e) => RefreshLights(FluentThemeMode.Light);
            lightsToggleSwitch.Unchecked += (sneder, e) => RefreshLights(FluentThemeMode.Dark);
        }

        Uri _baseUri = new Uri("avares://Mechanism.AvaloniaUI.Sample/Styles");
        void RefreshLights(FluentThemeMode mode)
        {
            /*App.Current.Styles.RemoveAt(0);
            App.Current.Styles.Insert(0, new FluentTheme(_baseUri)
            {
                Mode = mode
            });*/

            /*
            <StyleInclude Source="avares://Avalonia.Themes.Fluent/Accents/BaseDark.xaml" />
            <StyleInclude Source="avares://Avalonia.Themes.Fluent/Accents/Base.xaml" />
            <StyleInclude Source="avares://Avalonia.Themes.Fluent/Accents/FluentBaseDark.xaml" />
            <StyleInclude Source="avares://Avalonia.Themes.Fluent/Accents/FluentControlResourcesDark.xaml" />
            <StyleInclude Source="avares://Avalonia.Themes.Fluent/Controls/FluentControls.xaml" />
            */

            App.Current.Styles[0] = new StyleInclude(_baseUri)
            {
                Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/Base" + mode + ".xaml")
            };

            App.Current.Styles[2] = new StyleInclude(_baseUri)
            {
                Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/FluentBase" + mode + ".xaml")
            };

            App.Current.Styles[3] = new StyleInclude(_baseUri)
            {
                Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/FluentControlResources" + mode + ".xaml")
            };
        }
    }
}
