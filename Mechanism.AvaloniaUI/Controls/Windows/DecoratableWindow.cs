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

namespace Mechanism.AvaloniaUI.Controls.Windows
{
    public class DecoratableWindow : Window, IStyleable
    {
        static DecoratableWindow()
        {
            //DecoratableWindow.HasSystemDecorationsProperty.OverrideDefaultValue<DecoratableWindow>(false);
            UseBlurProperty.Changed.AddClassHandler<DecoratableWindow>(new Action<DecoratableWindow, AvaloniaPropertyChangedEventArgs>((sender, args) =>
            {
                bool newValue = (bool)args.NewValue;
                if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (sender.PlatformImpl != null))
                {
                    NativeMethodsWindows.DWM_BLURBEHIND blurInfo;
                    if (newValue)
                        blurInfo = new NativeMethodsWindows.DWM_BLURBEHIND()
                        {
                            dwFlags = NativeMethodsWindows.DWM_BB.Enable | NativeMethodsWindows.DWM_BB.BlurRegion | NativeMethodsWindows.DWM_BB.TransitionMaximized,
                            fEnable = true,
                            hRgnBlur = IntPtr.Zero,
                            fTransitionOnMaximized = true
                        };
                    else
                        blurInfo = new NativeMethodsWindows.DWM_BLURBEHIND()
                        {
                            //dwFlags = NativeMethodsWindows.DWM_BB.Enable | NativeMethodsWindows.DWM_BB.BlurRegion | NativeMethodsWindows.DWM_BB.TransitionMaximized,
                            fEnable = false,
                            //hRgnBlur = IntPtr.Zero,
                            fTransitionOnMaximized = true
                        };

                    NativeMethodsWindows.DwmEnableBlurBehindWindow(sender.PlatformImpl.Handle.Handle, ref blurInfo);
                }
            }));

            /*ClipByAlphaProperty.Changed.AddClassHandler<DecoratableWindow>(new Action<DecoratableWindow, AvaloniaPropertyChangedEventArgs>((sender, args) =>
            {
                if ((!(bool)args.NewValue) && (Environment.OSVersion.Platform == PlatformID.Win32NT))
                    NativeMethodsWindows.SetWindowRgn(sender.PlatformImpl.Handle.Handle, IntPtr.Zero, true);
            }));*/
        }

