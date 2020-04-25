using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Mechanism.AvaloniaUI.Controls.Windows;
using Mechanism.AvaloniaUI.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Timers;

namespace Mechanism.AvaloniaUI.Controls.Windows
{
    public enum TitlebarPlacementMode
    {
        OutsideContent,
        InsideContent,
        Headerbar
    }

    public enum TitlebarVisibilityMode
    {
        Full,
        Overlay,
        None
    }

    public class StyleableWindow : Window, IStyleable
    {
        public static readonly StyledProperty<bool> UseBlurProperty =
            AvaloniaProperty.Register<StyleableWindow, bool>(nameof(UseBlur), defaultValue: false);

        public bool UseBlur
        {
            get => GetValue(UseBlurProperty);
            set => SetValue(UseBlurProperty, value);
        }

        public static readonly StyledProperty<bool> CanBlurProperty =
        AvaloniaProperty.Register<StyleableWindow, bool>(nameof(CanBlur), defaultValue: false);

        public bool CanBlur
        {
            get => GetValue(CanBlurProperty);
            protected set => SetValue(CanBlurProperty, value);
        }

        public static readonly StyledProperty<bool> ShowCaptionIconProperty =
            AvaloniaProperty.Register<StyleableWindow, bool>(nameof(ShowCaptionIcon), defaultValue: true);

        public bool ShowCaptionIcon
        {
            get => GetValue(ShowCaptionIconProperty);
            set => SetValue(ShowCaptionIconProperty, value);
        }

        public static readonly StyledProperty<bool> HasIconProperty =
            AvaloniaProperty.Register<StyleableWindow, bool>(nameof(HasIcon), defaultValue: false);

        public bool HasIcon
        {
            get => GetValue(HasIconProperty);
            set => SetValue(HasIconProperty, value);
        }

        public static readonly StyledProperty<bool> ShowCaptionTextProperty =
            AvaloniaProperty.Register<StyleableWindow, bool>(nameof(ShowCaptionText), defaultValue: true);

        public bool ShowCaptionText
        {
            get => GetValue(ShowCaptionTextProperty);
            set => SetValue(ShowCaptionTextProperty, value);
        }

        public static readonly StyledProperty<TitlebarPlacementMode> TitlebarPlacementProperty =
            AvaloniaProperty.Register<StyleableWindow, TitlebarPlacementMode>(nameof(TitlebarPlacement), defaultValue: TitlebarPlacementMode.OutsideContent);

        public TitlebarPlacementMode TitlebarPlacement
        {
            get => GetValue(TitlebarPlacementProperty);
            set => SetValue(TitlebarPlacementProperty, value);
        }

        public static readonly StyledProperty<TitlebarVisibilityMode> TitlebarVisibilityProperty =
            AvaloniaProperty.Register<StyleableWindow, TitlebarVisibilityMode>(nameof(TitlebarVisibility), defaultValue: TitlebarVisibilityMode.Full);

        public TitlebarVisibilityMode TitlebarVisibility
        {
            get => GetValue(TitlebarVisibilityProperty);
            set => SetValue(TitlebarVisibilityProperty, value);
        }

        /*public static readonly StyledProperty<bool> ExtendBackgroundIntoTitlebarProperty =
            AvaloniaProperty.Register<StyleableWindow, bool>(nameof(ExtendBackgroundIntoTitlebar), defaultValue: false);

        public bool ExtendBackgroundIntoTitlebar
        {
            get => GetValue(ExtendBackgroundIntoTitlebarProperty);
            set => SetValue(ExtendBackgroundIntoTitlebarProperty, value);
        }*/

        public static readonly StyledProperty<HorizontalAlignment> HorizontalCaptionAlignmentProperty =
            AvaloniaProperty.Register<StyleableWindow, HorizontalAlignment>(nameof(HorizontalCaptionAlignment), defaultValue: HorizontalAlignment.Left);

        public HorizontalAlignment HorizontalCaptionAlignment
        {
            get => GetValue(HorizontalCaptionAlignmentProperty);
            set => SetValue(HorizontalCaptionAlignmentProperty, value);
        }

        public static readonly StyledProperty<double> BaseTitlebarHeightProperty =
            AvaloniaProperty.Register<StyleableWindow, double>(nameof(BaseTitlebarHeight), defaultValue: 0.0);

        public double BaseTitlebarHeight
        {
            get => GetValue(BaseTitlebarHeightProperty);
            protected set => SetValue(BaseTitlebarHeightProperty, value);
        }

        public static readonly StyledProperty<double> ExtendedTitlebarHeightProperty =
            AvaloniaProperty.Register<StyleableWindow, double>(nameof(ExtendedTitlebarHeight), defaultValue: 0.0);

        public double ExtendedTitlebarHeight
        {
            get => GetValue(ExtendedTitlebarHeightProperty);
            set => SetValue(ExtendedTitlebarHeightProperty, value);
        }

        public static readonly StyledProperty<double> TitlebarHeightProperty =
            AvaloniaProperty.Register<StyleableWindow, double>(nameof(TitlebarHeight), defaultValue: 0.0);

        public double TitlebarHeight
        {
            get => GetValue(TitlebarHeightProperty);
            protected set => SetValue(TitlebarHeightProperty, value);
        }

        public static readonly StyledProperty<bool> UseClosingAnimationProperty =
            AvaloniaProperty.Register<StyleableWindow, bool>(nameof(UseClosingAnimation), defaultValue: false);

