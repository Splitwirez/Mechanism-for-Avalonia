using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
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
            AvaloniaProperty.RegisterDirect<ToolStrip, ObservableCollection<ToolStripItemReference>>(nameof(DefaultItems), o => o.CurrentItems, (o, v) => o.CurrentItems = v);

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
        }

        bool ShouldBeInAvailableItems(IToolStripItem item)
        {
            return CurrentItems.Where(x => x.TargetItem == item).Count() == 0;
        }

        public ToolStrip()
        {
            ((AvaloniaList<object>)Items).CollectionChanged += (sender, e) =>
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
            };
            /*AvailableItems.CollectionChanged += (sender, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (IToolStripItem item in e.NewItems.OfType<IToolStripItem>().Where(x => x.IsDefault))
                        DefaultItems.Add(item.ToReference());
                }

                //UpdateItems();
            };*/

            if (CurrentItems.Count == 0)
                CurrentItems = DefaultItems;
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
            };
        }

        //public List<IToolStripItem> HoverItems = new List<IToolStripItem>();
        public ToolStripItemReference HoverItem = null;
        public void ValidateAddToToolStrip(IToolStripItem item)
        {
            Timer timer = new Timer(100);
            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (_currentItemsItemsControl.IsPointerOver)
                    {
                        if (HoverItem != null)
                            CurrentItems.Insert(CurrentItems.IndexOf(HoverItem)/*(CurrentItems.Last(x => x.TargetItem == HoverItems.Last()))*/, item.ToReference());
                        else
                            CurrentItems.Add(item.ToReference());
                    }
                    //_currentItemsItemsControl.ItemContainerGenerator.
                    Debug.WriteLine("Added: " + _currentItemsItemsControl.IsPointerOver);
                });
                timer.Stop();
            };
            timer.Start();
        }
        public void ValidateMoveOrRemoveFromToolStrip(IToolStripItem item)
        {
            Timer timer = new Timer(100);
            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    var matches = CurrentItems.Where(x => x.TargetItem == item).ToList();
                    foreach (ToolStripItemReference reference in matches)
                        CurrentItems.Remove(reference);

                    if (_currentItemsItemsControl.IsPointerOver)
                    {
                        if (HoverItem != null)
                            CurrentItems.Insert(CurrentItems.IndexOf(HoverItem), item.ToReference());
                        else
                            CurrentItems.Add(item.ToReference());
                    }
                    Debug.WriteLine("Added: " + _currentItemsItemsControl.IsPointerOver);
                });
                timer.Stop();
            };
            timer.Start();
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

        void AddToDefault(ToolStripItemReference reference)
        {
            ((ObservableCollection<ToolStripItemReference>)DefaultItems).Add(reference);
        }

        void AddToCurrent(ToolStripItemReference reference)
        {
            ((ObservableCollection<ToolStripItemReference>)CurrentItems).Add(reference);
        }

        ItemsControl _currentItemsItemsControl = null;
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            //e.NameScope.Get<MenuItem>("PART_CustomizeMenuItem").Click += (sneder, args) => IsCustomizing = true;
            _currentItemsItemsControl = e.NameScope.Get<ItemsControl>("PART_CurrentItemsItemsControl");
            /*_currentItemsItemsControl.PointerMoved += (sneder, args) =>
            {
                var pnt = args.GetCurrentPoint(_currentItemsItemsControl);
                Debug.WriteLine("Point: " + pnt.Position);
            };*/
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
