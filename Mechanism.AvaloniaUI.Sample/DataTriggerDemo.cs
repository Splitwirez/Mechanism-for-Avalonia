using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mechanism.AvaloniaUI.Sample
{
    public class DataTriggerDemo : INotifyPropertyChanged
    {
        double _numberValue = 1;
        
        public double NumberValue
        {
            get => _numberValue;
            set
            {
                _numberValue = value;
                NotifyPropertyChanged();
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}