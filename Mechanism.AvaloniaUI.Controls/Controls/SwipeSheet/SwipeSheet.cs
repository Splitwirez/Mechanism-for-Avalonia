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
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using System.Diagnostics;
using Avalonia.Media;
using Avalonia.Input;

namespace Mechanism.AvaloniaUI.Controls.SwipeSheet
{
    public class SwipeSheet : HeaderedContentControl
    {
        public static readonly StyledProperty<Dock> SwipeFromEdgeProperty =
        AvaloniaProperty.Register<SwipeSheet, Dock>(nameof(SwipeFromEdge), defaultValue: Dock.Bottom);

        public Dock SwipeFromEdge
        {
            get => GetValue(SwipeFromEdgeProperty);
            set => SetValue(SwipeFromEdgeProperty, value);
        }
        
        
        public static readonly StyledProperty<RelativePoint> SwipeToOpenThresholdProperty =
        AvaloniaProperty.Register<SwipeSheet, RelativePoint>(nameof(SwipeToOpenThreshold), defaultValue: new RelativePoint(0.5, 0.5, RelativeUnit.Relative));

        public RelativePoint SwipeToOpenThreshold
        {
            get => GetValue(SwipeToOpenThresholdProperty);
            set => SetValue(SwipeToOpenThresholdProperty, value);
        }


        private bool _isOpen;
        /// <summary>
        /// Defines the <see cref="IsOpen"/> property.
        /// </summary>
        public static readonly DirectProperty<SwipeSheet, bool> IsOpenProperty = Popup.IsOpenProperty.AddOwner<SwipeSheet>(
            o => o.IsOpen,
            (o, v) => o.IsOpen = v);
        

        /// <summary>
        /// Gets or sets a value indicating whether the SwipeSheet is currently open.
        /// </summary>
        public bool IsOpen
        {
            get => _isOpen;
            set => SetAndRaise(IsOpenProperty, ref _isOpen, value);
        }


        Thumb _swipeThumb = null;
        Thumb _swipePanelThumb = null;
        ContentControl _contentArea = null;

        //TranslateTransform _transform = new TranslateTransform(0, -10);
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _contentArea = e.NameScope.Find<ContentControl>("PART_ContentArea");
            _contentArea.IsVisible = false;
            Debug.WriteLine("Margin: " + _contentArea.Margin);
            //_contentArea.LayoutTransform = _transform;

            _swipeThumb = e.NameScope.Find<Thumb>("PART_SwipeEdge");
            _swipeThumb.DragStarted += Thumb_DragStarted;
            _swipeThumb.DragDelta += Thumb_DragDelta;
            _swipeThumb.DragCompleted += Thumb_DragCompleted;

            _swipePanelThumb = e.NameScope.Find<Thumb>("PART_SwipePanelThumb");
            _swipePanelThumb.DragStarted += Thumb_DragStarted;
            _swipePanelThumb.DragDelta += Thumb_DragDelta;
            _swipePanelThumb.DragCompleted += Thumb_DragCompleted;
        }

        void Thumb_DragStarted(object sender, VectorEventArgs e)
        {
            _contentArea.IsVisible = true;
            _contentArea.Margin = GetMarginForOffset(GetSwipeOffset(Vector.Zero, SwipeFromEdge), SwipeFromEdge);
        }

        void Thumb_DragDelta(object sender, VectorEventArgs e)
        {
            Debug.WriteLine("DragDelta: \n  " + e.Vector + "\n   " + _contentArea.Margin); //: " + (_contentArea.LayoutTransform as TranslateTransform).X + ", " + (_contentArea.LayoutTransform as TranslateTransform).Y);

            _contentArea.Margin =  GetMarginForOffset(GetSwipeOffset(e.Vector, SwipeFromEdge), SwipeFromEdge);
        }

