using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

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

        public static readonly DirectProperty<OverflowFlyoutItemsControl, AvaloniaList<object>> VisibleItemsProperty = AvaloniaProperty.RegisterDirect<OverflowFlyoutItemsControl, AvaloniaList<object>>(nameof(VisibleItems), o => o.VisibleItems, (o, v) => o.VisibleItems = v);
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
        }

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
                //sneder.InvalidateArrange();
                //sneder.InvalidateMeasure();
                sneder.SortControls();
            }));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            SortControls();
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureCore(Size availableSize)
        {
            //MinWidth = CalcSizeWithFirstControlOnly();
            Size baseSize = base.MeasureCore(availableSize);
            if (ForceDesiredSize && (Items != null))
            {
                double realWidth = SortControls();
                var items = Items.OfType<Control>();
                if (items.Count() > 0)
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
            VisibleItems.Clear();
            FlyoutItems.Clear();
            double _totalWidth = 0;
            if (_visPres != null)
            {
                //Debug.WriteLine("max width: " + _visPres.DesiredSize.Width);
                var children = LogicalChildren.OfType<object>();
                if (OverflowDirection == Direction.Left)
                    children = children.Reverse();

                foreach (object obj in children)
                {
                    if (obj is Control ctrl)
                    {
                        _totalWidth += ctrl.DesiredSize.Width;
                        //Debug.WriteLine("_totalWidth: " + _totalWidth);
                        if (_totalWidth <= _visPres.Bounds.Width)
                            AddToVisibleItems(ctrl);
                        else
                            AddToFlyoutItems(ctrl);
                    }
                }
            }
            HasFlyoutItems = FlyoutItems.Count > 0;
            /*else
                Debug.WriteLine("_visPres == null");*/
            return _totalWidth;
        }

        public void AddToVisibleItems(object obj)
        {
            if (FlyoutItems.Contains(obj))
                FlyoutItems.Remove(obj);
            VisibleItems.Add(obj);
        }

        public void AddToFlyoutItems(object obj)
        {
            if (VisibleItems.Contains(obj))
                VisibleItems.Remove(obj);
            FlyoutItems.Add(obj);
        }

        ItemsControl _visPres = null;
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            _visPres = e.NameScope.Find<ItemsControl>("PART_VisibleItemsPresenter");
            SortControls();
        }
    }
}
