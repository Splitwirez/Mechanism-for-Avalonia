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

namespace Mechanism.AvaloniaUI.Controls
{
    public class ToolBar : ContentControl
    {
        /*public static readonly StyledProperty<int> BandProperty =
        AvaloniaProperty.Register<ToolBar, int>(nameof(Band), defaultValue: 0);*/

        public static readonly AttachedProperty<int> BandProperty =
        AvaloniaProperty.RegisterAttached<ToolBar, IControl, int>(nameof(Band), defaultValue: -1);
        
        public static int GetBand(IControl element)
        {
            return element.GetValue(BandProperty);
        }

        public static void SetBand(IControl element, int value)
        {
            element.SetValue(BandProperty, value);
        }


        public int Band
        {
            get => GetBand(this);
            set => SetBand(this, value);
        }

        /*public static readonly StyledProperty<int> BandIndexProperty =
        AvaloniaProperty.Register<ToolBar, int>(nameof(BandIndex), defaultValue: 0);*/

        public static readonly AttachedProperty<int> BandIndexProperty =
        AvaloniaProperty.RegisterAttached<ToolBar, IControl, int>(nameof(BandIndex), defaultValue: -1);
        
        public static int GetBandIndex(IControl element)
        {
            return element.GetValue(BandIndexProperty);
        }

        public static void SetBandIndex(IControl element, int value)
        {
            element.SetValue(BandIndexProperty, value);
        }

        public int BandIndex
        {
            get => GetBandIndex(this);
            set => SetBandIndex(this, value);
        }


        public static readonly StyledProperty<double> BandLengthProperty =
        AvaloniaProperty.Register<ToolBar, double>(nameof(BandLength), defaultValue: double.NaN);

        public double BandLength
        {
            get => GetValue(BandLengthProperty);
            set => SetValue(BandLengthProperty, value);
        }

        /*public static readonly StyledProperty<double> BaseWidthProperty =
            OverflowFlyoutItemsControl.BaseWidthProperty.AddOwner<ToolBar>();

        public double BaseWidth
        {
            get => GetValue(BaseWidthProperty);
            set => SetValue(BaseWidthProperty, value);
        }*/

        /*public static readonly StyledProperty<bool> UseOverflowProperty =
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
        }*/


        Thumb _gripThumb = null;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _gripThumb = e.NameScope.Find<Thumb>("PART_GripThumb");
            _gripThumb.PointerPressed += GripThumb_PointerPressed;
            _gripThumb.DragStarted += GripThumb_DragStarted;
            _gripThumb.DragDelta += GripThumb_DragDelta;
        }
        
        //Vector _dragInit = Vector.Zero;

        //Point _dragInitPoint = new Point(0, 0);
        //double _drag

