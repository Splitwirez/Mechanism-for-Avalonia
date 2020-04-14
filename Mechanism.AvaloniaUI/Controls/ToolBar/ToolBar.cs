using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using System.Diagnostics;
using Avalonia.Metadata;
using System.Collections.ObjectModel;
using Avalonia.Input;
using System.Timers;
using Avalonia.Threading;

namespace Mechanism.AvaloniaUI.Controls.ToolBar
{
    public class ToolBar : ItemsControl
    {
        public static readonly StyledProperty<int> BandProperty =
        AvaloniaProperty.Register<ToolBar, int>(nameof(Band), defaultValue: 0);

        public int Band
        {
            get { return GetValue(BandProperty); }
            set { SetValue(BandProperty, value); }
        }

        public static readonly StyledProperty<int> BandIndexProperty =
        AvaloniaProperty.Register<ToolBar, int>(nameof(BandIndex), defaultValue: 0);

        public int BandIndex
        {
            get { return GetValue(BandIndexProperty); }
            set { SetValue(BandIndexProperty, value); }
        }

        public static readonly StyledProperty<double> BaseWidthProperty =
            OverflowFlyoutItemsControl.BaseWidthProperty.AddOwner<ToolBar>();

        public double BaseWidth
        {
            get { return GetValue(BaseWidthProperty); }
            set { SetValue(BaseWidthProperty, value); }
        }

        public static readonly StyledProperty<bool> UseOverflowProperty =
        AvaloniaProperty.Register<ToolBar, bool>(nameof(UseOverflow), defaultValue: true);

        public bool UseOverflow
        {
            get => GetValue(UseOverflowProperty);
            set => SetValue(UseOverflowProperty, value);
        }

        public static readonly StyledProperty<bool> IsFlyoutOpenProperty =
            OverflowFlyoutItemsControl.IsFlyoutOpenProperty.AddOwner<ToolBar>();

        public bool IsFlyoutOpen
        {
            get => GetValue(IsFlyoutOpenProperty);
            set => SetValue(IsFlyoutOpenProperty, value);
        }


        Thumb _gripThumb = null;
        OverflowFlyoutItemsControl _overflowControl;
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            _overflowControl = e.NameScope.Find<OverflowFlyoutItemsControl>("PART_OverflowItemsControl");
            _gripThumb = e.NameScope.Find<Thumb>("PART_GripThumb");
            if (_gripThumb != null)
            {
                _gripThumb.DragDelta += GripThumb_DragDelta;
                _gripThumb.DragStarted += GripThumb_DragStarted;
                //_gripThumb.PointerPressed += (sneder, args) => Debug.WriteLine("Pointer pressed");
                //_gripThumb.PointerPressed += GripThumb_PointerPressed;
                //_gripThumb.PointerReleased += (sneder, args) => _pointerDown = false;
            }
        }

        double _localX = -1;
        double _localY = -1;
        private void GripThumb_DragStarted(object sender, VectorEventArgs e)
        {
            Debug.WriteLine("Drag started");
            _localX = e.Vector.X;
            _localY = e.Vector.Y;
        }

