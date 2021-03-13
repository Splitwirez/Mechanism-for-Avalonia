using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Avalonia.Controls;
using Avalonia.Layout;
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

        
        Dock _xamlDock = Dock.Left;
        public Dock XamlDock
        {
            get => _xamlDock;
            set
            {
                _xamlDock = value;
                NotifyPropertyChanged();
            }
        }

        Dock _boundDock = Dock.Right;
        public Dock BoundDock
        {
            get => _boundDock;
            set
            {
                _boundDock = value;
                NotifyPropertyChanged();
            }
        }

        Orientation _orientation = Orientation.Vertical;
        public Orientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                NotifyPropertyChanged();
            }
        }


        public void ToggleOrientation(object parameter)
        {
            if (Orientation == Orientation.Vertical)
            {
                Orientation = Orientation.Horizontal;
                XamlDock = Dock.Top;
                BoundDock = Dock.Bottom;
            }
            else
            {
                Orientation = Orientation.Vertical;
                XamlDock = Dock.Left;
                BoundDock = Dock.Right;
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
