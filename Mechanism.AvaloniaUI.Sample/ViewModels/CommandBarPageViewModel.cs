using System;
using System.Collections.Generic;
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
    }
}