        bool _pointerDown = false;
        private void GripThumb_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            _pointerDown = true;
            Timer timer = new Timer(1);
            var parent = Parent as ToolBarTray;
            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.UIThread.Post(new Action(() =>
                {
                    Debug.WriteLine("Dispatcher...");
                    if (!_pointerDown)
                        timer.Stop();
                    else
                    {
                        ToolBar t = null;
                        foreach (ToolBar bar in parent.Items.OfType<ToolBar>())
                        {
                            Debug.WriteLine("Iterating...");
                            var point = e.GetPosition(bar);
                            point = point.WithX(point.X + 1);
                            point = point.WithY(point.Y + 1);
                            if (VisualRoot.Renderer.HitTest(point, bar, null).Contains(bar))
                            {
                                t = bar;
                                Debug.WriteLine("bar found!");
                            }
                        }
                        if (t != null) //foreach (ToolBar t in parent.Items.OfType<ToolBar>())
                        {
                            //PixelPoint tl = t.PointToScreen(t.Bounds.TopLeft);
                            //PixelPoint br = t.PointToScreen(t.Bounds.BottomRight);
                            /*bool horizontal = 0 <= e.GetPosition(t).X;
                            horizontal = horizontal && (e.GetPosition(t).X < t.Bounds.Width);
                            bool vertical = 0 <= e.GetPosition(t).Y;
                            vertical = vertical && (e.GetPosition(t).Y < t.Bounds.Height);*/
                            if (t != this) //((t != this) && (horizontal && vertical))
                            {
                                int index = BandIndex;
                                BandIndex = t.BandIndex;
                                t.BandIndex = index;
                                if (t.Band != Band)
                                    Band = t.Band;
                            }
                        }
                    }
                }));
            };
            timer.Start();
        }

        private void GripThumb_DragDelta(object sender, VectorEventArgs e)
        {
            double localX = _localX + e.Vector.X;
            double localY = _localY + e.Vector.Y;
            /*double localX = e.Vector.X - _localX;
            double localY = e.Vector.Y - _localY;*/
            //Debug.WriteLine("DragDelta: " + e.Vector.ToString() + "; " + localX + ", " + localY);
            ToolBar mouseOverBar = null;
            var parent = Parent as ToolBarTray;
            PixelPoint cursorPoint = _gripThumb.PointToScreen(new Point(localX, localY))/*.ToPoint(VisualRoot.RenderScaling)*/;
            var items = parent.Items.OfType<ToolBar>();
            /*bool swapLeft = false;
            bool swapRight = false;*/
            bool leftHalf = false;
            foreach (ToolBar bar in items)
            {
                Debug.WriteLine("INDEX: " + items.ToList().IndexOf(bar));
                /*var clientPoint = _gripThumb.TranslatePoint(, VisualRoot);
                if (clientPoint.HasValue)
                {
                    Debug.WriteLine("clientPoint: " + clientPoint.Value.ToString());*/
                //VisualRoot.Renderer.HitTest(/*clientPoint.Value*/new Point(localX, localY), VisualRoot, null);
                //var thisBarPoint = this.PointToScreen(new Point(0, 0)) + ;
                var barPoint = bar.PointToScreen(new Point(0, 0));
                Debug.WriteLine("points: " + cursorPoint.ToString() + "; " + barPoint.ToString());
                bool left = barPoint.X <= cursorPoint.X;
                bool top = barPoint.Y <= cursorPoint.Y;
                bool right = (barPoint.X + (bar.Bounds.Width / VisualRoot.RenderScaling)) >= cursorPoint.X;
                bool bottom = (barPoint.Y + (bar.Bounds.Height / VisualRoot.RenderScaling)) >= cursorPoint.Y;
                leftHalf = (barPoint.X + ((bar.Bounds.Width / VisualRoot.RenderScaling) / 2)) >= cursorPoint.X;
                Debug.WriteLine("Bounds comparison: " + left + ", " + top + ", " + right + ", " + bottom);
                //Debug.WriteLine("result.Count: " + result.Count());
                //if (result.Contains(bar)) //if (bar.IsPointerOver)
                if (left && top && right && bottom)
                {
                    mouseOverBar = bar;
                    break;
                    /*if ((!left) && (bar.Band == Band) && (bar.BandIndex == (BandIndex - 1)))
                    {
                        mouseOverBar = bar;
                        swap = true;
                        break;
                    }
                    else if (left)
                    {
                        mouseOverBar = bar;
                        break;
                    }*/
                }
                //}
            }

            var trayPoint = parent.PointToScreen(new Point(0, 0));
            Debug.WriteLine("points: " + cursorPoint.ToString() + "; " + trayPoint.ToString());
            //bool left = trayPoint.X <= cursorPoint.X;
            bool aboveTop = trayPoint.Y > cursorPoint.Y;
            //bool right = (trayPoint.X + (parent.Bounds.Width / VisualRoot.RenderScaling)) >= cursorPoint.X;
            bool belowBottom = (trayPoint.Y + (parent.Bounds.Height / VisualRoot.RenderScaling)) < cursorPoint.Y;
            if (belowBottom && (parent.Items.OfType<ToolBar>().Where(x => x.Band == parent.GetBandCount()).Count() > 1)) //(parent.Items.OfType<ToolBar>().Where(x => x.Band == (parent.GetBandCount() - 1)).Count()*/)// && (parent.Items.OfType<ToolBar>().Where(x => x.Band == (parent.GetBandCount() - 1)).Count() <= 1))
            {
                //Band = parent.Items.OfType<ToolBar>().Max(x => x.Band) + 1;
                Band = parent.GetBandCount() + 1;
            }
            else if (aboveTop && (parent.Items.OfType<ToolBar>().Where(x => x.Band == 0).Count() > 1))
            {
                foreach (ToolBar bar in parent.Items.OfType<ToolBar>().Where(x => x != this))
                {
                    bar.Band++;
                }
            }

            if ((mouseOverBar != null) && (mouseOverBar != this))
            {
                /*int index = BandIndex;
                BandIndex = mouseOverBar.BandIndex;
                mouseOverBar.BandIndex = index;*/
                if (Band == mouseOverBar.Band)
                {
                    bool swapLeft = (mouseOverBar.BandIndex == (BandIndex - 1)) && leftHalf;
                    bool swapRight = (mouseOverBar.BandIndex == (BandIndex + 1))/* && (!leftHalf)*/;
                    if (swapLeft || swapRight)
                    {
                        int index = BandIndex;
                        BandIndex = mouseOverBar.BandIndex;
                        mouseOverBar.BandIndex = index;
                        
                        double width = Width;
                        Width = mouseOverBar.Width;
                        mouseOverBar.Width = width;
                    }
                }
                else
                {
                    double width = Width;
                    ToolBar adjacent = items.FirstOrDefault(x => (x.Band == Band) && (x.BandIndex == (BandIndex + 1)));
                    if (adjacent == null)
                        adjacent = items.FirstOrDefault(x => (x.Band == Band) && (x.BandIndex == (BandIndex - 1)));
                    if (adjacent != null)
                        adjacent.Width += width;
                    Width = mouseOverBar.Width / 2;
                    mouseOverBar.Width /= 2;
                    Band = mouseOverBar.Band;
                    //BandIndex = mouseOverBar.BandIndex;
                    /*if ()
                    {
                        int index = BandIndex;
                        BandIndex = mouseOverBar.BandIndex;
                        mouseOverBar.BandIndex = index;
                    }*/
                }
            }
            if (BandIndex > 0)
            {
                ////Debug.WriteLine("GripThumb_DragDelta " + e.Vector.X);
                ToolBar targetBar = (Parent as ToolBarTray).Items.Cast<ToolBar>().Where(x => x.Band == Band).FirstOrDefault(x => x.BandIndex == (BandIndex - 1));
                
                if (targetBar != null)
                {
                    double newTargetWidth = targetBar.Width + e.Vector.X;
                    double newWidth = Width - e.Vector.X;
                    if ((newTargetWidth >= targetBar.MinWidth) && (newWidth >= MinWidth))
                    {
                        targetBar.Width = newTargetWidth; //+= e.Vector.X;
                        Width = newWidth; //-= e.Vector.X;
                    }
                    /*double newWidth = targetBar.Width + e.Vector.X;
                    if (targetBar.UseOverflow)
                    {

                    }
                    else
                    {
                        var items = targetBar.Items.OfType<Control>();
                        if ((items.Count() > 0) &&)
                        targetBar.Width = newWidth;
                    }*/
                    //if (targetBar.MaxWidth )
                    //ToolbarResized
                    ////Debug.WriteLine("targetBar width set to " + targetBar.Width);
                    /*targetBar.InvalidateArrange();
                    targetBar.InvalidateMeasure();
                    targetBar.InvalidateVisual();
                    (Parent as ToolBarTray).InvalidateArrange();
                    (Parent as ToolBarTray).InvalidateMeasure();
                    (Parent as ToolBarTray).InvalidateVisual();*/
                    //ToolBarTray parent = Parent as ToolBarTray;
                }
                /*else
                    Debug.WriteLine("targetBar == null");*/
            }
            parent.Measure(parent.Bounds.Size);
            parent.Arrange(parent.Bounds);
            ToolbarResized?.Invoke(this, new EventArgs());
            /*else
                Debug.WriteLine("BandIndex == 0");*/
        }

        protected override Size MeasureCore(Size availableSize)
        {
            //return new Size(0, base.MeasureCore(availableSize).Height);
            var baseSize = base.MeasureCore(availableSize);
            if (UseOverflow)
                MinWidth = _overflowControl.MinWidth + BaseWidth;
            else
            {
                /*var items = Items.OfType<Control>();
                if ((items.Count() > 0) && (items.ElementAt(0).IsVisible))
                    MinWidth = BaseWidth + items.ElementAt(0).DesiredSize.Width;
                else
                    MinWidth = BaseWidth;*/
                var items = Items.OfType<Control>();
                if (items.Count() > 0)
                {
                    var visibleItems = items.Where(x => x.IsVisible);
                    if (visibleItems.Count() > 0)
                        MinWidth = BaseWidth + visibleItems.ElementAt(0).DesiredSize.Width;
                    else
                        MinWidth = BaseWidth;
                }
            }

            /*if (UseOverflow && (!IsFlyoutOpen) && (items.Count() > 0))
            {
                List<bool> visibles = new List<bool>();
                if (items.Count() > 1)
                {
                    foreach (Control ctrl in items.Skip(1))
                    {
                        visibles.Add(ctrl.IsVisible);
                        ctrl.IsVisible = false;
                    }
                }
                MinWidth = base.MeasureCore(new Size(items.ElementAt(0).DesiredSize.Width + 100, double.PositiveInfinity)).Width;
                if (items.Count() > 1)
                {
                    foreach (Control ctrl in items.Skip(1))
                    {
                        ctrl.IsVisible = visibles.ElementAt(0);
                        visibles.RemoveAt(0);
                    }
                }
            }*/

            if (UseOverflow)
                return new Size(_overflowControl.DesiredSize.Width + BaseWidth, baseSize.Height);
            else
                return baseSize;
        }

        /*protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(0, base.MeasureOverride(availableSize).Height);
        }*/

        static ToolBar()
        {
            WidthProperty.OverrideDefaultValue<ToolBar>(100);
            AffectsArrange<ToolBar>(BandProperty, BandIndexProperty, WidthProperty);
            AffectsMeasure<ToolBar>(BandProperty, BandIndexProperty, WidthProperty);
            AffectsRender<ToolBar>(BandProperty, BandIndexProperty, WidthProperty);
            AffectsArrange<ToolBarTray>(BandProperty, BandIndexProperty, WidthProperty);
            AffectsMeasure<ToolBarTray>(BandProperty, BandIndexProperty, WidthProperty);
            AffectsRender<ToolBarTray>(BandProperty, BandIndexProperty, WidthProperty);
            AffectsArrange<ToolBarTrayPanel>(BandProperty, BandIndexProperty, WidthProperty);
            AffectsMeasure<ToolBarTrayPanel>(BandProperty, BandIndexProperty, WidthProperty);
            AffectsRender<ToolBarTrayPanel>(BandProperty, BandIndexProperty, WidthProperty);
        }

        /*public ToolBar()
        {
            if (double.IsNaN(Width))
                Width = DesiredSize.Width;
        }*/

        public static event EventHandler<EventArgs> ToolbarResized;
    }
}
