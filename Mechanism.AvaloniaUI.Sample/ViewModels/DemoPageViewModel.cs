using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Mechanism.AvaloniaUI.Sample.Views;

namespace Mechanism.AvaloniaUI.Sample.ViewModels
{
    public class DemoPageViewModel : ViewModelBase
    {
        public DemoPageViewModel()
        {
            
        }

        string _lastCommandText = "none";
        public string LastCommandText
        {
            get => _lastCommandText;
            set 
            {
                _lastCommandText = value;
                NotifyPropertyChanged();
            }
        }

        public void WriteToConsoleCommand(object parameter)
        {
            Console.WriteLine("parameter: " + parameter);

            string val = "{x:Null}";
            if (parameter != null)
                val = parameter.ToString() + ", " + parameter.GetType().FullName;
            
            LastCommandText = val;
        }
    }
}