        static void AddAlphaHandler()
        {
            ClipByAlphaProperty.Changed.AddClassHandler<DecoratableWindow>(new Action<DecoratableWindow, AvaloniaPropertyChangedEventArgs>((sender, args) =>
            {
                bool newValue = (bool)args.NewValue;
                if ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (sender.PlatformImpl != null))
                {
                    IntPtr handle = sender.PlatformImpl.Handle.Handle;
                    int oldExStyle = NativeMethodsWindows.GetWindowLongInt(handle, NativeMethodsWindows.GwlExstyle);
                    if (newValue)
                    {
                        RenderTargetBitmap bmp = new RenderTargetBitmap(new PixelSize((int)sender.PlatformImpl.ClientSize.Width, (int)sender.PlatformImpl.ClientSize.Height));
                        bmp.Render(sender.Content as IVisual);
                        NativeMethodsWindows.SetWindowLong(handle, NativeMethodsWindows.GwlExstyle, oldExStyle ^ NativeMethodsWindows.WsExLayered/* | NativeMethodsWindows.WsExTransparent*/);
                        //NativeMethodsWindows.SetLayeredWindowAttributes(handle, 0, 255, NativeMethodsWindows.LwaAlpha);
                        Action<Rect> oldPaint = sender.PlatformImpl.Paint;
                        sender.PlatformImpl.Paint += new Action<Rect>((rect) =>
                        //if (false)
                        {
                            //NativeMethodsWindows.SetLayeredWindowAttributes(handle, 0, 255, NativeMethodsWindows.LwaColorKey);
                            DrawSize size = new DrawSize((int)sender.PlatformImpl.ClientSize.Width, (int)sender.PlatformImpl.ClientSize.Height);
                            sender.Renderer.Paint(new Rect(0, 0, size.Width, size.Height));
                            //NativeMethodsWindows.SetLayeredWindowAttributes(handle, 0, 255, NativeMethodsWindows.LwaAlpha);
                            //AvBitmap bmp = new AvBitmap(PixelFormat.Bgra8888, IntPtr.Zero, new PixelSize((int)sender.PlatformImpl.ClientSize.Width, (int)sender.PlatformImpl.ClientSize.Height), new Vector(96, 96), 4);
                            //ImmediateRenderer.Render(this, bmp.)

                            DrawPoint pointSource = new DrawPoint(sender.PlatformImpl.Position.X, sender.PlatformImpl.Position.Y);
                            /*IntPtr screenDc = NativeMethodsWindows.GetDC(IntPtr.Zero);
                            IntPtr memDc = NativeMethodsWindows.CreateCompatibleDC(screenDc);
                            IntPtr winDc = NativeMethodsWindows.GetWindowDC(sender.PlatformImpl.Handle.Handle);
                            bool success = NativeMethodsWindows.BitBlt(memDc, 0, 0, size.Width, size.Height, winDc, 0, 0, 0x00CC0020 | 0x40000000);*/
                            //IntPtr hBitmap = bitmap.GetHbitmap(System.Drawing.Color.FromArgb(0));
                            //oldBitmap = Win32.SelectObject(memDc, hBitmap);

                            //IntPtr desktophWnd;
                            IntPtr desktopDc;
                            IntPtr memoryDc;
                            IntPtr bitmap;
                            IntPtr oldBitmap;
                            bool success;
                            //DrawBitmap result;
                            Rectangle region = new Rectangle(0, 0, size.Width, size.Height);
                            //desktophWnd = sender.PlatformImpl.Handle.Handle; //NativeMethodsWindows.GetDesktopWindow();
                            desktopDc = NativeMethodsWindows.GetDC(IntPtr.Zero);
                            memoryDc = NativeMethodsWindows.CreateCompatibleDC(desktopDc);
                            bitmap = NativeMethodsWindows.CreateCompatibleBitmap(desktopDc, region.Width, region.Height);
                            oldBitmap = NativeMethodsWindows.SelectObject(memoryDc, bitmap);

                            success = NativeMethodsWindows.BitBlt(memoryDc, 0, 0, region.Width, region.Height, desktopDc, region.Left, region.Top, NativeMethodsWindows.SrcCopy | NativeMethodsWindows.CaptureBlt);

                            /*System.Drawing.Point pointSource = new System.Drawing.Point(sender.PlatformImpl.Position.X, sender.PlatformImpl.Position.Y);
                            System.Drawing.Point topPos = new System.Drawing.Point(0, 0);*/
                            NativeMethodsWindows.BLENDFUNCTION blend = new NativeMethodsWindows.BLENDFUNCTION();
                            blend.BlendOp = 0;
                            blend.BlendFlags = 0;
                            blend.SourceConstantAlpha = 255;
                            blend.AlphaFormat = 1;
                            DrawPoint topPos = new DrawPoint(0, 0);
                            NativeMethodsWindows.UpdateLayeredWindow(sender.PlatformImpl.Handle.Handle, desktopDc, ref topPos, ref size, memoryDc, ref pointSource, 0, ref blend, 2);

                            NativeMethodsWindows.SelectObject(memoryDc, oldBitmap);
                            NativeMethodsWindows.DeleteObject(bitmap);
                            NativeMethodsWindows.DeleteDC(memoryDc);
                            NativeMethodsWindows.ReleaseDC(IntPtr.Zero, desktopDc);
                        });
                    }
                    else
                    {
                        NativeMethodsWindows.SetWindowLong(handle, NativeMethodsWindows.GwlExstyle, oldExStyle ^ (~NativeMethodsWindows.WsExLayered));
                    }
                }
            }));
        }
        
        public DecoratableWindow()
        {
            HasSystemDecorations = false;
        }

        
        public static readonly StyledProperty<bool> UseBlurProperty =
        AvaloniaProperty.Register<DecoratableWindow, bool>(nameof(UseBlur), defaultValue: false);

        public bool UseBlur
        {
            get { return GetValue(UseBlurProperty); }
            set { SetValue(UseBlurProperty, value); }
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

        bool _rendering = false;
        public void yRender(DrawingContext context)
        {
            base.Render(context);
            if (IsVisible && (!IsWindowVisible))
                IsWindowVisible = true;
        }

        public void zzRender(DrawingContext context)
        {
            IntPtr handle = PlatformImpl.Handle.Handle;
            //Renderer.Paint(new Rect(0, 0, Width, Height));

            DrawSize size = new DrawSize((int)PlatformImpl.ClientSize.Width, (int)PlatformImpl.ClientSize.Height);
            DrawPoint pointSource = new DrawPoint(PlatformImpl.Position.X, PlatformImpl.Position.Y);
            /*IntPtr screenDc = NativeMethodsWindows.GetDC(IntPtr.Zero);
            IntPtr memDc = NativeMethodsWindows.CreateCompatibleDC(screenDc);
            IntPtr winDc = NativeMethodsWindows.GetWindowDC(PlatformImpl.Handle.Handle);
            bool success = NativeMethodsWindows.BitBlt(memDc, 0, 0, size.Width, size.Height, winDc, 0, 0, 0x00CC0020 | 0x40000000);*/
            //IntPtr hBitmap = bitmap.GetHbitmap(System.Drawing.Color.FromArgb(0));
            //oldBitmap = Win32.SelectObject(memDc, hBitmap);

            //IntPtr desktophWnd;
            IntPtr desktopDc;
            IntPtr memoryDc;
            IntPtr bitmap;
            IntPtr oldBitmap;
            bool success;
            //DrawBitmap result;
            Rectangle region = new Rectangle(0, 0, size.Width, size.Height);
            //desktophWnd = PlatformImpl.Handle.Handle; //NativeMethodsWindows.GetDesktopWindow();
            desktopDc = NativeMethodsWindows.GetDC(IntPtr.Zero);
            memoryDc = NativeMethodsWindows.CreateCompatibleDC(desktopDc);
            bitmap = NativeMethodsWindows.CreateCompatibleBitmap(desktopDc, region.Width, region.Height);
            oldBitmap = NativeMethodsWindows.SelectObject(memoryDc, bitmap);

            success = NativeMethodsWindows.BitBlt(memoryDc, 0, 0, region.Width, region.Height, desktopDc, region.Left, region.Top, NativeMethodsWindows.SrcCopy | NativeMethodsWindows.CaptureBlt);

            /*System.Drawing.Point pointSource = new System.Drawing.Point(PlatformImpl.Position.X, PlatformImpl.Position.Y);
            System.Drawing.Point topPos = new System.Drawing.Point(0, 0);*/
            NativeMethodsWindows.BLENDFUNCTION blend = new NativeMethodsWindows.BLENDFUNCTION();
            blend.BlendOp = 0;
            blend.BlendFlags = 0;
            blend.SourceConstantAlpha = 255;
            blend.AlphaFormat = 1;
            DrawPoint topPos = new DrawPoint(0, 0);
            //NativeMethodsWindows.UpdateLayeredWindow(PlatformImpl.Handle.Handle, desktopDc, ref topPos, ref size, memoryDc, ref pointSource, 0, ref blend, 2);

            NativeMethodsWindows.SelectObject(memoryDc, oldBitmap);
            NativeMethodsWindows.DeleteObject(bitmap);
            NativeMethodsWindows.DeleteDC(memoryDc);
            NativeMethodsWindows.ReleaseDC(IntPtr.Zero, desktopDc);

            //NativeMethodsWindows.SetLayeredWindowAttributes(handle, 0, 255, NativeMethodsWindows.LwaColorKey);
            
            //NativeMethodsWindows.SetLayeredWindowAttributes(handle, 0, 255, NativeMethodsWindows.LwaAlpha);
            if (IsVisible && (!IsWindowVisible))
                IsWindowVisible = true;
        }

        public override void Render(DrawingContext context)
        {
            if (IsVisible && (!IsWindowVisible))
                IsWindowVisible = true;
        }

        public void zzzzRender(DrawingContext context)
        {
            if (!_rendering)
            {
                _rendering = true;
                if (IsVisible && (!IsWindowVisible))
                    IsWindowVisible = true;

                if (ClipByAlpha)
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        int pxWidth = (int)PlatformImpl.ClientSize.Width;
                        int pxHeight = (int)PlatformImpl.ClientSize.Height;
                        //Debug.WriteLine("Bounds: " + pxWidth + ", " + pxHeight);
                        if ((pxWidth >= 1) && (pxHeight >= 1))
                        {

                            IntPtr rgn = NativeMethodsWindows.CreateRectRgn(0, 0, pxWidth, pxHeight); //NativeMethodsWindows.CreateRoundRectRgn(0, 0, pxWidth + 1, pxHeight + 1, 10, 10); //NativeMethodsWindows.CreateRectRgn(0, 0, pxWidth, pxHeight); //IntPtr.Zero;
                                                                                                      //NativeMethodsWindows.GetWindowRgn(PlatformImpl.Handle.Handle, rgn);
                            RenderTargetBitmap rtbmp = new RenderTargetBitmap(new PixelSize(pxWidth, pxHeight)); //WriteableBitmap rtbmp = new WriteableBitmap(new PixelSize(pxWidth, pxHeight), new Vector(96, 96));
                                                                                                                 //ImmediateRenderer.Render(this, context);
                            rtbmp.Render(this);
                            //rtbmp.Render(this);
                            //rtbmp.Save(@"C:\Users\Splitwirez\Test.png");
                            using (System.IO.Stream unconvStream = new System.IO.MemoryStream())
                            {
                                rtbmp.Save(unconvStream);
                                //AvBitmap bmp = new AvBitmap(unconvStream); //AvBitmap.FromStream(unconvStream);
                                using (DrawBitmap bmp = (DrawBitmap)DrawBitmap.FromStream(unconvStream, true))
                                {
                                    int leftSize = (int)BorderThickness.Left;
                                    int topSize = (int)BorderThickness.Top;
                                    int rightSize = (int)BorderThickness.Right;
                                    int bottomSize = (int)BorderThickness.Bottom;
                                    //top left corner
                                    for (int y = 0; y < topSize; y++)
                                    {
                                        for (int x = 0; x < leftSize; x++)
                                            ExcludePixelFromRegion(unconvStream, x, y, pxWidth, pxHeight, bmp, rgn);
                                    }
                                    //top right corner
                                    for (int y = 0; y < topSize; y++)
                                    {
                                        for (int x = (pxWidth - rightSize); x < pxWidth; x++)
                                            ExcludePixelFromRegion(unconvStream, x, y, pxWidth, pxHeight, bmp, rgn);
                                    }
                                    //bottom left corner
                                    for (int y = (pxHeight - bottomSize); y < pxHeight; y++)
                                    {
                                        for (int x = 0; x < leftSize; x++)
                                            ExcludePixelFromRegion(unconvStream, x, y, pxWidth, pxHeight, bmp, rgn);
                                    }
                                    //bottom right corner
                                    for (int y = (pxHeight - bottomSize); y < pxHeight; y++)
                                    {
                                        for (int x = (pxWidth - rightSize); x < pxWidth; x++)
                                            ExcludePixelFromRegion(unconvStream, x, y, pxWidth, pxHeight, bmp, rgn);
                                    }
                                    NativeMethodsWindows.SetWindowRgn(PlatformImpl.Handle.Handle, rgn, true);
                                    //Debug.WriteLine("Done!");
                                }
                            }
                            #region Trash
                            /*List<DrawSize> coords = new List<DrawSize>();
                            for (int i = 13; i < pixelCount; i += 4)
                            {
                                stream.Position = i;
                                int alpha = stream.ReadByte();
                                //Debug.WriteLine("Processing pixel: " + alpha + ", coords: " + xPos + ", " + yPos);
                                if (alpha == 0)
                                {
                                    GetCoordsFromPixelIndex(i, pxWidth, pxHeight, out xPos, out yPos);
                                    //coords.Add(new DrawSize())
                                    IntPtr srcRgn = rgn;
                                    IntPtr deletRgn = NativeMethodsWindows.CreateRectRgn(xPos, yPos, xPos + 1, yPos + 1);//NativeMethodsWindows.CreateRectRgn(xPos, yPos, xPos + 1, yPos + 1);
                                    NativeMethodsWindows.CombineRgn(rgn, deletRgn, srcRgn, NativeMethodsWindows.CombineRgnStyles.RGN_XOR);
                                }
                                int innerRight = (int)(pxWidth - BorderThickness.Right);
                                int innerBottom = (int)(pxHeight - BorderThickness.Bottom);



                                //xPos++;
                                if ((xPos > BorderThickness.Left) && (xPos < innerRight))
                                {
                                    i += 4 * (innerRight - (int)BorderThickness.Left);
                                    //xPos = innerRight;
                                }
                                if ((yPos > BorderThickness.Top) && (yPos < innerBottom))
                                {
                                    i = GetPixelIndexFromCoords(innerRight, innerBottom, pxWidth, pxHeight);
                                    //i += pxWidth * innerBottom * 4;

                                    //yPos = innerBottom;
                                }

                                /*if (xPos > pxWidth)
                                {
                                    yPos++;
                                    xPos = 0;
                                }*
                            }*/
                            //Debug.WriteLine("Done!");
                            /*for (int xAlpha = 3; xAlpha < (pxWidth - 1); xAlpha += 4)
                            //for (int x = 0; x < (pxWidth - 1); x++)
                            {
                                for (int yAlpha = 3; yAlpha < (pxHeight - 1); yAlpha += 4)
                                //for (int y = 0; y < (pxHeight - 1); y++)
                                {
                                    stream.Position = xAlpha * yAlpha;
                                    //stream.Position = ((x * 4) + 3) * ((y * 4) + 3);
                                    //stream.Position = ((x * 4) + 3);
                                    if (stream.ReadByte() == 0)
                                    {
                                        IntPtr srcRgn = rgn;
                                        IntPtr deletRgn = NativeMethodsWindows.CreateRectRgn(xPos, yPos, xPos + 1, yPos + 1);
                                        NativeMethodsWindows.CombineRgn(rgn, deletRgn, srcRgn, NativeMethodsWindows.CombineRgnStyles.RGN_XOR);
                                    }
                                    yPos++;
                                }
                                xPos++;
                            }*/
                            /*DrawBitmap bmp = new DrawBitmap(stream);
                            for (int x = 0; x < (pxWidth - 1); x++)
                            {
                                for (int y = 0; y < (pxHeight - 1); y++)
                                {
                                    Color color = bmp.GetPixel(x, y);
                                    if (color.A == 0)
                                    {
                                        //IntPtr srcRgn = rgn;
                                        IntPtr deletRgn = NativeMethodsWindows.CreateRectRgn(x, y, x + 1, y + 1);
                                        //NativeMethodsWindows.CombineRgn(rgn, deletRgn, srcRgn, NativeMethodsWindows.CombineRgnStyles.RGN_XOR);
                                    }
                                }
                            }*/

                            //WriteableBitmap bmp = new WriteableBitmap(new PixelSize(context.bou))
                            //IRenderTargetBitmapImpl bmp = context.PlatformImpl.CreateLayer(new Size(100, 100));
                            //context.PlatformImpl.
                            #endregion Trash
                        }
                    }
                }
                _rendering = false;
            }
        }

        void ExcludePixelFromRegion(System.IO.Stream stream, int x, int y, int width, int height, DrawBitmap bmp, IntPtr rgn)
        {
            stream.Position = (GetPixelIndexFromCoords(x, y, width, height) * 4) + 13;
            //ExcludePixelFromRegion(stream, x, y, pxWidth, pxHeight, rgn);
            if (bmp.GetPixel(x, y).A <= AlphaThreshold/*unconvStream.ReadByte() == 0*/)
            {
                IntPtr srcRgn = rgn;
                IntPtr deletRgn = NativeMethodsWindows.CreateRectRgn(x, y, x + 1, y + 1);//NativeMethodsWindows.CreateRectRgn(xPos, yPos, xPos + 1, yPos + 1);
                NativeMethodsWindows.CombineRgn(rgn, deletRgn, srcRgn, NativeMethodsWindows.CombineRgnStyles.RGN_XOR);
            }
        }


        static void zExcludePixelFromRegion(System.IO.Stream stream, int xPos, int yPos, int width, int height, IntPtr rgn)
        {
            int index = GetPixelIndexFromCoords(xPos, yPos, width, height);
            int streamPos = (index * 4) + 13;
            stream.Position = streamPos;
            int val = stream.ReadByte();
            Debug.WriteLine("Pos info: " + index + " = " + xPos + ", " + yPos + " = " + streamPos + " => " + val);
            if (val == 0)
            {
                IntPtr srcRgn = rgn;
                IntPtr deletRgn = NativeMethodsWindows.CreateRectRgn(xPos, yPos, xPos + 1, yPos + 1);//NativeMethodsWindows.CreateRectRgn(xPos, yPos, xPos + 1, yPos + 1);
                NativeMethodsWindows.CombineRgn(rgn, deletRgn, srcRgn, NativeMethodsWindows.CombineRgnStyles.RGN_XOR);
            }
        }

        static void GetCoordsFromPixelIndex(int pos, int width, int height, out int xPos, out int yPos)
        {
            /*int posMult = 0;
            while ((posMult + width) < pos)
            {
                posMult += width;
            }
            xPos = (pos - posMult) / 4;
            yPos = posMult / 4;*/
            int pixelCount = width * height;
            int pIndex = (pos / 4) - 13;
            //Debug.WriteLine("pIndex: " + pIndex);
            double pIndex2 = pIndex;
            double pIndexDivided = pIndex2 / width;
            int xRem = 0;
            while (pIndexDivided != (int)pIndexDivided)
            {
                pIndex2--;
                xRem++;
                pIndexDivided = pIndex2 / width;
            }
            xPos = xRem;
            yPos = (int)pIndex2 / height;
            //Debug.WriteLine("Coords: " + xPos + ", " + yPos);
            //yPos = width * pIndex2;
        }

        static int GetPixelIndexFromCoords(int xPos, int yPos, int width, int height)
        {
            return (yPos * width) + xPos;
        }

        /*protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            IsOpen = true;
        }*/
    }
}
