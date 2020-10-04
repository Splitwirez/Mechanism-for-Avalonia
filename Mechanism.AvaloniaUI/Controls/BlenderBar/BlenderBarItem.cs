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
using Avalonia.LogicalTree;

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

        public static readonly StyledProperty<object> LastSelectedItemProperty =
        AvaloniaProperty.Register<BlenderBarItem, object>(nameof(LastSelectedItem), null);

        public object LastSelectedItem
        {
            get => GetValue(LastSelectedItemProperty);
            set => SetValue(LastSelectedItemProperty, value);
        }

        static BlenderBarItem()
        {
            AffectsMeasure<BlenderBarItem>(AnyChildSelectedProperty, LastSelectedItemProperty);
            AffectsArrange<BlenderBarItem>(AnyChildSelectedProperty, LastSelectedItemProperty);
            AffectsRender<BlenderBarItem>(AnyChildSelectedProperty, LastSelectedItemProperty);

            ItemCountProperty.Changed.AddClassHandler<BlenderBarItem>((sender, args) =>
            {
                if (((int)args.NewValue > 0) && (sender.LastSelectedItem == null))
                    sender.LastSelectedItem = sender.Items.Cast<object>().FirstOrDefault(x => x != null);
            });
        }

        public BlenderBarItem()
        {
            UpdatePseudoClasses(IsPressed);
            
            if (ItemCount > 0)
            {
                
            }

            AttachedToLogicalTree += BlenderBarItem_AttachedToLogicalTree;
        }

        void BlenderBarItem_AttachedToLogicalTree(object sender, LogicalTreeAttachmentEventArgs e)
        {
            GetOwnerBlenderBar().SelectionChanged += (sneder, args) =>
            {
                UpdateAnyChildSelected();
                
                if ((LastSelectedItem != null) && (args.AddedItems != null) && args.AddedItems.Contains(this))
                {
                    args.Handled = true;
                    GetOwnerBlenderBar().SelectedItems.Clear();
                    GetOwnerBlenderBar().SelectedItems.Add(LastSelectedItem);
                }
                    

                InvalidateArrange();
                InvalidateMeasure();
                InvalidateStyles();
                InvalidateVisual();
            };
            
            
            AttachedToLogicalTree -= BlenderBarItem_AttachedToLogicalTree;
        }

        void UpdateAnyChildSelected()
        {
            var bar = GetOwnerBlenderBar();
            if (bar != null)
            {
                var selected = bar.SelectedItem; //.SelectedItems.Cast<object>().FirstOrDefault(x => Items.Cast<object>().Contains(x));
                if ((selected != null) && Items.Cast<object>().Contains(selected))
                {
                    LastSelectedItem = selected;
                    /*if (selected is Visual vis)
                        AttachedIcon.SetIconGap(this, AttachedIcon.GetIconGap(vis));*/
                    AnyChildSelected = true;
                    /*bool newVal = Items.Cast<object>().Contains(selected);
                    
                    AnyChildSelected = newVal;
                    
                    if (newVal)
                    {
                        if (newVal)
                            LastSelectedItem = selected;
                        else if (LastSelectedItem == null)
                            LastSelectedItem = Items.Cast<object>().ElementAtOrDefault(0);
                    }
                    else if (LastSelectedItem == null)
                        LastSelectedItem = Items.Cast<object>().ElementAtOrDefault(0);
                }
                else if (LastSelectedItem == null)
                    LastSelectedItem = Items.Cast<object>().ElementAtOrDefault(0);*/
                }
                else
                    AnyChildSelected = false;
            }
        }

        void WaitForPointer(Action action)
        {
            Timer timer = new Timer(100);
            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.UIThread.Post(action);
                timer.Stop();
            };
            timer.Start();
        }

        int _millis = 0;
        ulong _pressedTimestamp = 0;
        DispatcherTimer _timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(150)
        };
        bool _timerTickEventHandlerSet = false;
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                IsPressed = true;
                /*_pressedTimestamp = e.Timestamp;
                Console.WriteLine("_pressedTimestamp: " + _pressedTimestamp);*/

                if (ItemCount > 0)
                {
                    //e.Handled = true;
                    if (!_timerTickEventHandlerSet)
                    {
                        
                        _timer.Tick += (snader, ergs) =>
                        {
                            /*_millis++;
                            Console.WriteLine("_millis: " + _millis);*/
                            //_timer.Stop();
                            if (!_itemsPopup.IsOpen)
                            {
                                var items = VisualRoot.Renderer.HitTest(e.GetPosition(VisualRoot), this, null);
                                /*foreach (object ob in items)
                                    Console.WriteLine("ob type: " + ob.GetType().FullName + ", " + (ob as Visual).TemplatedParent.GetType().FullName);
                                Console.WriteLine("items.Count(): " + items.Count());*/
                                if (items.Contains(this) || items.OfType<Visual>().Any(x => x.TemplatedParent == this))
                                    _itemsPopup.IsOpen = true;
                            }
                            /*else
                            {
                                
                                //Avalonia.VisualExtensions.
                                //var popupRoot = _itemsPopup.GetLogicalAncestors().OfType<TopLevel>().First();
                                var popupItems = VisualRoot.Renderer.HitTest(e.GetCurrentPoint(_itemsPopup).Position, _itemsPopup, null).OfType<Visual>();
                                var items = Items.Cast<object>().ToList();
                                foreach (object o in items)
                                {
                                    BlenderBarItem bbi = null;
                                    if (o is BlenderBarItem bb)
                                        bbi = bb;
                                    else
                                    {
                                        var container = ItemContainerGenerator.ContainerFromIndex(items.IndexOf(o));
                                        if (container is BlenderBarItem bb2)
                                            bbi = bb2;
                                    }

                                    bbi.SetPointerOver(popupItems.Any(x => (x == bbi) || (x.TemplatedParent == bbi)));
                                }
                            }*/
                        };
                        _timerTickEventHandlerSet = true;
                    }
                    _timer.Start();
                }
            }
        }
        
        internal void SetPointerOver(bool newValue)
        {
            //SetValue(IsPointerOverProperty, newValue);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (_timer.IsEnabled)
                _timer.Stop();

            if (IsPressed && (e.InitialPressMouseButton == MouseButton.Left))
                IsPressed = false;
            
            if ((ItemCount > 0) && (_itemsPopup.IsOpen))
            {
                var items = Items.Cast<object>().ToList();
                List<BlenderBarItem> bbItems = new List<BlenderBarItem>();
                foreach (object o in items)
                {
                    BlenderBarItem bbi = null;
                    if (o is BlenderBarItem bb)
                        bbi = bb;
                    else
                    {
                        var container = ItemContainerGenerator.ContainerFromIndex(items.IndexOf(o));
                        if (container is BlenderBarItem bb2)
                            bbi = bb2;
                    }
                    bbItems.Add(bbi);
                }

                
                //Console.WriteLine("popup grandparent: " + (_itemsPopup.Parent as Panel).Parent.GetType().FullName);
                var popupRoot = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(_itemsPopup.Child);

                var point = e.GetCurrentPoint(this).Position;
                //Console.WriteLine("Original point coords: " + point.ToString() + ", " + (popupRoot == VisualRoot).ToString());
                var pxPoint = VisualExtensions.PointToScreen(this, point); //.ToPoint(VisualRoot.RenderScaling);
                point = VisualExtensions.PointToClient(_itemsPopup.Child, pxPoint);

                var hitTestItems = popupRoot.Renderer.HitTest(point, _itemsPopup.Child, null).OfType<Visual>();
                //hitTestItems = hitTestItems.OfType<Visual>(); //VisualRoot.Renderer.HitTest(e.GetCurrentPoint(VisualRoot).Position, _itemsPopup, null).OfType<Visual>();
                //Console.WriteLine("hhitTestItems.Count(): " + hhitTestItems.Count() + "\nPoint coords: " + point.ToString());
                
                var sel = bbItems.FirstOrDefault(x => hitTestItems.Any(w => (w == x) || (w.TemplatedParent == x)));
                if (sel != null)
                {
                    sel.SetPointerOver(false);
                    GetOwnerBlenderBar().SelectedItems.Clear();
                    GetOwnerBlenderBar().SelectedItems.Add(items.ElementAt(bbItems.IndexOf(sel)));
                    UpdateAnyChildSelected();
                }
                _itemsPopup.IsOpen = false;
            }
            /*if ((ItemCount > 0) && (_itemsPopup.IsPointerOver))
            {
                var items = Items.Cast<object>().ToList();
                foreach (object o in items)
                {
                    BlenderBarItem bbi = null;
                    if (o is BlenderBarItem bb)
                        bbi = bb;
                    else
                    {
                        var container = ItemContainerGenerator.ContainerFromIndex(items.IndexOf(o));
                        if (container is BlenderBarItem bb2)
                            bbi = bb2;
                    }
                    Console.WriteLine("_itemsPopup parent type name: " + _itemsPopup.Parent.GetType().FullName);
                    VisualRoot.Renderer.HitTest(e.GetPosition(bbi), _itemsPopup, null);
                    //_itemsPopup.PointerLeave += (sneder, args) => UnsetSubItemEventHandlers();
                }
            }*/
        }

        void zOnPointerPressed(PointerPressedEventArgs e)
        {
            if (ItemCount == 0)
                base.OnPointerPressed(e);

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                IsPressed = true;
                _pressedTimestamp = e.Timestamp;
            }
            
            if (ItemCount > 0)
            {
                //e.Handled = true;
                _millis = 0;

                Timer timer = new Timer(1);
                timer.Elapsed += (snader, ergs) =>
                {
                    Console.WriteLine("e.Timestamp gap: " + (e.Timestamp - _pressedTimestamp).ToString());
                    _pressedTimestamp = e.Timestamp;
                    _millis++;
                    
                    if (_millis > 300)
                    {
                        timer.Stop();
                        Dispatcher.UIThread.Post(() =>
                        {
                            WaitForPointer(() =>
                            {
                                if (IsPointerOver)
                                    _itemsPopup.IsOpen = true;
                            });
                        });
                    }
                };
                timer.Start();
            }
        }

        void zOnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            //Console.WriteLine("e.Timestamp gap: " + (e.Timestamp - _pressedTimestamp).ToString());
            if (IsPressed && (e.InitialPressMouseButton == MouseButton.Left))
                IsPressed = false;

            if (ItemCount > 0)
            {
                e.Handled = true;

                if (_millis > 300)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        if ((IsPointerOver || _itemsPopup.IsPointerOver) && _itemsPopup.IsOpen)
                        {
                            var items = Items.Cast<object>().ToList();
                            foreach (object o in items)
                            {
                                if (o is BlenderBarItem bbi)
                                {
                                    bbi.PointerMoved += SubItem_PointerEnter;
                                }
                                else
                                {
                                    var container = ItemContainerGenerator.ContainerFromIndex(items.IndexOf(o));
                                    if (container is BlenderBarItem bbi2)
                                        bbi2.PointerMoved += SubItem_PointerEnter;
                                }
                                //_itemsPopup.PointerLeave += (sneder, args) => UnsetSubItemEventHandlers();
                            }
                        }
                    });
                }
                else
                {
                    GetOwnerBlenderBar().SelectedItems.Clear();
                    GetOwnerBlenderBar().SelectedItems.Add(LastSelectedItem);
                }
                _millis = 0;
                /*Timer timer = new Timer(100);
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
                timer.Start();*/
            }
            //_itemsPopup.PointerEnter += ItemsPopup_PointerMoved;
        }

        void SubItem_PointerEnter(object sender, PointerEventArgs e)
        {
            try
            {
                var item = Items.Cast<object>().ElementAt(ItemContainerGenerator.IndexFromContainer(sender as BlenderBarItem));
                Console.WriteLine("ITEM SELECTED FROM CONTAINER: " + item.GetType().FullName);
                GetOwnerBlenderBar().SelectedItems.Clear();
                GetOwnerBlenderBar().SelectedItems.Add(item);
                UpdateAnyChildSelected();
            }
            catch
            {
                Console.WriteLine("ITEM SELECTED BUT IT WAS THE CONTAINER");
                GetOwnerBlenderBar().SelectedItems.Clear();
                GetOwnerBlenderBar().SelectedItems.Add(sender);
                UpdateAnyChildSelected();
            }
            UnsetSubItemEventHandlers();
        }

        void UnsetSubItemEventHandlers()
        {
            var items = Items.Cast<object>().ToList();
            foreach (object o in items)
            {
                if (o is BlenderBarItem bbi)
                {
                    bbi.PointerMoved -= SubItem_PointerEnter;
                }
                else
                {
                    var container = ItemContainerGenerator.ContainerFromIndex(items.IndexOf(o));
                    if (container is BlenderBarItem bbi2)
                        bbi2.PointerMoved -= SubItem_PointerEnter;
                }
            }
            _itemsPopup.IsOpen = false;
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
            /*else if (change.Property == IsSelectedProperty)
            {
                if (change.NewValue.GetValueOrDefault<bool>())
                {
                    GetOwnerBlenderBar().SelectedItems.Clear();
                    GetOwnerBlenderBar().SelectedItems.Add(LastSelectedItem);   
                }
            }*/

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