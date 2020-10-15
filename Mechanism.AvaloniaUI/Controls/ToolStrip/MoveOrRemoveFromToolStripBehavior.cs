using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using System.IO;
using Avalonia.Layout;
using Avalonia.Controls;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class MoveOrRemoveFromToolStripBehavior : Behavior<Thumb>
    {
        public static readonly StyledProperty<ToolStrip> OwnerProperty =
            AvaloniaProperty.Register<MoveOrRemoveFromToolStripBehavior, ToolStrip>(nameof(Owner));

        public ToolStrip Owner
        {
            get => GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        /*public static readonly StyledProperty<IToolStripItem> TargetItemProperty =
            AvaloniaProperty.Register<MoveOrRemoveFromToolStripBehavior, IToolStripItem>(nameof(TargetItem));

        public IToolStripItem TargetItem
        {
            get => GetValue(TargetItemProperty);
            set => SetValue(TargetItemProperty, value);
        }*/
        public static readonly StyledProperty<ToolStripItemReference> TargetProperty =
            AvaloniaProperty.Register<ToolStripItemPointerOverBehavior, ToolStripItemReference>(nameof(Target));

        public ToolStripItemReference Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            Debug.WriteLine("MoveOrRemoveFromToolStripBehavior");
            AssociatedObject.DragStarted += AssociatedObject_DragStarted;
            AssociatedObject.DragDelta += AssociatedObject_DragDelta;
            AssociatedObject.DragCompleted += AssociatedObject_DragCompleted;
        }

        Rectangle _popupDragMovePreview = null;
        Rectangle _windowDragMovePreview = null;
        double _dragMoveStartLeft = 0;
        double _dragMoveStartTop = 0;
        private void AssociatedObject_DragStarted(object sender, VectorEventArgs e)
        {
            var visRoot = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(AssociatedObject);
            var pnt = AssociatedObject.TranslatePoint(new Point(0, 0), visRoot).GetValueOrDefault();
            _dragMoveStartLeft = pnt.X;
            _dragMoveStartTop = pnt.Y;


            var tmplParent = AssociatedObject.Parent as Visual;
            var pxSize = new PixelSize((int)(tmplParent.Bounds.Width * visRoot.RenderScaling), (int)(tmplParent.Bounds.Height * visRoot.RenderScaling));
            Console.WriteLine("pxSize: " + pxSize);
            RenderTargetBitmap bmp = new RenderTargetBitmap(pxSize);
            bmp.Render(tmplParent);
            ImageBrush brush = null;
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream);
                stream.Position = 0;
                brush = new ImageBrush(new Bitmap(stream));
            }

            
            _windowDragMovePreview = new Rectangle()
            {
                Width = AssociatedObject.Bounds.Width,
                Height = AssociatedObject.Bounds.Height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Fill = brush,
                Margin = new Thickness(_dragMoveStartLeft, _dragMoveStartTop, -_dragMoveStartLeft, -_dragMoveStartTop)
            };
            _popupDragMovePreview = new Rectangle()
            {
                Width = AssociatedObject.Bounds.Width,
                Height = AssociatedObject.Bounds.Height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Fill = brush,
                Margin = WindowMarginToPopupMargin(_windowDragMovePreview.Margin)
            };

            AdornerLayer.GetAdornerLayer(Owner).Children.Add(_windowDragMovePreview);
            AdornerLayer.GetAdornerLayer(Owner._customizePopup).Children.Add(_popupDragMovePreview);
        }

        private void AssociatedObject_DragDelta(object sender, VectorEventArgs e)
        {
            double x = _dragMoveStartLeft + e.Vector.X;
            double y = _dragMoveStartTop + e.Vector.Y;
            _windowDragMovePreview.Margin = new Thickness(x, y, -x, -y);
            _popupDragMovePreview.Margin = WindowMarginToPopupMargin(_windowDragMovePreview.Margin);
        }

        private void AssociatedObject_DragCompleted(object sender, VectorEventArgs e)
        {
            var popupLayer = AdornerLayer.GetAdornerLayer(Owner._customizePopup);
            
            if (popupLayer.Children.Contains(_popupDragMovePreview))
                popupLayer.Children.Remove(_popupDragMovePreview);

            var windowLayer = AdornerLayer.GetAdornerLayer(Owner);
            
            if (windowLayer.Children.Contains(_windowDragMovePreview))
                windowLayer.Children.Remove(_windowDragMovePreview);


            Debug.WriteLine("Drag completed");
            Owner.ValidateMoveOrRemoveFromToolStrip(Target, sender as Visual, e.Vector);
        }

        Thickness WindowMarginToPopupMargin(Thickness popupMargin)
        {
            /*if (Avalonia.VisualTree.VisualExtensions.GetVisualRoot(Owner) is Window win)
                win.OffScreenMargin*/
            var win = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(Owner);
            var pnt = win.PointToScreen(Avalonia.VisualTree.VisualExtensions.GetVisualRoot(Owner._customizePopup).PointToClient(new PixelPoint((int)(popupMargin.Left * win.RenderScaling), (int)(popupMargin.Top * win.RenderScaling))));
            return new Thickness(pnt.X, pnt.Y, -pnt.X, -pnt.Y);
        }
    }
}
