using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using Avalonia.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    /// <summary>
    /// A control which mimicks the customizable Toolbars found in many native macOS apps.
    /// </summary>
    public class ToolStrip : ItemsControl, IItemsPresenterHost
    {
        ObservableCollection<ToolStripItemReference> _defaultItems = new ObservableCollection<ToolStripItemReference>();

        /// <summary>
        /// Defines the <see cref="DefaultItems"/> property.
        /// </summary>
        public static readonly DirectProperty<ToolStrip, ObservableCollection<ToolStripItemReference>> DefaultItemsProperty =
            AvaloniaProperty.RegisterDirect<ToolStrip, ObservableCollection<ToolStripItemReference>>(nameof(DefaultItems), o => o.DefaultItems, (o, v) => o.DefaultItems = v);
        
        /// <summary>
        /// Gets or sets a value indicating what items should be placed into the <see cref="ToolStrip"/> initially or when resetting the <see cref="CurrentItems"/>.
        /// </summary>
        public ObservableCollection<ToolStripItemReference> DefaultItems
        {
            get => _defaultItems;
            set => SetAndRaise(DefaultItemsProperty, ref _defaultItems, value);
        }

        ObservableCollection<ToolStripItemReference> _currentItems = new ObservableCollection<ToolStripItemReference>();
        
        /// <summary>
        /// Defines the <see cref="CurrentItems"/> property.
        /// </summary>
        public static readonly DirectProperty<ToolStrip, ObservableCollection<ToolStripItemReference>> CurrentItemsProperty =
            AvaloniaProperty.RegisterDirect<ToolStrip, ObservableCollection<ToolStripItemReference>>(nameof(CurrentItems), o => o.CurrentItems, (o, v) => o.CurrentItems = v);
        
        /// <summary>
        /// Gets or sets a value indicating what items are currently present in the <see cref="ToolStrip"/>.
        /// </summary>
        public ObservableCollection<ToolStripItemReference> CurrentItems
        {
            get => _currentItems;
            set => SetAndRaise(CurrentItemsProperty, ref _currentItems, value);
        }

        ObservableCollection<ToolStripItemReference> _availableItems = new ObservableCollection<ToolStripItemReference>();
        
        /// <summary>
        /// Defines the <see cref="AvailableItems"/> property.
        /// </summary>
        public static readonly DirectProperty<ToolStrip, ObservableCollection<ToolStripItemReference>> AvailableItemsProperty =
            AvaloniaProperty.RegisterDirect<ToolStrip, ObservableCollection<ToolStripItemReference>>(nameof(AvailableItems), o => o.AvailableItems, (o, v) => o.AvailableItems = v);

        /// <summary>
        /// Gets or sets a value indicating what items can be added to the <see cref="ToolStrip"/> when <see cref="IsCustomizing"/> is <see langword="true"/>.
        /// </summary>
        public ObservableCollection<ToolStripItemReference> AvailableItems
        {
            get => _availableItems;
            set => SetAndRaise(AvailableItemsProperty, ref _availableItems, value);
        }

        /// <summary>
        /// Defines the <see cref="IsCustomizing"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsCustomizingProperty =
            AvaloniaProperty.Register<ToolStrip, bool>(nameof(IsCustomizing), false);

        /// <summary>
        /// Gets or sets a value indicating whether or not the <see cref="ToolStrip"/>'s Customization <see cref="Popup"/> is currently open. Commands on the <see cref="ToolStrip"/> cannot be invoked by the user when this property is <see langword="true"/>, as mouse interaction is needed to rearrange the items.
        /// </summary>
        public bool IsCustomizing
        {
            get => GetValue(IsCustomizingProperty);
            set => SetValue(IsCustomizingProperty, value);
        }

        /// <summary>
        /// Defines the <see cref="ShowLabels"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> ShowLabelsProperty =
            AvaloniaProperty.Register<ToolStrip, bool>(nameof(ShowLabels), false);

        /// <summary>
        /// Gets or sets a value indicating whether or not the <see cref="ToolStrip"/>'s <see cref="CurrentItems"/> should show text-labels beneath themselves. The <see cref="AvailableItems"/> do not obey this property, and show text-labels regardless of its value.
        /// </summary>
        public bool ShowLabels
        {
            get => GetValue(ShowLabelsProperty);
            set => SetValue(ShowLabelsProperty, value);
        }


        static ToolStrip()
        {
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
            ((AvaloniaList<object>)Items).CollectionChanged += Items_CollectionChanged;
            CurrentItems.CollectionChanged += CurrentItems_CollectionChanged;
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
                foreach (ToolStripItemReference rfrnc in e.OldItems)
                {   
                    if (!AvailableItems.Any(x => x.TargetItem.Equals(rfrnc.TargetItem) || (x.TargetItem == rfrnc.TargetItem)))
                        AvailableItems.Add(rfrnc);
                }
            }

            if (e.NewItems != null)
            {
                foreach (ToolStripItemReference rfrnc in e.NewItems.OfType<ToolStripItemReference>().Where(x => x.TargetItem != null))
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
                ((AvaloniaList<object>)Items).RemoveAll(flexibleSpaces);

            if (flexibleSpaces.Count() == 0)
                ((AvaloniaList<object>)Items).Insert(0, ToolStripFlexibleSpaceReference.ItemInstance);
        }
        
        public ToolStripItemReference HoverItem = null;
        internal void ValidateAddToToolStrip(IToolStripItem item, Visual sender, Vector cursor)
        {
            var popupRoot = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(sender);
            
            var point = VisualRoot.PointToClient(sender.PointToScreen(new Point(cursor.X, cursor.Y)));

            var hitTestItems = VisualRoot.Renderer.HitTest(point, VisualRoot, null).OfType<Visual>();
            
            Point tL = _currentItemsItemsControl.TranslatePoint(new Point(0, 0), VisualRoot).GetValueOrDefault();
            Point bR = _currentItemsItemsControl.TranslatePoint(new Point(_currentItemsItemsControl.Bounds.Size.Width, _currentItemsItemsControl.Bounds.Size.Height), VisualRoot).GetValueOrDefault();
            if ((point.X > tL.X) && (point.Y > tL.Y) && (point.X < bR.X) && (point.Y < bR.Y))
            {
                var hoverItem = Avalonia.VisualTree.VisualExtensions.GetVisualDescendants(_currentItemsItemsControl).OfType<Visual>().FirstOrDefault(x => hitTestItems.Contains(x) && _currentItemsItemsControl.Items.OfType<object>().Contains(x.DataContext)).DataContext;

                if (hoverItem != null)
                    CurrentItems.Insert(_currentItemsItemsControl.Items.OfType<object>().ToList().IndexOf(hoverItem), item.ToReference()); /*(CurrentItems.Last(x => x.TargetItem == HoverItems.Last()))*, item.ToReference());*/
                else
                    CurrentItems.Add(item.ToReference());
            }
        }
        internal void ValidateMoveOrRemoveFromToolStrip(ToolStripItemReference item, Visual sender, Vector cursor)
        {
            var point = sender.TranslatePoint(new Point(cursor.X, cursor.Y), VisualRoot).GetValueOrDefault();
            
            if (CurrentItems.Contains(item))
                CurrentItems.Remove(item);
            
            var hitTestItems = VisualRoot.Renderer.HitTest(point, VisualRoot, null);
            
            Point tL = _currentItemsItemsControl.TranslatePoint(new Point(0, 0), VisualRoot).GetValueOrDefault();
            Point bR = _currentItemsItemsControl.TranslatePoint(new Point(_currentItemsItemsControl.Bounds.Size.Width, _currentItemsItemsControl.Bounds.Size.Height), VisualRoot).GetValueOrDefault();
            if ((point.X > tL.X) && (point.Y > tL.Y) && (point.X < bR.X) && (point.Y < bR.Y))
            {
                var hoverItem = Avalonia.VisualTree.VisualExtensions.GetVisualDescendants(_currentItemsItemsControl).OfType<Visual>().FirstOrDefault(x => hitTestItems.Contains(x) && CurrentItems.Contains(x.DataContext));
                if (hoverItem != null)
                {
                    if (hoverItem.DataContext is ToolStripItemReference tItem)
                        CurrentItems.Insert(CurrentItems.IndexOf(tItem), item);
                }
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrentItems"/> to match the <see cref="DefaultItems"/>.
        /// </summary>
        public void ResetToDefaults()
        {
            CurrentItems.Clear();
            
            foreach (ToolStripItemReference reference in DefaultItems)
                CurrentItems.Add(reference);
        }

        Rectangle _popupDragMovePreview = null;
        Rectangle _windowDragMovePreview = null;
        double _dragMoveStartLeft = 0;
        double _dragMoveStartTop = 0;

        ItemsControl _currentItemsItemsControl = null;
        ItemsControl _defaultItemsItemsControl = null;
        Thumb _defaultItemsDragThumb = null;
        internal Popup _customizePopup = null;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            
            _currentItemsItemsControl = e.NameScope.Get<ItemsControl>("PART_CurrentItemsItemsControl");
            _defaultItemsItemsControl = e.NameScope.Get<ItemsControl>("PART_DefaultItemsItemsControl");
            _customizePopup = e.NameScope.Get<Popup>("PART_CustomizeFlyout");
            
            _defaultItemsDragThumb = e.NameScope.Get<Thumb>("PART_DefaultItemsThumb");
            _defaultItemsDragThumb.DragStarted += DefaultItemsDragThumb_DragStarted;
            _defaultItemsDragThumb.DragDelta += DefaultItemsDragThumb_DragDelta;
            _defaultItemsDragThumb.DragCompleted += DefaultItemsDragThumb_DragCompleted;
        }

        private void DefaultItemsDragThumb_DragStarted(object sender, VectorEventArgs e)
        {
            var visRoot = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(_defaultItemsDragThumb);
            var pnt = _defaultItemsItemsControl.TranslatePoint(new Point(0, 0), visRoot).GetValueOrDefault();
            _dragMoveStartLeft = pnt.X;
            _dragMoveStartTop = pnt.Y;


            var pxSize = new PixelSize((int)(_defaultItemsItemsControl.Bounds.Width * visRoot.RenderScaling), (int)(_defaultItemsItemsControl.Bounds.Height * visRoot.RenderScaling));
            RenderTargetBitmap bmp = new RenderTargetBitmap(pxSize);
            bmp.Render(_defaultItemsItemsControl);
            ImageBrush brush = null;
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream);
                stream.Position = 0;
                brush = new ImageBrush(new Bitmap(stream));
            }

            _popupDragMovePreview = new Rectangle()
            {
                Width = _defaultItemsItemsControl.Bounds.Width,
                Height = _defaultItemsItemsControl.Bounds.Height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Fill = brush,
                Margin = new Thickness(_dragMoveStartLeft, _dragMoveStartTop, -_dragMoveStartLeft, -_dragMoveStartTop)
            };
            _windowDragMovePreview = new Rectangle()
            {
                Width = _defaultItemsItemsControl.Bounds.Width,
                Height = _defaultItemsItemsControl.Bounds.Height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Fill = brush,
                Margin = PopupMarginToWindowMargin(_popupDragMovePreview.Margin)
            };

            
            AdornerLayer.GetAdornerLayer(_defaultItemsDragThumb).Children.Add(_popupDragMovePreview);
            AdornerLayer.GetAdornerLayer(this).Children.Add(_windowDragMovePreview);
        }

        private void DefaultItemsDragThumb_DragDelta(object sender, VectorEventArgs e)
        {
            double x = _dragMoveStartLeft + e.Vector.X;
            double y = _dragMoveStartTop + e.Vector.Y;
            _popupDragMovePreview.Margin = new Thickness(x, y, -x, -y);
            _windowDragMovePreview.Margin = PopupMarginToWindowMargin(_popupDragMovePreview.Margin);
        }

        private void DefaultItemsDragThumb_DragCompleted(object sender, VectorEventArgs e)
        {
            var popupLayer = AdornerLayer.GetAdornerLayer(_defaultItemsDragThumb);
            
            if (popupLayer.Children.Contains(_popupDragMovePreview))
                popupLayer.Children.Remove(_popupDragMovePreview);

            var windowLayer = AdornerLayer.GetAdornerLayer(this);
            
            if (windowLayer.Children.Contains(_windowDragMovePreview))
                windowLayer.Children.Remove(_windowDragMovePreview);

            _popupDragMovePreview = null;

            var popupRoot = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(_defaultItemsDragThumb);
        
            var point = VisualRoot.PointToClient(_defaultItemsDragThumb.PointToScreen(new Point(e.Vector.X, e.Vector.Y)));
            
            Point tL = _currentItemsItemsControl.TranslatePoint(new Point(0, 0), VisualRoot).GetValueOrDefault();
            Point bR = _currentItemsItemsControl.TranslatePoint(new Point(_currentItemsItemsControl.Bounds.Size.Width, _currentItemsItemsControl.Bounds.Size.Height), VisualRoot).GetValueOrDefault();
            if ((point.X > tL.X) && (point.Y > tL.Y) && (point.X < bR.X) && (point.Y < bR.Y))
            {
                ResetToDefaults();
            }
        }

        Thickness PopupMarginToWindowMargin(Thickness popupMargin)
        {   
            var pnt = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(this).PointToClient(Avalonia.VisualTree.VisualExtensions.GetVisualRoot(_defaultItemsDragThumb).PointToScreen(new Point(popupMargin.Left, popupMargin.Top)));
            return new Thickness(pnt.X, pnt.Y, -pnt.X, -pnt.Y);
        }

        void IItemsPresenterHost.RegisterItemsPresenter(IItemsPresenter presenter)
        {
            Presenter = presenter;
        }
    }
}
