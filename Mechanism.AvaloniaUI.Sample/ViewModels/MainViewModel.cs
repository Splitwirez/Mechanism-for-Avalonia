using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Mechanism.AvaloniaUI.Sample.Views;

namespace Mechanism.AvaloniaUI.Sample.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
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


        static readonly string HOME_TITLE = "Home";
        string _currentTitle = HOME_TITLE;
        public string CurrentTitle
        {
            get => _currentTitle;
            set
            {
                _currentTitle = value;
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

                bool newValNotNull = _currentView != null;
                CanGoHome = newValNotNull;

                if (newValNotNull)
                {
                    if (_currentView.DataContext is DemoPageViewModel demoVm)
                        CurrentTitle = demoVm.Title;
                }
                else
                    CurrentTitle = HOME_TITLE;
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