        public bool UseClosingAnimation
        {
            get => GetValue(UseClosingAnimationProperty);
            set => SetValue(UseClosingAnimationProperty, value);
        }

        public static readonly StyledProperty<bool> IsWindowVisibleProperty =
            AvaloniaProperty.Register<StyleableWindow, bool>(nameof(IsWindowVisible), defaultValue: false);

        public bool IsWindowVisible
        {
            get => GetValue(IsWindowVisibleProperty);
            set => SetValue(IsWindowVisibleProperty, value);
        }

        Type IStyleable.StyleKey => typeof(StyleableWindow);

        static StyleableWindow()
        {
            //HasSystemDecorationsProperty.OverrideDefaultValue<StyleableWindow>(false);
            /*HasSystemDecorationsProperty.OverrideValidation<StyleableWindow>(new Func<StyleableWindow, bool, bool>((window, value) =>
            {
                return value == false;
            }));*/
            //HasSystemDecorationsProperty.OverrideMetadata<StyleableWindow>(new StyledPropertyMetadata<bool>(false, new Func<IAvaloniaObject, bool, bool>((window, value) => value == false)));
            HasSystemDecorationsProperty.OverrideMetadata<StyleableWindow>(new StyledPropertyMetadata<bool>(true, new Func<IAvaloniaObject, bool, bool>((window, value) => false)));
            BaseTitlebarHeightProperty.Changed.AddClassHandler<StyleableWindow>((sender, e) => sender.UpdateTotalTitlebarHeight());
            ExtendedTitlebarHeightProperty.Changed.AddClassHandler<StyleableWindow>((sender, e) => sender.UpdateTotalTitlebarHeight());
            IconProperty.Changed.AddClassHandler<StyleableWindow>((sender, e) => sender.HasIcon = (sender.Icon != null));
        }

        protected void UpdateTotalTitlebarHeight()
        {
            TitlebarHeight = BaseTitlebarHeight + ExtendedTitlebarHeight;
        }

        public StyleableWindow()
        {
            HasSystemDecorations = false;
            //hass
        }

        public override void Render(DrawingContext context)
        {
            if (IsVisible && (!IsWindowVisible))
                IsWindowVisible = true;

            base.Render(context);
        }

        bool _isClosingNow = false;
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (UseClosingAnimation)
            {
                bool animate = false;

                if (!(e.Cancel) & !_isClosingNow)
                {
                    e.Cancel = true;
                    _isClosingNow = true;
                    animate = true;
                }

                if (animate)
                {
                    Timer timer = new Timer(1);
                    timer.Elapsed += (sneder, args) =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            if (!IsVisible)
                                Close();
                        });
                    };
                    timer.Start();
                    IsWindowVisible = false;
                }
            }
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            if (e.NameScope.TryGet("PART_MinimizeButton", out Button minBtn))
            {
                minBtn.Click += (sneder, args) => WindowState = WindowState.Minimized;
            }
            if (e.NameScope.TryGet("PART_MaximizeButton", out Button maxBtn))
            {
                maxBtn.Click += (sneder, args) => ToggleMaximized();
            }
            if (e.NameScope.TryGet("PART_CloseButton", out Button closeBtn))
            {
                closeBtn.Click += (sneder, args) => Close();
            }

            if (e.NameScope.TryGet("PART_Titlebar", out Control titlebar))
            {
                titlebar.DoubleTapped += (sneder, args) => ToggleMaximized();
                titlebar.PointerPressed += (sneder, args) => BeginMoveDrag(args);
            }

            if (e.NameScope.TryGet("PART_RestoreMenuItem", out MenuItem restMI))
                restMI.Click += (sneder, args) => WindowState = WindowState.Normal;

            if (e.NameScope.TryGet("PART_MinimizeMenuItem", out MenuItem minMI))
                minMI.Click += (sneder, args) => WindowState = WindowState.Minimized;

            if (e.NameScope.TryGet("PART_MaximizeMenuItem", out MenuItem maxMI))
                maxMI.Click += (sneder, args) => WindowState = WindowState.Maximized;

            if (e.NameScope.TryGet("PART_CloseMenuItem", out MenuItem closeMI))
                closeMI.Click += (sneder, args) => Close();

            SetupSide("TopLeft", StandardCursorType.TopLeftCorner, WindowEdge.NorthWest, ref e);
            SetupSide("TopCenter", StandardCursorType.TopSide, WindowEdge.North, ref e);
            SetupSide("TopRight", StandardCursorType.TopRightCorner, WindowEdge.NorthEast, ref e);
            SetupSide("MiddleRight", StandardCursorType.RightSide, WindowEdge.East, ref e);
            SetupSide("BottomRight", StandardCursorType.BottomRightCorner, WindowEdge.SouthEast, ref e);
            SetupSide("BottomCenter", StandardCursorType.BottomSide, WindowEdge.South, ref e);
            SetupSide("BottomLeft", StandardCursorType.BottomLeftCorner, WindowEdge.SouthWest, ref e);
            SetupSide("MiddleLeft", StandardCursorType.LeftSide, WindowEdge.West, ref e);
        }

        void SetupSide(string name, StandardCursorType cursor, WindowEdge edge, ref TemplateAppliedEventArgs e)
        {
            var control = e.NameScope.Get<Control>("PART_" + name + "Edge");
            control.Cursor = new Cursor(cursor);
            control.PointerPressed += (sender, ep) => BeginResizeDrag(edge, ep);
        }

        void ToggleMaximized()
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }
    }
}
