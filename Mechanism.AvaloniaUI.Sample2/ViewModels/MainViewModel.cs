using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Mechanism.AvaloniaUI.Sample2.Views;

namespace Mechanism.AvaloniaUI.Sample2.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Hello World!";


        bool _canGoHome = false;
        public bool CanGoHome
        {
            get => _canGoHome;
            set
            {
                _canGoHome = value;
                NotifyPropertyChanged();
            }
        }

        public void GoHome(object parameter = null)
        {
            CurrentView = null; //new HomePageView();
        }

        UserControl _currentView = null;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                NotifyPropertyChanged();

                CanGoHome = _currentView != null;
            }
        }

        public void GoToView(object parameter)
        {
            if (parameter is UserControl ctrl)
                CurrentView = ctrl;
        }

        public MainViewModel()
        {
            CurrentView = null;
            //CanGoHome = _currentView != null;
        }
    }
}