        void Thumb_DragCompleted(object sender, VectorEventArgs e)
        {
            if (
                    ((SwipeToOpenThreshold.Unit == RelativeUnit.Absolute) && (Math.Abs(_contentArea.Margin.Bottom) < SwipeToOpenThreshold.Point.Y)) ||
                    ((SwipeToOpenThreshold.Unit == RelativeUnit.Relative) && (Math.Abs(_contentArea.Margin.Bottom) < (SwipeToOpenThreshold.Point.Y * Bounds.Height)))
                )
            {
                _contentArea.Margin = new Thickness(0);
            }
            else
            {
                _contentArea.IsVisible = false;
            }
            /*double threshold = 0;
            double movement = 0;
            if (IsHorizontal)
            {
                movement = _transform.X;

                if (SwipeToOpenThreshold.Unit == RelativeUnit.Absolute)
                    threshold = SwipeToOpenThreshold.Point.X;
                else
                    threshold = Bounds.Width * SwipeToOpenThreshold.Point.X;
                
                if (SwipeFromEdge == Dock.Right)
                {
                    threshold = Bounds.Width - threshold;
                    movement = Bounds.Width - _transform.X;
                }
            }
            else
            {
                if (SwipeToOpenThreshold.Unit == RelativeUnit.Absolute)
                    threshold = SwipeToOpenThreshold.Point.Y;
                else
                    threshold = Bounds.Height * SwipeToOpenThreshold.Point.Y;
                
                if (SwipeFromEdge == Dock.Bottom)
                {
                    threshold = Bounds.Height - threshold;
                    movement = Bounds.Height - _transform.Y;
                }
            }

            if (movement < threshold)
            {
                if (SwipeFromEdge == Dock.Left)
                {
                    _transform.X = -Bounds.Width;
                }
                else if (SwipeFromEdge == Dock.Top)
                {
                    _transform.Y = -Bounds.Height;
                }
                else if (SwipeFromEdge == Dock.Right)
                {
                    _transform.X = 0;
                }
                else
                {

                }
            }
            else
            {
                if (SwipeFromEdge == Dock.Left)
                {

                }
                else if (SwipeFromEdge == Dock.Top)
                {
                    
                }
                else if (SwipeFromEdge == Dock.Right)
                {
                    
                }
                else
                {

                }
            }*/
        }

        bool IsHorizontal => (SwipeFromEdge == Dock.Left) || (SwipeFromEdge == Dock.Right);

        protected double GetSwipeOffset(Vector vector, Dock edge)
        {
            if ((edge == Dock.Left) || (edge == Dock.Right))
                return GetSwipeOffset(vector.X, edge);
            else
                return GetSwipeOffset(vector.Y, edge);
        }

        protected double GetSwipeOffset(double distance, Dock edge)
        {
            double offset = 0;
            if (edge == Dock.Left)
                offset = Math.Max(0, distance) + Bounds.Width;
            else if (edge == Dock.Top)
                offset = Math.Max(0, distance) + Bounds.Height;
            else if (edge == Dock.Right)
                offset = Math.Max(0, distance) + Bounds.Width;
            else //Bottom
                offset = Math.Min(0, distance) + Bounds.Height;
            
            return offset;
        }

        protected double GetSwipeOffset(Thumb  thumb, Vector vector, Dock edge)
        {
            if ((edge == Dock.Left) || (edge == Dock.Right))
                return GetSwipeOffset(thumb, vector.X, edge);
            else
                return GetSwipeOffset(thumb, vector.Y, edge);
        }

        protected double GetSwipeOffset(Thumb thumb, double distance, Dock edge)
        {
            double offset = 0;
            if (edge == Dock.Left)
                offset = Math.Max(0, distance) + (Bounds.Right - thumb.Bounds.Right);
            else if (edge == Dock.Top)
                offset = Math.Max(0, distance) + (Bounds.Bottom - thumb.Bounds.Bottom);
            else if (edge == Dock.Right)
                offset = Math.Max(0, distance) + (Bounds.Right - thumb.Bounds.Right);
            else //Bottom
                offset = Math.Min(0, distance) + (Bounds.Bottom - thumb.Bounds.Bottom);
            
            return offset;
        }

        protected Thickness GetMarginForOffset(double offset, Dock edge)
        {
            if (edge == Dock.Bottom)
                return new Thickness(0, offset, 0, -offset);
            else
                return new Thickness(offset, 0, -offset, 0);
        }
    }


    public class RelativeDouble
    {
    }
}