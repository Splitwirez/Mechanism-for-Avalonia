using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Avalonia.Controls;
using Mechanism.AvaloniaUI.Controls;
using Mechanism.AvaloniaUI.Sample.Views;

namespace Mechanism.AvaloniaUI.Sample.ViewModels
{
    public class CommandBarPageViewModel : DemoPageViewModel
    {
        public CommandBarPageViewModel()
        {
            Title = "CommandBar";
        }

        bool _itemsAlignmentIsRight = false;
        public bool ItemsAlignmentIsRight
        {
            get => _itemsAlignmentIsRight;
            set 
            {
                _itemsAlignmentIsRight = value;
                NotifyPropertyChanged();
                if (_itemsAlignmentIsRight)
                    ItemsAlignment = ChildrenHorizontalAlignment.Right;
                else
                    ItemsAlignment = ChildrenHorizontalAlignment.Left;
            }
        }

        ChildrenHorizontalAlignment _itemsAlignment = ChildrenHorizontalAlignment.Left;
        public ChildrenHorizontalAlignment ItemsAlignment
        {
            get => _itemsAlignment;
            set 
            {
                _itemsAlignment = value;
                NotifyPropertyChanged();
            }
        }


        ObservableCollection<CommandBarItemItemViewModel> _items = new ObservableCollection<CommandBarItemItemViewModel>()
        {
            new CommandBarItemItemViewModel("Lorem", true, true),
            new CommandBarItemItemViewModel("Ipsum", true, false),
            new CommandBarItemItemViewModel("Dolor", false, true),
            new CommandBarItemItemViewModel("Sit", false, false),
            new CommandBarItemItemViewModel("Amet", true, true),
            new CommandBarItemItemViewModel("One", true, false),
            new CommandBarItemItemViewModel("Two", false, true),
            new CommandBarItemItemViewModel("Three", false, false),
            new CommandBarItemItemViewModel("Four", true, true),
            new CommandBarItemItemViewModel("Five", true, false)
        };

        public ObservableCollection<CommandBarItemItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                NotifyPropertyChanged();
            }
        }
    }

    public class CommandBarItemItemViewModel : ViewModelBase
    {
        public CommandBarItemItemViewModel(string title, bool layerA, bool layerB) : base()
        {
            Title = title;
            Layers["LayerA"] = layerA;
            Layers["LayerB"] = layerB;
        }
        
        Dictionary<string, bool> _layers = new Dictionary<string, bool>();
        public Dictionary<string, bool> Layers
        {
            get => _layers;
            set
            {
                _layers = value;
                NotifyPropertyChanged();
            }
        }
    }
}
