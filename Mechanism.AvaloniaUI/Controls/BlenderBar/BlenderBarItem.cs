using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Avalonia.Layout;
using Avalonia.Controls.Templates;
using System.Collections;
using Avalonia.Collections;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using System.Diagnostics;
using Avalonia.Data;
using Mechanism.AvaloniaUI.Core;
using Avalonia.Threading;
using Avalonia.Utilities;
using Avalonia.Styling;
using System.Globalization;
using Avalonia.Input;
using System.Timers;

namespace Mechanism.AvaloniaUI.Controls.BlenderBar
{
    public class BlenderBarItem : TreeViewItem, IStyleable
    {
        public static readonly StyledProperty<int> GroupIndexProperty =
        AvaloniaProperty.Register<BlenderBar, int>(nameof(GroupIndex), 0);

        public int GroupIndex
        {
            get => GetValue(GroupIndexProperty);
            set => SetValue(GroupIndexProperty, value);
        }

        public static readonly StyledProperty<bool> IsPressedProperty =
        Button.IsPressedProperty.AddOwner<BlenderBarItem>();

        public bool IsPressed
        {
            get => GetValue(IsPressedProperty);
            private set => SetValue(IsPressedProperty, value);
        }



        public BlenderBarItem()
        {
            UpdatePseudoClasses(IsPressed);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                IsPressed = true;
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (IsPressed && e.InitialPressMouseButton == MouseButton.Left)
                IsPressed = false;
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsPressedProperty)
                UpdatePseudoClasses(change.NewValue.GetValueOrDefault<bool>());
        }

        Popup _itemsPopup = null;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _itemsPopup = e.NameScope.Find<Popup>("PART_ItemsPopup");

            if (
                //(_selectionBorder != null) && 
                (_itemsPopup != null)
               )
            {
                PointerPressed += (sneder, args) =>
                {
                    Timer timer = new Timer(500);
                    timer.Elapsed += (snader, ergs) =>
                    {
                        timer.Stop();
                        Dispatcher.UIThread.Post(() =>
                        {
                            if (IsPressed)
                            {
                                IsExpanded = true;
                                e.Handled = true;
                            }
                        });
                    };
                    timer.Start();
                };
            }
        }

        private void UpdatePseudoClasses(bool isPressed)
        {
            PseudoClasses.Set(":pressed", isPressed);
        }
        Type IStyleable.StyleKey => typeof(BlenderBarItem);
    }
}