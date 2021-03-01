using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Avalonia.Controls;
using Mechanism.AvaloniaUI.Sample.Views;

namespace Mechanism.AvaloniaUI.Sample.ViewModels
{
    public class TestViewModel : DemoPageViewModel
    {
        public TestViewModel()
        {
            Title = "TEST";
        }

        public void AddItemCommand(object parameter)
        {
            Items.Add(new TestItemViewModel("Yes"));
        }


        ObservableCollection<TestItemViewModel> _items = new ObservableCollection<TestItemViewModel>()
        {
            new TestItemViewModel("Lorem"),
            new TestItemViewModel("Ipsum"),
            new TestItemViewModel("Dolor"),
            new TestItemViewModel("Sit"),
            new TestItemViewModel("Amet"),
            new TestItemViewModel("One"),
            new TestItemViewModel("Two"),
            new TestItemViewModel("Three"),
            new TestItemViewModel("Four"),
            new TestItemViewModel("Five")
        };

        public ObservableCollection<TestItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                NotifyPropertyChanged();
            }
        }
    }

    public class TestItemViewModel : ViewModelBase
    {
        public TestItemViewModel(string title) : base()
        {
            Title = title;
        }
    }
}
