using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Avalonia.Controls;
using Mechanism.AvaloniaUI.Sample.Views;

namespace Mechanism.AvaloniaUI.Sample.ViewModels
{
    public class ToolBarTrayPageViewModel : DemoPageViewModel
    {
        public ToolBarTrayPageViewModel()
        {
            Title = "ToolBarTray and ToolBar";
        }


        ObservableCollection<ObservableCollection<CommandBarItemItemViewModel>> _items = new ObservableCollection<ObservableCollection<CommandBarItemItemViewModel>>()
        {
            new ObservableCollection<CommandBarItemItemViewModel>()
            {
                new CommandBarItemItemViewModel("Lorem", true, true),
                new CommandBarItemItemViewModel("Ipsum", true, false),
                new CommandBarItemItemViewModel("Dolor", false, true),
                new CommandBarItemItemViewModel("Sit", false, false),
                new CommandBarItemItemViewModel("Amet", true, true)
            },
            new ObservableCollection<CommandBarItemItemViewModel>()
            {
                new CommandBarItemItemViewModel("One", true, false),
                new CommandBarItemItemViewModel("Two", false, true),
                new CommandBarItemItemViewModel("Three", false, false),
                new CommandBarItemItemViewModel("Four", true, true),
                new CommandBarItemItemViewModel("Five", true, false)
            }
        };

        public ObservableCollection<ObservableCollection<CommandBarItemItemViewModel>> Items
        {
            get => _items;
            set
            {
                _items = value;
                NotifyPropertyChanged();
            }
        }
    }


}
