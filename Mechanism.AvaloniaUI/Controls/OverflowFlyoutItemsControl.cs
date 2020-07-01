using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Avalonia.Controls.Generators;

namespace Mechanism.AvaloniaUI.Controls
{
    public enum Direction
    {
        Left,
        Right
    }
    public class OverflowFlyoutItemsControl : ItemsControl
    {
        public static readonly StyledProperty<ChildrenHorizontalAlignment> HorizontalItemsAlignmentProperty =
            AvaloniaProperty.Register<OverflowFlyoutItemsControl, ChildrenHorizontalAlignment>(nameof(HorizontalItemsAlignment), defaultValue: ChildrenHorizontalAlignment.Left);

        public ChildrenHorizontalAlignment HorizontalItemsAlignment
        {
            get => GetValue(HorizontalItemsAlignmentProperty);
            set => SetValue(HorizontalItemsAlignmentProperty, value);
        }

        /*public static readonly DirectProperty<OverflowFlyoutItemsControl, AvaloniaList<object>> VisibleItemsProperty = AvaloniaProperty.RegisterDirect<OverflowFlyoutItemsControl, AvaloniaList<object>>(nameof(VisibleItems), o => o.VisibleItems, (o, v) => o.VisibleItems = v);
        private AvaloniaList<object> _visibleItems = new AvaloniaList<object>();
        public AvaloniaList<object> VisibleItems
        {
            get { return _visibleItems; }
            set { SetAndRaise(VisibleItemsProperty, ref _visibleItems, value); }
        }

        public static readonly DirectProperty<OverflowFlyoutItemsControl, AvaloniaList<object>> FlyoutItemsProperty = AvaloniaProperty.RegisterDirect<OverflowFlyoutItemsControl, AvaloniaList<object>>(nameof(FlyoutItems), o => o.FlyoutItems, (o, v) => o.FlyoutItems = v);
        private AvaloniaList<object> _flyoutItems = new AvaloniaList<object>();
        public AvaloniaList<object> FlyoutItems
        {
            get { return _flyoutItems; }
            set { SetAndRaise(FlyoutItemsProperty, ref _flyoutItems, value); }
        }*/

        public static readonly StyledProperty<bool> IsFlyoutOpenProperty =
        AvaloniaProperty.Register<OverflowFlyoutItemsControl, bool>(nameof(IsFlyoutOpen), defaultValue: false);

        public bool IsFlyoutOpen
        {
            get { return GetValue(IsFlyoutOpenProperty); }
            set { SetValue(IsFlyoutOpenProperty, value); }
        }

        public static readonly StyledProperty<Direction> OverflowDirectionProperty =
        AvaloniaProperty.Register<OverflowFlyoutItemsControl, Direction>(nameof(OverflowDirection), defaultValue: Direction.Right);

        public Direction OverflowDirection
        {
            get { return GetValue(OverflowDirectionProperty); }
            set { SetValue(OverflowDirectionProperty, value); }
        }

        public static readonly StyledProperty<bool> HasFlyoutItemsProperty =
            AvaloniaProperty.Register<OverflowFlyoutItemsControl, bool>(nameof(HasFlyoutItems), defaultValue: false);

        public bool HasFlyoutItems
        {
            get { return GetValue(HasFlyoutItemsProperty); }
            private set { SetValue(HasFlyoutItemsProperty, value); }
        }

        private static readonly FuncTemplate<IPanel> FlyoutDefaultPanel = new FuncTemplate<IPanel>(() => new WrapPanel());

        public static readonly StyledProperty<ITemplate<IPanel>> FlyoutItemsPanelProperty =
            AvaloniaProperty.Register<ItemsControl, ITemplate<IPanel>>(nameof(FlyoutItemsPanel), FlyoutDefaultPanel);

        public ITemplate<IPanel> FlyoutItemsPanel
        {
            get => GetValue(FlyoutItemsPanelProperty);
            set => SetValue(FlyoutItemsPanelProperty, value);
        }

        public static readonly StyledProperty<double> BaseWidthProperty =
            AvaloniaProperty.Register<OverflowFlyoutItemsControl, double>(nameof(BaseWidth), defaultValue: 0);

        public double BaseWidth
        {
            get { return GetValue(BaseWidthProperty); }
            set { SetValue(BaseWidthProperty, value); }
        }

        public static readonly StyledProperty<bool> ForceDesiredSizeProperty =
            AvaloniaProperty.Register<OverflowFlyoutItemsControl, bool>(nameof(ForceDesiredSize), defaultValue: false);

        public bool ForceDesiredSize
        {
            get => GetValue(ForceDesiredSizeProperty);
            set => SetValue(ForceDesiredSizeProperty, value);
        }

        protected virtual double GetBaseWidth()
        {
            return BaseWidth;
        }

