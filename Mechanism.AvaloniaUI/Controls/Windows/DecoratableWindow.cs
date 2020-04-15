using Avalonia;
using Avalonia.Rendering;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using DrawingContext = Avalonia.Media.DrawingContext;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Media;
using Mechanism.AvaloniaUI.Core;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using DrawImage = System.Drawing.Image;
using DrawBitmap = System.Drawing.Bitmap;
using AvBitmap = Avalonia.Media.Imaging.Bitmap;
using System.Text;
using DrawSize = System.Drawing.Size;
using DrawPoint = System.Drawing.Point;
using System.ComponentModel;
using System.Timers;
using System.Runtime.InteropServices;

namespace Mechanism.AvaloniaUI.Controls.Windows
{
    public class DecoratableWindow : Window, IStyleable
    {
        static DecoratableWindow()
        {
            //DecoratableWindow.HasSystemDecorationsProperty.OverrideDefaultValue<DecoratableWindow>(false);
            UseBlurProperty.Changed.AddClassHandler<DecoratableWindow>(new Action<DecoratableWindow, AvaloniaPropertyChangedEventArgs>((sender, args) =>
            {
                sender.UpdateComposition();
            }));
        }

        public IDecoratableWindowImpl Impl = null;
        
        public DecoratableWindow()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Impl = new WindowsDecoratableWindowImpl()
                {
                    Window = this
                };
            else //TODO: Proper support for Linux and (if possible) macOS
                Impl = new DefaultDecoratableWindowImpl()
                {
                    Window = this
                };
            //else
            CanBlur = Impl.GetCanBlur();
            Impl.CanBlurChanged += (sneder, args) => CanBlur = Impl.GetCanBlur();
            HasSystemDecorations = false;
            UpdateComposition();
        }

        
        public static readonly StyledProperty<bool> UseBlurProperty =
        AvaloniaProperty.Register<DecoratableWindow, bool>(nameof(UseBlur), defaultValue: false);

        public bool UseBlur
        {
            get => GetValue(UseBlurProperty);
            set => SetValue(UseBlurProperty, value);
        }

        public static readonly StyledProperty<bool> CanBlurProperty =
        AvaloniaProperty.Register<DecoratableWindow, bool>(nameof(CanBlur), defaultValue: false);

        public bool CanBlur
        {
            get => GetValue(CanBlurProperty);
            private set => SetValue(CanBlurProperty, value);
        }

        public static readonly StyledProperty<bool> ClipByAlphaProperty =
        AvaloniaProperty.Register<DecoratableWindow, bool>(nameof(ClipByAlpha), defaultValue: false);

        public bool ClipByAlpha
        {
            get { return GetValue(ClipByAlphaProperty); }
            set { SetValue(ClipByAlphaProperty, value); }
        }

        public static readonly StyledProperty<double> AlphaThresholdProperty =
        AvaloniaProperty.Register<DecoratableWindow, double>(nameof(AlphaThreshold), defaultValue: 0.0);

        public double AlphaThreshold
        {
            get { return GetValue(AlphaThresholdProperty); }
            set { SetValue(AlphaThresholdProperty, value); }
        }

        public static readonly StyledProperty<bool> ShowTitleProperty =
        AvaloniaProperty.Register<DecoratableWindow, bool>(nameof(ShowTitle), defaultValue: true);

        public bool ShowTitle
        {
            get { return GetValue(ShowTitleProperty); }
            set { SetValue(ShowTitleProperty, value); }
        }

        public static readonly StyledProperty<bool> IsWindowVisibleProperty =
        AvaloniaProperty.Register<DecoratableWindow, bool>(nameof(IsWindowVisible), defaultValue: false);

        public bool IsWindowVisible
        {
            get { return GetValue(IsWindowVisibleProperty); }
            set { SetValue(IsWindowVisibleProperty, value); }
        }

        public static readonly StyledProperty<TimeSpan> HideTransitionDurationProperty =
        AvaloniaProperty.Register<DecoratableWindow, TimeSpan>(nameof(HideTransitionDuration), defaultValue: TimeSpan.FromMilliseconds(0));

        public TimeSpan HideTransitionDuration
        {
            get { return GetValue(HideTransitionDurationProperty); }
            set { SetValue(HideTransitionDurationProperty, value); }
        }

