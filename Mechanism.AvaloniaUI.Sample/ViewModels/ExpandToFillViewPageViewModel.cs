using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Avalonia.Controls;
using Avalonia.Media;
using Mechanism.AvaloniaUI.Sample.Views;

namespace Mechanism.AvaloniaUI.Sample.ViewModels
{
    public class ExpandToFillViewPageViewModel : DemoPageViewModel
    {
        public ExpandToFillViewPageViewModel()
        {
            Title = "ExpandToFillView";
        }


        ObservableCollection<ExpandToFillViewItemItemViewModel> _items = new ObservableCollection<ExpandToFillViewItemItemViewModel>()
        {
            new ExpandToFillViewItemItemViewModel("Chassis & Cockpits", "RedEllipse", new SolidColorBrush(Colors.DarkRed)),
            new ExpandToFillViewItemItemViewModel("Wings, Wheels, Sails", "GreenEllipse", new SolidColorBrush(Colors.DarkGreen)),
            new ExpandToFillViewItemItemViewModel("Details", "BlueEllipse", new SolidColorBrush(Colors.DarkBlue))
        };

        public ObservableCollection<ExpandToFillViewItemItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                NotifyPropertyChanged();
            }
        }
    }

    public class ExpandToFillViewItemItemViewModel : ViewModelBase
    {
        public ExpandToFillViewItemItemViewModel(string title, string iconKey, IBrush brush) : base()
        {
            Title = title;
            IconKey = iconKey;
            Brush = brush;
        }

        string _iconKey = string.Empty;
        public string IconKey
        {
            get => _iconKey;
            set
            {
                _iconKey = value;
                NotifyPropertyChanged();
            }
        }
        
        IBrush _brush = null;
        public IBrush Brush
        {
            get => _brush;
            set
            {
                _brush = value;
                NotifyPropertyChanged();
            }
        }
    }
}