        static OverflowFlyoutItemsControl()
        {
            AffectsMeasure<OverflowFlyoutItemsControl>(ItemsProperty);
            AffectsArrange<OverflowFlyoutItemsControl>(ItemsProperty);
            AffectsRender<OverflowFlyoutItemsControl>(ItemsProperty);

            AffectsMeasure<OverflowFlyoutItemsControl>(HorizontalItemsAlignmentProperty);
            AffectsArrange<OverflowFlyoutItemsControl>(HorizontalItemsAlignmentProperty);
            AffectsRender<OverflowFlyoutItemsControl>(HorizontalItemsAlignmentProperty);

            AffectsMeasure<AlignableStackPanel>(ForceDesiredSizeProperty);
            AffectsArrange<AlignableStackPanel>(ForceDesiredSizeProperty);
            AffectsRender<AlignableStackPanel>(ForceDesiredSizeProperty);

            BoundsProperty.Changed.AddClassHandler<OverflowFlyoutItemsControl>(new Action<OverflowFlyoutItemsControl, AvaloniaPropertyChangedEventArgs>((sneder, args) =>
            {
                sneder.InvalidateArrange();
                sneder.InvalidateMeasure();
                sneder.SortControls();
            }));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            SortControls();
            return base.ArrangeOverride(finalSize);
        }


        List<IControl> _items = new List<IControl>();
        List<IControl> GetControlItems()
        {
            /*if ((Presenter?.Panel != null) && (Presenter.Panel.Children.Count > 0))
                return Presenter.Panel.Children.ToList<IControl>();
            else
            {*/
            //ItemContainerGenerator.Clear();
            List<IControl> items = null;
            var itemsCollection = Items.Cast<object>().ToList();
            if (itemsCollection.All(x => x is IControl))
                items = itemsCollection.Cast<IControl>().ToList();
            else if (true)
            {
                items = new List<IControl>();
                foreach (object obj in itemsCollection)
                {
                    if (obj is IControl ctrl)
                        items.Add(ctrl);
                    else
                    {
                        var ctrl2 = ItemTemplate.Build(obj);
                        ctrl2.DataContext = obj;

                        items.Add(ctrl2 /*new ListBoxItem()
                        {
                            //DataContext = obj,
                            Content = obj,
                            ContentTemplate = ItemTemplate
                            //[!ListBoxItem.ContentTemplateProperty] = this[!OverflowFlyoutItemsControl.ItemTemplateProperty]
                        }*//*ItemContainerGenerator.Materialize(itemsCollection.IndexOf(obj), obj).ContainerControl*/);
                    }
                }
            }
            else
            {
                items = new List<IControl>();
                foreach (object obj in itemsCollection)
                {
                    if (obj is IControl ctrl)
                        items.Add(ctrl);
                    else
                    {
                        /*var ctnr = ItemContainerGenerator.Containers.FirstOrDefault(x => x.Item == obj);

                        if (ctnr != null)
                            items.Add(ctnr.ContainerControl);
                        Debug.WriteLine("NO CONTAINER");*/
                        int index = itemsCollection.IndexOf(obj);
                        Debug.WriteLine("ITEM INDEX: " + index + ", CONTAINER COUNT: " + ItemContainerGenerator.Containers.Count());
                        var ctnr = ItemContainerGenerator.ContainerFromIndex(index);
                        if (ctnr != null)
                        {
                            items.Add(ctnr);
                            Debug.WriteLine("CONTAINER ADDED");
                        }
                        else
                        {
                            var ctnr2 = ItemContainerGenerator.Containers.FirstOrDefault(x => x.Item == obj);

                            if (ctnr2 != null)
                                items.Add(ctnr2.ContainerControl);
                            else
                                Debug.WriteLine("NO CONTAINER FOR ITEM OF TYPE " + obj.GetType().FullName);
                        }
                        /*else
                            items.Add(ItemContainerGenerator.Materialize(itemsCollection.IndexOf(obj), obj).ContainerControl);*/
                    }
                }
            }
            return items;
            //}
            /*if (Presenter?.Panel != null)
            {
                if (Presenter.Panel.Children.Count > 0)
                    _items = Presenter.Panel.Children.ToList<IControl>();
                //return Presenter.Panel.Children.ToList<IControl>();
            }
            return _items;*/
            /*else
            {
                Debug.WriteLine("NO PANEL");
                return new List<IControl>();
            }*/
        }

