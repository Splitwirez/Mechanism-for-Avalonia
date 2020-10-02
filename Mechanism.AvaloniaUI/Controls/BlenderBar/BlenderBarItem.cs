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

        public static readonly StyledProperty<bool> AnyChildSelectedProperty =
        AvaloniaProperty.Register<BlenderBarItem, bool>(nameof(AnyChildSelected), false);

        public bool AnyChildSelected
        {
            get => GetValue(AnyChildSelectedProperty);
            private set => SetValue(AnyChildSelectedProperty, value);
        }


        public BlenderBarItem()
        {
            UpdatePseudoClasses(IsPressed);
            UpdateAnyChildSelected();
        }

        object _lastSelected = null;
        void UpdateAnyChildSelected()
        {
            var bar = GetOwnerBlenderBar();
            if ((bar != null))
            {
                var selected = bar.SelectedItems.Cast<object>().FirstOrDefault(x => Items.Cast<object>().Contains(x));
                AnyChildSelected = selected != null;

                if (AnyChildSelected)
                    _lastSelected = selected;    
            }
        }

        bool _handle = true;
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (ItemCount == 0)
                base.OnPointerPressed(e);

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                IsPressed = true;
            
            if (ItemCount > 0)
            {
                e.Handled = true;

                Timer timer = new Timer(300);
                timer.Elapsed += (snader, ergs) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        if (IsPressed)
                            _itemsPopup.IsOpen = true;
                    });
                };
                timer.Start();
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (IsPressed && (e.InitialPressMouseButton == MouseButton.Left))
                IsPressed = false;

            if (ItemCount > 0)
            {
                e.Handled = true;
                Timer timer = new Timer(100);
                timer.Elapsed += (snader, ergs) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        var items = Items.Cast<object>().ToList();
                        foreach (object obj in items)
                        {
                            if ((obj is BlenderBarItem bbi) && bbi.IsPointerOver)
                            {
                                GetOwnerBlenderBar().SelectedItems.Clear();
                                GetOwnerBlenderBar().SelectedItems.Add(bbi);
                                Console.WriteLine("bbi selected");
                                break;
                            }
                            else
                            {
                                var container = ItemContainerGenerator.ContainerFromIndex(items.IndexOf(obj));
                                if ((container is BlenderBarItem bbi2) && bbi2.IsPointerOver)
                                {
                                    GetOwnerBlenderBar().SelectedItems.Clear();
                                    GetOwnerBlenderBar().SelectedItems.Add(bbi2);
                                    Console.WriteLine("bbi2 selected");
                                    break;
                                }
                            }
                        }
                        if (_itemsPopup.IsOpen)
                            _itemsPopup.IsOpen = false;
                    });
                    timer.Stop();
                };
                timer.Start();
            }
                //_itemsPopup.PointerEnter += ItemsPopup_PointerMoved;
        }

        void ItemsPopup_PointerMoved(object sender, PointerEventArgs e)
        {
            _itemsPopup.PointerMoved -= ItemsPopup_PointerMoved;
            /*Timer timer = new Timer(100);
            timer.Elapsed += (snader, ergs) =>
            {
                Dispatcher.UIThread.Post(() =>
                {*/
                    if (ItemCount > 0)
                    {
                        var items = Items.Cast<object>().ToList();
                        foreach (object obj in items)
                        {
                            if ((obj is BlenderBarItem bbi) && bbi.IsPointerOver)
                            {
                                GetOwnerBlenderBar().SelectedItems.Clear();
                                GetOwnerBlenderBar().SelectedItems.Add(bbi);
                                Console.WriteLine("bbi selected");
                                break;
                            }
                            else
                            {
                                var container = ItemContainerGenerator.ContainerFromIndex(items.IndexOf(obj));
                                if ((container is BlenderBarItem bbi2) && bbi2.IsPointerOver)
                                {
                                    GetOwnerBlenderBar().SelectedItems.Clear();
                                    GetOwnerBlenderBar().SelectedItems.Add(bbi2);
                                    Console.WriteLine("bbi2 selected");
                                    break;
                                }
                            }
                        }
                    }
                    if (_itemsPopup.IsOpen)
                        _itemsPopup.IsOpen = false;
                /*});
                timer.Stop();
            };
            timer.Start();*/
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsPressedProperty)
                UpdatePseudoClasses(change.NewValue.GetValueOrDefault<bool>());

            UpdateAnyChildSelected();
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
                _itemsPopup.PointerLeave += (sneder, args) => _itemsPopup.IsOpen = false;
            }
        }

        BlenderBar GetOwnerBlenderBar()
        {
            IStyledElement prnt = this;
            while ((prnt != null) && (!(prnt is BlenderBar)))
            {
                prnt = prnt.Parent;
            }
            if (prnt is BlenderBar bar)
                return bar;
            else
                return null;
        }

        private void UpdatePseudoClasses(bool isPressed)
        {
            PseudoClasses.Set(":pressed", isPressed);
        }
        Type IStyleable.StyleKey => typeof(BlenderBarItem);
    }
}