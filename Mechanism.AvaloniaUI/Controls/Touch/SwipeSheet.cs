using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace Mechanism.AvaloniaUI.Controls.Touch
{
    public class SwipeSheet : ContentControl
    {
        public static readonly StyledProperty<double> SwipeDistanceProperty =
            AvaloniaProperty.Register<SwipeSheet, double>(nameof(SwipeDistance), defaultValue: 0.0);

        public double SwipeDistance
        {
            get => GetValue(SwipeDistanceProperty);
            protected set => SetValue(SwipeDistanceProperty, value);
        }

        /*protected static readonly StyledProperty<double> AnimatedSwipeDistanceProperty =
            AvaloniaProperty.Register<SwipeSheet, double>(nameof(AnimatedSwipeDistance), defaultValue: 0.0);

        protected double AnimatedSwipeDistance
        {
            get => GetValue(AnimatedSwipeDistanceProperty);
            set => SetValue(AnimatedSwipeDistanceProperty, value);
        }*/

        public static readonly StyledProperty<bool> IsSheetVisibleProperty =
            AvaloniaProperty.Register<SwipeSheet, bool>(nameof(IsSheetVisible), defaultValue: false);

        public bool IsSheetVisible
        {
            get => GetValue(IsSheetVisibleProperty);
            set => SetValue(IsSheetVisibleProperty, value);
        }

        public static readonly StyledProperty<bool> AnimatedIsSheetVisibleProperty =
            AvaloniaProperty.Register<SwipeSheet, bool>(nameof(AnimatedIsSheetVisible), defaultValue: false);

        protected bool AnimatedIsSheetVisible
        {
            get => GetValue(AnimatedIsSheetVisibleProperty);
            set => SetValue(AnimatedIsSheetVisibleProperty, value);
        }


        static SwipeSheet()
        {
            SwipeDistanceProperty.Changed.AddClassHandler<SwipeSheet>(new Action<SwipeSheet, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                sender.UpdateSheet();
                Debug.WriteLine("SwipeDistance: " + sender.SwipeDistance.ToString());
            }));

            IsSheetVisibleProperty.Changed.AddClassHandler<SwipeSheet>(new Action<SwipeSheet, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                if (sender.IsSheetVisible)
                    sender.ShowSheet(sender.Bounds.Height);
                else
                    sender.HideSheet();
            }));
        }


        ContentControl _contentSheet;
        Thumb _swipeThumb;
        Button _cancelButton;
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);

            _contentSheet = e.NameScope.Find<ContentControl>("PART_ContentSheet");
            
            _swipeThumb = e.NameScope.Find<Thumb>("PART_SwipeThumb");
            _swipeThumb.DragStarted += SwipeThumb_DragStarted;
            _swipeThumb.DragDelta += SwipeThumb_DragDelta;
            _swipeThumb.DragCompleted += SwipeThumb_DragCompleted;

            _cancelButton = e.NameScope.Find<Button>("PART_CancelButton");
            _cancelButton.Click += CancelButton_Click;

            SwipeDistance = 0;
        }

        bool _isThumbPressed = false;
        double _xOffset = 0;
        double _yOffset = 0;
        private void SwipeThumb_DragStarted(object sender, Avalonia.Input.VectorEventArgs e)
        {
            _isThumbPressed = true;
            _xOffset = e.Vector.X;
            _yOffset = e.Vector.Y;
            AnimatedIsSheetVisible = true;
        }

        private void SwipeThumb_DragDelta(object sender, Avalonia.Input.VectorEventArgs e)
        {
            double newDistance = e.Vector.Y - _yOffset;
            //SwipeDistance = Math.Max(Math.Min(Bounds.Height * -1, e.Vector.Y), Bounds.Height);
            Debug.WriteLine("Delta Y: " + newDistance);
            SwipeDistance = newDistance;
        }

        private void SwipeThumb_DragCompleted(object sender, Avalonia.Input.VectorEventArgs e)
        {
            _isThumbPressed = false;
            double distance = Math.Abs(Math.Min(Bounds.Height, Math.Max(0, Math.Abs(SwipeDistance))));

            //double finalDistance = 0;
            SwipeDistance = distance;
            if (distance > (Bounds.Height / 2))
            {
                IsSheetVisible = true;
            }
            else
            {
                IsSheetVisible = false;
            }
        }

        private void CancelButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SwipeDistance = 0;
            IsSheetVisible = false;
        }


        protected override Size MeasureCore(Size availableSize)
        {
            UpdateSheet();
            return base.MeasureCore(availableSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateSheet();
            return base.MeasureOverride(availableSize);
        }

        /*public override void Render(DrawingContext context)
        {
            if (_swipeThumb.IsPointerOver)
                AnimatedSwipeDistance = SwipeDistance;
            else if (Math.Round(SwipeDistance, 2) != Math.Round(AnimatedSwipeDistance, 2))
                AnimatedSwipeDistance += (SwipeDistance - AnimatedSwipeDistance) / 4;

            base.Render(context);
        }*/

        protected void UpdateSheet()
        {
            if (_contentSheet != null)
            {
                double distance = Math.Abs(Math.Min(Bounds.Height, Math.Max(0, Math.Abs(SwipeDistance))));
                /*if (distance > _swipeThumb.Bounds.Height)
                    IsSheetVisible = true;*/

                (_contentSheet.RenderTransform as TranslateTransform).Y = Bounds.Height - distance;

                /*if (distance <= _swipeThumb.Bounds.Height)
                    IsSheetVisible = false;*/
            }
        }

        void ShowSheet(double finalDistance)
        {
            AnimatedIsSheetVisible = true;
            Timer timer = new Timer(1);
            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (Math.Round(SwipeDistance, 2) != Math.Round(finalDistance, 2))
                        SwipeDistance += (finalDistance - SwipeDistance) / 4;
                    else
                        timer.Stop();
                });
            };
            timer.Start();
        }

        void HideSheet()
        {
            Timer timer = new Timer(1);
            timer.Elapsed += (sneder, args) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (Math.Round(SwipeDistance, 2) != 0)
                        SwipeDistance -= SwipeDistance / 4;
                    else
                    {
                        AnimatedIsSheetVisible = false;
                        timer.Stop();
                    }
                });
            };
            timer.Start();
        }
    }
}