        protected override Size MeasureCore(Size availableSize)
        {
            //MinWidth = CalcSizeWithFirstControlOnly();
            Size baseSize = base.MeasureCore(availableSize);
            if (ForceDesiredSize && (Items != null))
            {
                double realWidth = SortControls();
                var items = GetControlItems();
                if ((items != null) && (items.Count() > 0))
                {
                    var visibleItems = items.Where(x => x.IsVisible);
                    if (visibleItems.Count() > 0)
                    {
                        //MinWidth = BaseWidth + visibleItems.ElementAt(0).DesiredSize.Width;
                        if (OverflowDirection == Direction.Left)
                            MinWidth = GetBaseWidth() + visibleItems.Last().DesiredSize.Width;
                        else
                            MinWidth = GetBaseWidth() + visibleItems.First().DesiredSize.Width;
                    }
                    else
                        MinWidth = GetBaseWidth();
                }
                return new Size(realWidth + GetBaseWidth(), baseSize.Height);
            }
            else
                return baseSize;
        }

        /*private double CalcSizeWithFirstControlOnly()
        {
            var controls = Items.OfType<Control>();
            if (controls.Count() > 0)
                return base.MeasureCore(new Size(controls.ElementAt(0).DesiredSize.Width * 2, controls.ElementAt(0).DesiredSize.Height * 2)).Width;
            else
                return base.MeasureCore(Size.Infinity).Width;
        }*/

        private double SortControls()
        {
            /*VisibleItems.Clear();
            FlyoutItems.Clear();*/
            _visPanel.Children.Clear();
            _flyPanel.Children.Clear();
            double _totalWidth = 0;
            if ((_visPanel != null) && (_flyPanel != null))
            {
                //Debug.WriteLine("max width: " + _visPres.DesiredSize.Width);
                var children = GetControlItems();
                if (children != null)
                {
                    if (OverflowDirection == Direction.Left)
                        /*children = */
                        children.Reverse();

                    foreach (object obj in children)
                    {
                        if (obj is Control ctrl)
                        {
                            _totalWidth += ctrl.DesiredSize.Width;
                            //Debug.WriteLine("_totalWidth: " + _totalWidth);
                            if (_totalWidth <= _visPanel.Bounds.Width)
                                AddToVisibleItems(ctrl);
                            else
                                AddToFlyoutItems(ctrl);
                        }
                    }
                }
            }
            HasFlyoutItems = _flyPanel.Children.Count > 0;
            /*else
                Debug.WriteLine("_visPres == null");*/
            return _totalWidth;
        }

        public void AddToVisibleItems(IControl obj)
        {
            /*if (obj.Parent != null)
            {
                if (obj.Parent is ItemsControl con)
                    con.Presenter.Panel?.Children.Remove(obj);
                else if (obj.Parent is Panel pnl)
                    pnl.Children.Remove(obj);
                else if (obj.Parent is ContentControl ctrl)
                    ctrl.Content = null;
            }*/
            if (obj.Parent != null)
                RemoveFrom(obj, obj.Parent);
            if (obj.VisualParent != null)
                RemoveFrom(obj, obj.VisualParent);

            /*if (_flyPanel.Children.Contains(obj))
                _flyPanel.Children.Remove(obj);*/
            //Debug.WriteLine("PARENT: " + obj.Parent);
            _visPanel.Children.Add(obj);
        }

        public void AddToFlyoutItems(IControl obj)
        {
            /*if (obj.Parent != null)
                (obj.Parent as Panel).Children.Remove(obj);*/
            if (obj.Parent != null)
                RemoveFrom(obj, obj.Parent);
            if (obj.VisualParent != null)
                RemoveFrom(obj, obj.VisualParent);

            /*if (_visPanel.Children.Contains(obj))
                _visPanel.Children.Remove(obj);*/
            //Debug.WriteLine("PARENT: " + obj.Parent);
            _flyPanel.Children.Add(obj);
        }

        void RemoveFrom(IControl obj, object parent)
        {
            if (parent != null)
            {
                if (parent is ItemsControl con)
                    con.Presenter.Panel?.Children.Remove(obj);
                else if (parent is Panel pnl)
                    pnl.Children.Remove(obj);
                else if (parent is ContentControl ctrl)
                    ctrl.Content = null;
            }
        }

        //ItemsControl _visPres = null;
        //Panel _visHost = null;
        Panel _visPanel = null;
        Panel _flyPanel = null;
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            //_visPres = e.NameScope.Find<ItemsControl>("PART_VisibleItemsPresenter");
            _visPanel = e.NameScope.Find<Panel>("PART_VisibleItemsPanel");
            _flyPanel = e.NameScope.Find<Panel>("PART_FlyoutItemsPanel");
            //_visHost = _visPres.GetTemplateChildren()/*.Find<Panel>*/.First(x => x.Name == "PART_VisibleItemsHost") as Panel;
            //Debug.WriteLine("_visHost != null: " + (_visHost != null));
            SortControls();
        }

        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ItemContainerGenerator<ListBoxItem>(this, ListBoxItem.ContentProperty, ListBoxItem.ContentTemplateProperty);
            //return new ItemContainerGenerator(this);
        }
    }
}
