﻿using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mechanism.AvaloniaUI.Sample.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;  

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")  
        {  
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
