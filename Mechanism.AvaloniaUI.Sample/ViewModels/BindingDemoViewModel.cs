using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mechanism.AvaloniaUI.Sample.ViewModels
{
    public class BindingDemoViewModel : INotifyPropertyChanged
    {
        ObservableCollection<ToolCommandItem> _commandItems = new ObservableCollection<ToolCommandItem>()
        {
            new ToolCommandItem()
            {
                DisplayName = "Cut"
            },
            new ToolCommandItem()
            {
                DisplayName = "Copy"
            },
            new ToolCommandItem()
            {
                DisplayName = "Paste"
            }
        };
        public ObservableCollection<ToolCommandItem> CommandItems
        {
            get => _commandItems;
            set
            {
                _commandItems = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
