﻿using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class ToolStrip : ItemsControl, IItemsPresenterHost
    {
        //private ObservableCollection<IToolStripItem> _availableItems = new ObservableCollection<IToolStripItem>();

        /*static ObservableCollection<IToolStripItem> ItemsDefaultCollection => new ObservableCollection<IToolStripItem>();
        public static readonly StyledProperty<ObservableCollection<IToolStripItem>> ItemsProperty =
            AvaloniaProperty.Register<ToolStrip, ObservableCollection<IToolStripItem>>(nameof(Items), ItemsDefaultCollection);

        [Content]
        public ObservableCollection<IToolStripItem> Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }*/

        /*public static readonly DirectProperty<ToolStrip, ObservableCollection<IToolStripItem>> AvailableItemsProperty =
            AvaloniaProperty.RegisterDirect<ToolStrip, ObservableCollection<IToolStripItem>>(nameof(AvailableItems), o => o.AvailableItems, (o, v) => o.AvailableItems = v);

        [Content]
        public ObservableCollection<IToolStripItem> AvailableItems
        {
            get { return _availableItems; }
            set { SetAndRaise(AvailableItemsProperty, ref _availableItems, value); }
        }*/

        //private ObservableCollection<ToolStripItemReference> DefaultItems = new ObservableCollection<ToolStripItemReference>();
        ObservableCollection<ToolStripItemReference> _defaultItems = new ObservableCollection<ToolStripItemReference>();
        public static readonly DirectProperty<ToolStrip, ObservableCollection<ToolStripItemReference>> DefaultItemsProperty =
            AvaloniaProperty.RegisterDirect<ToolStrip, ObservableCollection<ToolStripItemReference>>(nameof(DefaultItems), o => o.DefaultItems, (o, v) => o.DefaultItems = v);

        public ObservableCollection<ToolStripItemReference> DefaultItems
        {
            get => _defaultItems;
            set => SetAndRaise(DefaultItemsProperty, ref _defaultItems, value);
        }

        //private ObservableCollection<ToolStripItemReference> CurrentItems = new ObservableCollection<ToolStripItemReference>();
        ObservableCollection<ToolStripItemReference> _currentItems = new ObservableCollection<ToolStripItemReference>();
        public static readonly DirectProperty<ToolStrip, ObservableCollection<ToolStripItemReference>> CurrentItemsProperty =
            AvaloniaProperty.RegisterDirect<ToolStrip, ObservableCollection<ToolStripItemReference>>(nameof(CurrentItems), o => o.CurrentItems, (o, v) => o.CurrentItems = v);

        public ObservableCollection<ToolStripItemReference> CurrentItems
        {
            get => _currentItems;
            set => SetAndRaise(CurrentItemsProperty, ref _currentItems, value);
        }

        ObservableCollection<ToolStripItemReference> _availableItems = new ObservableCollection<ToolStripItemReference>();
        public static readonly DirectProperty<ToolStrip, ObservableCollection<ToolStripItemReference>> AvailableItemsProperty =
            AvaloniaProperty.RegisterDirect<ToolStrip, ObservableCollection<ToolStripItemReference>>(nameof(AvailableItems), o => o.AvailableItems, (o, v) => o.AvailableItems = v);

        public ObservableCollection<ToolStripItemReference> AvailableItems
        {
            get => _availableItems;
            set => SetAndRaise(AvailableItemsProperty, ref _availableItems, value);
        }

        public static readonly StyledProperty<bool> IsCustomizingProperty =
            AvaloniaProperty.Register<ToolStrip, bool>(nameof(IsCustomizing), false);

        public bool IsCustomizing
        {
            get => GetValue(IsCustomizingProperty);
            set => SetValue(IsCustomizingProperty, value);
        }

        public static readonly StyledProperty<bool> ShowLabelsProperty =
            AvaloniaProperty.Register<ToolStrip, bool>(nameof(ShowLabels), false);

        public bool ShowLabels
        {
            get => GetValue(ShowLabelsProperty);
            set => SetValue(ShowLabelsProperty, value);
        }


        static ToolStrip()
        {
            /*AvailableItemsProperty.Changed.AddClassHandler<ToolStrip>(new Action<ToolStrip, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                sender.UpdateItems();
            }));*/
            ItemsProperty.Changed.AddClassHandler<ToolStrip>(new Action<ToolStrip, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                if (e.OldValue != null)
                    ((AvaloniaList<object>)e.OldValue).CollectionChanged -= sender.Items_CollectionChanged;

                if (e.NewValue != null)
                    ((AvaloniaList<object>)e.NewValue).CollectionChanged += sender.Items_CollectionChanged;
            }));

            CurrentItemsProperty.Changed.AddClassHandler<ToolStrip>(new Action<ToolStrip, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                if (e.OldValue != null)
                    ((ObservableCollection<ToolStripItemReference>)e.OldValue).CollectionChanged -= sender.CurrentItems_CollectionChanged;

                if (e.NewValue != null)
                    ((ObservableCollection<ToolStripItemReference>)e.NewValue).CollectionChanged += sender.CurrentItems_CollectionChanged;
            }));
        }

        bool ShouldBeInAvailableItems(IToolStripItem item)
        {
            return CurrentItems.Where(x => x.TargetItem == item).Count() == 0;
        }

        public ToolStrip()
        {
            /*((AvaloniaList<object>)Items).CollectionChanged += (sender, e) =>
            {
                
            };*/
            /*AvailableItems.CollectionChanged += (sender, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (IToolStripItem item in e.NewItems.OfType<IToolStripItem>().Where(x => x.IsDefault))
                        DefaultItems.Add(item.ToReference());
                }

                //UpdateItems();
            };*/
            ((AvaloniaList<object>)Items).CollectionChanged += Items_CollectionChanged;
            CurrentItems.CollectionChanged += CurrentItems_CollectionChanged;
            //DefaultItems.CollectionChanged += DefaultItems_CollectionChanged;
            /*CurrentItems.CollectionChanged += CurrentItems_CollectionChanged;
            CurrentItems.CollectionChanged += (sneder, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (ToolStripItemReference rfrnc in args.NewItems)
                    {
                        var item = rfrnc.TargetItem;
                        if (!item.AllowDuplicates)
                        {
                            var matches = AvailableItems.Where(x => x.TargetItem == item).ToList();
                            if (matches.Count() > 0)
                            {
                                foreach (ToolStripItemReference reference in matches)
                                    AvailableItems.Remove(reference);
                            }
                        }
                    }
                }

                if (args.OldItems != null)
                {
                    foreach (ToolStripItemReference rfrnc in args.OldItems)
                    {
                        AvailableItems.Add(rfrnc);
                    }
                }
            };*/
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (CurrentItems.Count == 0)
                ResetToDefaults();
        }

        private void DefaultItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }

        /*void UpdateDefaultItems()
        {
            var spaceItem = Items.OfType<ToolStripItemReference>().FirstOrDefault<ToolStripFlexibleSpaceReference>();
            foreach (ToolStripItemReference reference in DefaultItems)
            {
                if (reference is ToolStripFlexibleSpaceReference)
                {
                    reference.TargetItem = spaceItem;//[!ToolStripFlexibleSpaceReference.TargetItemProperty] = 
                }
            }
        }*/

        private void CurrentItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                AvailableItems.Clear();
                foreach (IToolStripItem itm in Items.OfType<IToolStripItem>().Where(x => !CurrentItems.Any(w => w.TargetItem == x)))
                    AvailableItems.Add(itm.ToReference());
            }
            else if (e.OldItems != null)
            {
                Debug.WriteLine("e.OldItems.Count: " + e.OldItems.Count);
                foreach (ToolStripItemReference rfrnc in e.OldItems)
                {
                    Debug.WriteLine("DisplayName: " + rfrnc.TargetItem.DisplayName);
                    //if (!rfrnc.TargetItem.AllowDuplicates)
                    if (!AvailableItems.Any(x => x.TargetItem.Equals(rfrnc.TargetItem) || (x.TargetItem == rfrnc.TargetItem)))
                        AvailableItems.Add(rfrnc);
                    //else if (AvailableItems.FirstOrDefault(x => x.TargetItem == rfrnc.TargetItem).TargetItem.AllowDuplicates)
                }
            }
            else
                Debug.WriteLine("e.OldItems == null");

            if (e.NewItems != null)
            {
                foreach (ToolStripItemReference rfrnc in e.NewItems)
                {
                    var item = rfrnc.TargetItem;
                    if (!item.AllowDuplicates)
                    {
                        var matches = AvailableItems.Where(x => x.TargetItem == item).ToList();
                        if (matches.Count() > 0)
                        {
                            foreach (ToolStripItemReference reference in matches.Where(x => AvailableItems.Contains(x)))
                                AvailableItems.Remove(reference);
                        }
                    }
                }
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (IToolStripItem item in e.NewItems)
                {
                    item.Owner = this;
                    if (ShouldBeInAvailableItems(item))
                        AvailableItems.Add(item.ToReference());
                }
            }

            if (e.OldItems != null)
            {
                foreach (IToolStripItem item in e.OldItems)
                {
                    item.Owner = null;
                    var reference = AvailableItems.FirstOrDefault(x => x.TargetItem == item);
                    if (AvailableItems.Contains(reference))
                        AvailableItems.Remove(reference);
                }
            }

            var flexibleSpaces = Items.OfType<FlexibleSpaceToolStripItem>().ToList();
            if (flexibleSpaces.Count() > 1)
                ((AvaloniaList<object>)Items).RemoveAll(flexibleSpaces); //.Insert(0, new FlexibleSpaceToolStripItem());

            if (flexibleSpaces.Count() == 0)
                ((AvaloniaList<object>)Items).Insert(0, ToolStripFlexibleSpaceReference.ItemInstance);
        }

        //public List<IToolStripItem> HoverItems = new List<IToolStripItem>();
        public ToolStripItemReference HoverItem = null;
        internal void ValidateAddToToolStrip(IToolStripItem item, Visual sender, Vector cursor)
        {
            var popupRoot = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(sender);
            
            var point = VisualRoot.PointToClient(sender.PointToScreen(new Point(cursor.X, cursor.Y)));
            //Console.WriteLine("point: " + point);

            var hitTestItems = VisualRoot.Renderer.HitTest(point, VisualRoot, null).OfType<Visual>();
            //Console.WriteLine("hitTestItems count: " + hitTestItems.Count());
            Point tL = _currentItemsItemsControl.Bounds.TopLeft;
            Point bR = _currentItemsItemsControl.Bounds.BottomRight;
            if ((point.X > tL.X) && (point.Y > tL.Y) && (point.X < bR.X) && (point.Y < bR.Y))  //  /*hitTestItems.Contains(_currentItemsItemsControl)*/)
            {
                //Console.WriteLine("hitTestItems.Contains(_currentItemsItemsControl) == true");
                var hoverItem = Avalonia.VisualTree.VisualExtensions.GetVisualDescendants(_currentItemsItemsControl).OfType<Visual>().FirstOrDefault(x => hitTestItems.Contains(x) && _currentItemsItemsControl.Items.OfType<object>().Contains(x.DataContext)).DataContext;
                //(point.X > tL.X) && (point.Y > tL.Y) && (point.X < bR.X) && (point.Y < bR.Y)


                /*var hoverItem = hitTestItems.FirstOrDefault(x => _currentItemsItemsControl.Items.OfType<object>().Contains(x));*/
                if (hoverItem != null)
                    CurrentItems.Insert(_currentItemsItemsControl.Items.OfType<object>().ToList().IndexOf(hoverItem), item.ToReference()); /*(CurrentItems.Last(x => x.TargetItem == HoverItems.Last()))*, item.ToReference());*/
                else
                    CurrentItems.Add(item.ToReference());
            }
                //_currentItemsItemsControl.ItemContainerGenerator.
                //Debug.WriteLine("Added: " + _currentItemsItemsControl.IsPointerOver);
            //});
        }
        internal void ValidateMoveOrRemoveFromToolStrip(ToolStripItemReference item, Visual sender, Vector cursor)
        {
            Console.WriteLine("cursor: " + cursor);
            var point = new Point(cursor.X, cursor.Y)/*.WithOffset(sender.Bounds.X, sender.Bounds.Y)*/.WithOffset(_currentItemsItemsControl.Bounds.X, _currentItemsItemsControl.Bounds.Y); // sender.Bounds.TopLeft.WithOffset(cursor.X, cursor.Y);
            Console.WriteLine("point: " + point);
            //var match = CurrentItems.FirstOrDefault(x => x.TargetItem == item);
            if (CurrentItems.Contains(item))
                CurrentItems.Remove(item);
            /*foreach (ToolStripItemReference reference in matches)
            {
                CurrentItems.Remove(reference);
                break;
            }*/
            var hitTestItems = VisualRoot.Renderer.HitTest(point, VisualRoot, null);
            
            Point tL = _currentItemsItemsControl.Bounds.TopLeft;
            Point bR = _currentItemsItemsControl.Bounds.BottomRight;
            if ((point.X > tL.X) && (point.Y > tL.Y) && (point.X < bR.X) && (point.Y < bR.Y)) //(VisualRoot.Renderer.HitTest(VisualRoot.TranslatePoint(new Point(cursor.X, cursor.Y), sender).Value, this, null).Contains(_currentItemsItemsControl))
            {
                var hoverItem = Avalonia.VisualTree.VisualExtensions.GetVisualDescendants(_currentItemsItemsControl).OfType<Visual>().FirstOrDefault(x => hitTestItems.Contains(x) && CurrentItems.Contains(x.DataContext));
                if (hoverItem != null)
                {
                    Console.WriteLine("hoverItem != null");
                    if (hoverItem.DataContext is ToolStripItemReference tItem)
                    {
                        Console.WriteLine("hoverItem.DataContext is IToolStripItem tItem");
                        CurrentItems.Insert(CurrentItems.IndexOf(tItem), item);
                    }
                    else
                        Console.WriteLine(hoverItem.DataContext.GetType().FullName);
                }
                //CurrentItems.Remove(hoverItem);
                /*int hoverItemIndex = -1;
                if (HoverItem != null)
                    hoverItemIndex = CurrentItems.IndexOf(HoverItem);

                if ((hoverItemIndex >= 0) && (hoverItemIndex < CurrentItems.Count))
                    CurrentItems.Insert(hoverItemIndex, item);
                else
                    CurrentItems.Add(item);*/
            }
            //Debug.WriteLine("Added: " + _currentItemsItemsControl.IsPointerOver);
            //});
        }

        /*void WaitForPointer(Action action)
        {
            Timer timer = new Timer(100);
            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.UIThread.Post(action);
                timer.Stop();
            };
            timer.Start();
        }*/

        public void ResetToDefaults()
        {
            /*for (int i = 0; i < CurrentItems.Count; i++)
                CurrentItems.Remove(CurrentItems[0]);*/
            Debug.WriteLine("CLEARING");
                CurrentItems.Clear();
            Debug.WriteLine("CLEARED");

            //CurrentItems.Clear();
            foreach (ToolStripItemReference reference in DefaultItems)
                CurrentItems.Add(reference);
        }

        void zUpdateItems()
        {
            DefaultItems.Clear();
            Debug.WriteLine("AvailableItems.Count: " + Items.OfType<IToolStripItem>().Count());
            foreach (IToolStripItem item in Items/*.OfType<IToolStripItem>().Where(x => x.IsDefault)*/)
                DefaultItems.Add(item.ToReference());

            Debug.WriteLine("DefaultItems.Count: " + DefaultItems.Count/*.OfType<ToolStripItemReference>().Count()*/);
            /*if (_currentItemsList.Count() == 0)
                CurrentItems = DefaultItems;*/
        }

        /*void AddToDefault(ToolStripItemReference reference)
        {
            ((ObservableCollection<ToolStripItemReference>)DefaultItems).Add(reference);
        }

        void AddToCurrent(ToolStripItemReference reference)
        {
            ((ObservableCollection<ToolStripItemReference>)CurrentItems).Add(reference);
        }*/

        ItemsControl _currentItemsItemsControl = null;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            //e.NameScope.Get<MenuItem>("PART_CustomizeMenuItem").Click += (sneder, args) => IsCustomizing = true;
            _currentItemsItemsControl = e.NameScope.Get<ItemsControl>("PART_CurrentItemsItemsControl");
            /*_currentItemsItemsControl.PointerMoved += (sneder, args) =>
            {
                var pnt = args.GetCurrentPoint(_currentItemsItemsControl);
                Debug.WriteLine("Point: " + pnt.Position);
            };*/
            Thumb defaultItemsDragThumb = e.NameScope.Get<Thumb>("PART_DefaultItemsThumb");
            defaultItemsDragThumb.DragCompleted += (sneder, args) =>
            {
                if (VisualRoot.Renderer.HitTest(VisualRoot.TranslatePoint(new Point(args.Vector.X, args.Vector.Y), defaultItemsDragThumb).Value, this, null).Contains(_currentItemsItemsControl))//_currentItemsItemsControl.IsPointerOver)
                    ResetToDefaults();
            };
        }

        public IItemsPresenter Presenter
        {
            get;
            protected set;
        }

        void IItemsPresenterHost.RegisterItemsPresenter(IItemsPresenter presenter)
        {
            Presenter = presenter;
        }
    }
}