        //double _dragInitLocX = -1;
        //double _dragInitLocY = -1;
        Point _dragInitPnt = new Point(0, 0);
        private void GripThumb_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            /*var pnt = e.GetPosition(this);
            var bnd = Bounds.TopLeft;
            _dragInitPnt = new Point(pnt.X - bnd.X, pnt.Y - bnd.Y);*/
            _dragInitPnt = e.GetPosition(this);
            //_dragInitLocX = pos.X;
            //_dragInitLocY = pos.Y;
        }


        
        double _dragInitVecX = -1;
        double _dragInitVecY = -1;
        private void GripThumb_DragStarted(object sender, VectorEventArgs e)
        {
            //_dragInit = e.Vector;
            _dragInitVecX = e.Vector.X;
            _dragInitVecY = e.Vector.Y;
            _localX = e.Vector.X;
            _localY = e.Vector.Y;
        }

        private void GripThumb_DragDelta(object sender, VectorEventArgs e)
        {
            double vecX = e.Vector.X;
            double vecY = e.Vector.Y;

            double totalX = /*_localX + */_dragInitVecX + vecX;
            double totalY = /*_localY + */_dragInitVecY + vecY;
            //Debug.WriteLine("DragDelta: \n  Vector: " + vecX + ", " + vecY + "\n    Total: " + totalX + ", " + totalY);

            var panel = GetParentPanel();
            //Debug.WriteLine("TL: " + panel.Bounds.TopLeft + ", " + Bounds.TopLeft);
            var panelBars = GetSiblings(panel).ToList();
            foreach (ToolBar bar in panelBars.Where(x => double.IsNaN(x.BandLength)))
            {
                bar.Measure(Size.Infinity);
                bar.BandLength = bar.DesiredSize.Width;
                //Debug.WriteLine("LENGTH SET IN BAR: " + bar.BandLength);
            }

            
            if (vecX != 0)
            {
                var prevBar = GetPreviousBar(panelBars);
                var nextBar = GetNextBar(panelBars);
                if (prevBar != null)
                {
                    //Debug.WriteLine("prevBar: " + prevBar.BandIndex + ", " + BandIndex);
                    double newPrevBarWidth = prevBar.BandLength + vecX;
                    var point = new Point(_dragInitPnt.X + Bounds.X, _dragInitPnt.Y + Bounds.Y);
                    if (prevBar.Bounds.Contains(point) && (point.X < prevBar.Bounds.Center.X))
                    {
                        int prevBandIndex = prevBar.BandIndex;
                        prevBar.BandIndex = BandIndex;
                        BandIndex = prevBandIndex;
                    }
                    else if ((nextBar != null) && (nextBar.Bounds.Contains(_dragInitPnt)))
                    {
                        int nextBandIndex = nextBar.BandIndex;
                        nextBar.BandIndex = BandIndex;
                        BandIndex = nextBandIndex;
                    }
                    else if (prevBar.IsValidForBandLength(newPrevBarWidth))
                    {
                        prevBar.BandLength = newPrevBarWidth;

                        double newThisWidth = BandLength - vecX;
                        if (newThisWidth > MinWidth)
                            BandLength = newThisWidth;
                    }
                    /*else if ((Bounds.X - deltaX) < prevBar.Bounds.X)
                    {
                        int prevBandIndex = prevBar.BandIndex;
                        prevBar.BandIndex = BandIndex;
                        BandIndex = prevBandIndex;
                    }*/
                }
            }


            if (vecY != 0)
            {
                //double panelTop = panel.Bounds.Top;
                double band = Band;
                

                double movY = Bounds.Y + _dragInitPnt.Y + vecY;
                Point movPnt = new Point(Bounds.X + _dragInitPnt.X + vecX, movY);
                if (!Bounds.Contains(movPnt))
                {
                    //Debug.WriteLine("movPnt: " + movPnt);
                    var barPointerOver = panelBars.FirstOrDefault(x => x.Bounds.Contains(movPnt));

                    bool isAloneInBand = panelBars.Count(x => x.Band == band) <= 1;
                    if (vecY > 0)
                    {
                        int lastBand = panelBars.Where(x => x != this).Max(x => x.Band);
                        
                        if (movY > Bounds.Bottom)
                        {
                            if ((!isAloneInBand) && (band == lastBand))
                            {
                                Band = lastBand + 1;
                            }
                            else
                            {
                                var subsequentBars = panelBars.Where(x => x.Band > band);

                                if (barPointerOver != null)
                                {
                                    //Debug.WriteLine("Y pos: " + movY + ", " + barPointerOver.Bounds.Y);
                                    double distFromTop = movY - barPointerOver.Bounds.Y;
                                    double distFromBottom = barPointerOver.Bounds.Bottom - movY;
                                    
                                    int poBand = barPointerOver.Band;
                                    IEnumerable<ToolBar> moveDownBars = null;
                                    
                                    if ((distFromTop >= 0) && (distFromTop <= 10))
                                    {
                                        moveDownBars = subsequentBars.Where(x => x.Band >= poBand);
                                    }
                                    else if ((distFromBottom >= 0) && (distFromBottom <= 10))
                                    {
                                        moveDownBars = subsequentBars.Where(x => x.Band > poBand);
                                        //poBand
                                    }

                                    if (moveDownBars != null)
                                    {
                                        foreach (ToolBar sBar in moveDownBars.Where(x => x != this))
                                        {
                                            sBar.Band++;
                                        }
                                    }

                                    int bandIndex = BandIndex;
                                    foreach (ToolBar bar in panelBars.Where((x => (x != barPointerOver) && (x.Band == poBand) && (x.BandIndex > barPointerOver.BandIndex))))
                                    {
                                        bar.BandIndex++;
                                    }
                                    BandIndex = barPointerOver.BandIndex;
                                    barPointerOver.BandIndex = bandIndex;

                                    Band = poBand;
                                }
                            }
                        }
                        //panel.TranslatePoint()
                        //if (vecY)
                    }
                    else if (vecY < 0)
                    {
                        if (movY < Bounds.Top)
                        {
                            if ((!isAloneInBand) && (band == 1))
                            {
                                Band = 0;
                            }
                            else
                            {
                                var subsequentBars = panelBars.Where(x => x.Band < band);

                                if (barPointerOver != null)
                                {
                                    //Debug.WriteLine("Y pos: " + movY + ", " + barPointerOver.Bounds.Y);
                                    double distFromTop = movY - barPointerOver.Bounds.Y;
                                    double distFromBottom = barPointerOver.Bounds.Bottom - movY;
                                    
                                    int poBand = barPointerOver.Band;
                                    IEnumerable<ToolBar> moveDownBars = null;
                                    
                                    if ((distFromTop >= 0) && (distFromTop <= 10))
                                    {
                                        moveDownBars = subsequentBars.Where(x => x.Band >= poBand);
                                    }
                                    else if ((distFromBottom >= 0) && (distFromBottom <= 10))
                                    {
                                        moveDownBars = subsequentBars.Where(x => x.Band >= (poBand + 1));
                                        //poBand
                                    }

                                    if (moveDownBars != null)
                                    {
                                        foreach (ToolBar sBar in moveDownBars.Where(x => x != this))
                                        {
                                            sBar.Band++;
                                        }
                                    }

                                    Band = poBand;
                                }
                            }
                        }
                    }
                }
            }
            //Debug.WriteLine("Band: " + Band);
            /*else if (deltaX > 0)
            {
                var nextBar = GetNextBar(siblings);
                if (nextBar != null)
                {
                    if ((nextBar.Width + deltaX) > nextBar.MinWidth)
                    {
                        nextBar.Width += deltaX;
                        if (Width - deltaX < DesiredSize.Width)
                            Width -= deltaX;
                    }
                    else if ((Bounds.X - deltaX) < nextBar.Bounds.X)
                    {
                    }
                }
            }*/
        }

        ToolBarTrayPanel GetParentPanel() => Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<ToolBarTrayPanel>(this);

        IEnumerable<ToolBar> GetSiblings(ToolBarTrayPanel panel) => panel.Children.OfType<ToolBar>().OrderBy(x => x.Band);

        IEnumerable<ToolBar> GetSharedBandBars(IEnumerable<ToolBar> siblings)
        {
            double band = Band;
            return siblings.Where(x => /*(x != this) && (*/x.Band == band/*)*/).OrderBy(x => x.BandIndex);
        }
            

        ToolBar GetPreviousBar(IEnumerable<ToolBar> shared)
        {
            int thisBandIndex = BandIndex;
            return shared.Take(shared.ToList().IndexOf(this)).LastOrDefault(/*x => x.BandIndex <= thisBandIndex*/);
        }

        ToolBar GetNextBar(IEnumerable<ToolBar> shared)
        {
            int thisBandIndex = BandIndex;
            return shared.Skip(shared.ToList().IndexOf(this) + 1).FirstOrDefault(/*x => x.BandIndex >= thisBandIndex*/);
        }






        double _localX = -1;
        double _localY = -1;
        private void zGripThumb_DragDelta(object sender, VectorEventArgs e)
        {
            double localX = _localX + e.Vector.X;
            double localY = _localY + e.Vector.Y;
            
            ToolBar mouseOverBar = null;
            var parent = Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<ToolBarTrayPanel>(this, false);
            
            var cursorPoint = e.Vector; //_gripThumb.PointToScreen(new Point(localX, localY)).ToPoint(VisualRoot.RenderScaling);
            var items = parent.Children.OfType<ToolBar>();

            int bandCount = 0; //parent.Children.Count();
            if (items.Count() > 0)
                bandCount = items.Max(x => x.Band);
            
            bool leftHalf = false;
            foreach (ToolBar bar in items)
            {
                var barPoint = bar.PointToScreen(new Point(0, 0)).ToPoint(VisualRoot.RenderScaling);
                bool left = barPoint.X <= cursorPoint.X;
                bool top = barPoint.Y <= cursorPoint.Y;
                bool right = (barPoint.X + bar.Bounds.Width) >= cursorPoint.X;
                bool bottom = (barPoint.Y + bar.Bounds.Height) >= cursorPoint.Y;
                leftHalf = (barPoint.X + (bar.Bounds.Width / 2)) >= cursorPoint.X;

                if (left && top && right && bottom)
                {
                    mouseOverBar = bar;
                    break;
                }
            }

            var trayPoint = parent.PointToScreen(new Point(0, 0));
            
            bool aboveTop = trayPoint.Y > cursorPoint.Y;
            //bool right = (trayPoint.X + (parent.Bounds.Width / VisualRoot.RenderScaling)) >= cursorPoint.X;
            bool belowBottom = (trayPoint.Y + (parent.Bounds.Height / VisualRoot.RenderScaling)) < cursorPoint.Y;
            if (belowBottom && (items.Where(x => x.Band == bandCount).Count() > 1)) //(items.Where(x => x.Band == (bandCount - 1)).Count()*/)// && (items.Where(x => x.Band == (bandCount - 1)).Count() <= 1))
            {
                //Band = items.Max(x => x.Band) + 1;
                Band = bandCount + 1;
            }
            else if (aboveTop && (items.Where(x => x.Band == 0).Count() > 1))
            {
                foreach (ToolBar bar in items.Where(x => x != this))
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
                ToolBar targetBar = items.OfType<ToolBar>().Where(x => x.Band == Band).FirstOrDefault(x => x.BandIndex == (BandIndex - 1));
                
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
            parent.InvalidateArrange();
            parent.InvalidateMeasure();
            parent.InvalidateVisual();
            //parent.Measure(parent.Bounds.Size);
            //parent.Arrange(parent.Bounds);
            //ToolbarResized?.Invoke(this, new EventArgs());
            /*else
                Debug.WriteLine("BandIndex == 0");*/
        }

        private void zzGripThumb_DragDelta(object sender, VectorEventArgs e)
        {
            double localX = _localX + e.Vector.X;
            double localY = _localY + e.Vector.Y;
            
            ToolBar mouseOverBar = null;
            var parent = GetParentPanel(); //Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<ToolBarTrayPanel>(this, false);
            
            var cursorPoint = e.Vector; //_gripThumb.PointToScreen(new Point(localX, localY)).ToPoint(VisualRoot.RenderScaling);
            var items = parent.Children.OfType<ToolBar>();

            int bandCount = 0; //parent.Children.Count();
            if (items.Count() > 0)
                bandCount = items.Max(x => x.Band);
            
            bool leftHalf = false;

            foreach (ToolBar bar in items.Where(x => double.IsNaN(x.BandLength)))
            {
                bar.Measure(Size.Infinity);
                bar.BandLength = bar.DesiredSize.Width;
                Debug.WriteLine("LENGTH SET IN BAR: " + bar.BandLength);
            }

            foreach (ToolBar bar in items)
            {
                var barPoint = bar.PointToScreen(new Point(0, 0)).ToPoint(VisualRoot.RenderScaling);
                bool left = barPoint.X <= cursorPoint.X;
                bool top = barPoint.Y <= cursorPoint.Y;
                bool right = (barPoint.X + bar.Bounds.Width) >= cursorPoint.X;
                bool bottom = (barPoint.Y + bar.Bounds.Height) >= cursorPoint.Y;
                leftHalf = (barPoint.X + (bar.Bounds.Width / 2)) >= cursorPoint.X;

                if (left && top && right && bottom)
                {
                    mouseOverBar = bar;
                    break;
                }
            }

            var trayPoint = parent.PointToScreen(new Point(0, 0));
            
            bool aboveTop = trayPoint.Y > cursorPoint.Y;
            //bool right = (trayPoint.X + (parent.Bounds.Width / VisualRoot.RenderScaling)) >= cursorPoint.X;
            bool belowBottom = (trayPoint.Y + (parent.Bounds.Height / VisualRoot.RenderScaling)) < cursorPoint.Y;
            if (belowBottom && (items.Where(x => x.Band == bandCount).Count() > 1)) //(items.Where(x => x.Band == (bandCount - 1)).Count()*/)// && (items.Where(x => x.Band == (bandCount - 1)).Count() <= 1))
            {
                //Band = items.Max(x => x.Band) + 1;
                Band = bandCount + 1;
            }
            else if (aboveTop && (items.Where(x => x.Band == 0).Count() > 1))
            {
                foreach (ToolBar bar in items.Where(x => x != this))
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
                        
                        double width = BandLength;
                        BandLength = mouseOverBar.BandLength;
                        mouseOverBar.BandLength = width;
                    }
                }
                else
                {
                    double width = BandLength;
                    ToolBar adjacent = items.FirstOrDefault(x => (x.Band == Band) && (x.BandIndex == (BandIndex + 1)));
                    if (adjacent == null)
                        adjacent = items.FirstOrDefault(x => (x.Band == Band) && (x.BandIndex == (BandIndex - 1)));
                    if (adjacent != null)
                        adjacent.BandLength += width;
                    BandLength = mouseOverBar.BandLength / 2;
                    mouseOverBar.BandLength /= 2;
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
                ToolBar targetBar = items.OfType<ToolBar>().Where(x => x.Band == Band).FirstOrDefault(x => x.BandIndex == (BandIndex - 1));
                
                if (targetBar != null)
                {
                    double newTargetWidth = targetBar.BandLength + e.Vector.X;
                    double newWidth = BandLength - e.Vector.X;
                    if ((newTargetWidth >= targetBar.MinWidth) && (newWidth >= MinWidth))
                    {
                        targetBar.BandLength = newTargetWidth; //+= e.Vector.X;
                        BandLength = newWidth; //-= e.Vector.X;
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
            //parent.InvalidateArrange();
            //parent.InvalidateMeasure();
            //parent.InvalidateVisual();
            //parent.Measure(parent.Bounds.Size);
            //parent.Arrange(parent.Bounds);
            //ToolbarResized?.Invoke(this, new EventArgs());
            /*else
                Debug.WriteLine("BandIndex == 0");*/
        }

        /*protected override Size MeasureCore(Size availableSize)
        {
            var baseSize = base.MeasureCore(availableSize);
            
            var items = Items.OfType<Control>();
            
            if (items.Count() > 0)
            {
                var visibleItems = items.Where(x => x.IsVisible);
                if (visibleItems.Count() > 0)
                    MinWidth = BaseWidth + visibleItems.ElementAt(0).DesiredSize.Width;
                else
                    MinWidth = BaseWidth;
            }
            return baseSize;
        }*/

        /*protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(0, base.MeasureOverride(availableSize).Height);
        }*/

        static ToolBar()
        {
            /*BandProperty.OverrideDefaultValue<ToolBar>(0);
            BandIndexProperty.OverrideDefaultValue<ToolBar>(0);*/
            //WidthProperty.OverrideDefaultValue<ToolBar>(100);
            AffectsArrange<ToolBar>(BandProperty, BandIndexProperty, BandLengthProperty);
            AffectsMeasure<ToolBar>(BandProperty, BandIndexProperty, BandLengthProperty);
            AffectsRender<ToolBar>(BandProperty, BandIndexProperty, BandLengthProperty);

            BandProperty.Changed.AddClassHandler<ToolBar>(BarPropertiesChanged);
            BandIndexProperty.Changed.AddClassHandler<ToolBar>(BarPropertiesChanged);
            BandLengthProperty.Changed.AddClassHandler<ToolBar>(BarPropertiesChanged);

            ContentProperty.Changed.AddClassHandler<ToolBar>((x, e) =>
            {
                if ((e.NewValue != null) && (e.NewValue is IControl ctrl))
                {
                    int band = ToolBar.GetBand(ctrl);
                    if (band > -1)
                        x.Band = band;

                    int bandIndex = ToolBar.GetBandIndex(ctrl);
                    if (bandIndex > -1)
                        x.BandIndex = bandIndex;
                    
                    ctrl.Measure(Size.Infinity);
                    x.BandLength = Math.Max(x.BandLength, ctrl.DesiredSize.Width);
                }
            });
        }
        
        static void BarPropertiesChanged(ToolBar bar, AvaloniaPropertyChangedEventArgs e)
        {
            var panel = bar.GetParentPanel();
            if (panel != null)
            {
                panel.InvalidateArrange();
                panel.InvalidateMeasure();
                panel.InvalidateVisual();
            }
        }

        public ToolBar()
        {
            /*if (double.IsNaN(Width))
                Width = DesiredSize.Width;*/
            if ((Content != null) && (Content is IControl ctrl) && GetBand(ctrl) > -1)
                Band = GetBand(ctrl);
            else if (Band < 0)
                Band = 0;
            if ((Content != null) && (Content is IControl ctrl2) && GetBandIndex(ctrl2) > -1)
                BandIndex = GetBandIndex(ctrl2);
            else if (BandIndex < 0)
                BandIndex = 0;
        }

        //public static event EventHandler<EventArgs> ToolbarResized;

        internal bool IsValidForBandLength(double newLength)
        {
            bool isLargeEnough = newLength >= MinWidth;
            bool isSmallEnough = true;
            /*if (Content is IControl vis)
            {
                double desiredWidth = vis.DesiredSize.Width;
                if (desiredWidth <= MinWidth)
                    return false;
                else
                    isSmallEnough = newLength <= desiredWidth;
            }*/

            return isLargeEnough && isSmallEnough;
        }
    }
}