        public static readonly StyledProperty<double> TitlebarHeightProperty =
        AvaloniaProperty.Register<DecoratableWindow, double>(nameof(TitlebarHeight), defaultValue: 0.0);

        public double TitlebarHeight
        {
            get { return GetValue(TitlebarHeightProperty); }
            set { SetValue(TitlebarHeightProperty, value); }
        }

        Type IStyleable.StyleKey => typeof(DecoratableWindow);


        void SetupSide(string name, StandardCursorType cursor, WindowEdge edge, ref TemplateAppliedEventArgs e)
        {
            var control = e.NameScope.Get<Control>(name);
            control.Cursor = new Cursor(cursor);
            control.PointerPressed += (sender, ep) => BeginResizeDrag(edge, ep);
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            try
            {
                var titleBar = e.NameScope.Get<Control>("PART_Titlebar");

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    titleBar.DoubleTapped += (sneder, args) => WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

                titleBar.PointerPressed += (sneder, args) => BeginMoveDrag(args);
            }
            catch (KeyNotFoundException ex)
            {

            }

            try
            {
                e.NameScope.Get<Button>("PART_MinimizeButton").Click += (sneder, args) => WindowState = WindowState.Minimized;
                e.NameScope.Get<Button>("PART_MaximizeButton").Click += (sneder, args) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                e.NameScope.Get<Button>("PART_CloseButton").Click += (sneder, args) => Close();
            }
            catch (KeyNotFoundException ex)
            {

            }

            try
            {
                SetupSide("PART_TopLeftEdge", StandardCursorType.TopLeftCorner, WindowEdge.NorthWest, ref e);
                SetupSide("PART_TopCenterEdge", StandardCursorType.TopSide, WindowEdge.North, ref e);
                SetupSide("PART_TopRightEdge", StandardCursorType.TopRightCorner, WindowEdge.NorthEast, ref e);
                SetupSide("PART_MiddleRightEdge", StandardCursorType.RightSide, WindowEdge.East, ref e);
                SetupSide("PART_BottomRightEdge", StandardCursorType.BottomRightCorner, WindowEdge.SouthEast, ref e);
                SetupSide("PART_BottomCenterEdge", StandardCursorType.BottomSide, WindowEdge.South, ref e);
                SetupSide("PART_BottomLeftEdge", StandardCursorType.BottomLeftCorner, WindowEdge.SouthWest, ref e);
                SetupSide("PART_MiddleLeftEdge", StandardCursorType.LeftSide, WindowEdge.West, ref e);
            }
            catch (Exception ex)
            {

            }
        }

        /*public override void Show()
        {
            base.Show();
            IsOpen = true;
        }

        public override void Hide()
        {
            IsOpen = false;
            base.Hide();
        }*/

        bool _isClosingNow = false;
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            bool animate = false;

            if (!(e.Cancel) & !_isClosingNow)
            {
                e.Cancel = true;
                _isClosingNow = true;
                animate = true;
                //Debug.WriteLine("doing the thing");
            }

            if (animate == true)
            {
                IsWindowVisible = false;
                //int interval = 0;
                bool close = false;
                double time = 1;
                if (HideTransitionDuration.TotalMilliseconds > time)
                    time = HideTransitionDuration.TotalMilliseconds;
                Timer timer = new Timer(time);
                timer.Elapsed += (sneder, args) =>
                {
                    if (!close)
                        close = true;
                    else
                        Dispatcher.UIThread.Post(() => Close());
                };
                /*timer.Elapsed += (sneder, args) =>
                {
                    double mil = 0;
                    Dispatcher.UIThread.Post(() => mil = HideTransitionDuration.TotalMilliseconds);
                    if (interval >= mil)
                    {
                        Dispatcher.UIThread.Post(() => Close());
                        timer.Stop();
                    }
                    else
                    {
                        interval++;
                        //Debug.WriteLine("interval: " + interval);
                    }
                };*/
                timer.Start();
            }
        }

        void UpdateComposition()
        {
            if (Impl != null)
            {
                if (UseBlur && Impl.GetCanBlur())
                    Impl?.SetBlur(UseBlur);
            }
        }
        

        public override void Render(DrawingContext context)
        {
            if (IsVisible && (!IsWindowVisible))
                IsWindowVisible = true;
        }

        /*protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            IsOpen = true;
        }*/
    }
}
