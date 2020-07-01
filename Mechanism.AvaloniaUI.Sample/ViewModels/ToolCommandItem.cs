using Avalonia.Controls.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Mechanism.AvaloniaUI.Sample.ViewModels
{
    public class ToolCommandItem : INotifyPropertyChanged
    {
        /*ICommand _command = null;
        public ICommand Command
        {
            get => _command;
            set
            {
                _command = value;
                NotifyPropertyChanged();
            }
        }*/
        public void ItemCommand(object parameter)
        {
            Debug.WriteLine("Command executed for \"" + DisplayName + "\"");
        }

        string _displayName = string.Empty;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                NotifyPropertyChanged();
            }
        }

        IControlTemplate _icon = null;
        public IControlTemplate Icon
        {
            get => _icon;
            set
            {
                _icon = value;
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
