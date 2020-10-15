using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Layout;
using Avalonia.Input;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media.Imaging;
using System.IO;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class AddToToolStripBehavior : Behavior<Thumb>
    {
        public static readonly StyledProperty<ToolStrip> OwnerProperty =
            AvaloniaProperty.Register<AddToToolStripBehavior, ToolStrip>(nameof(Owner));

        public ToolStrip Owner
        {
            get => GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        public static readonly StyledProperty<IToolStripItem> TargetItemProperty =
            AvaloniaProperty.Register<AddToToolStripBehavior, IToolStripItem>(nameof(TargetItem));

        public IToolStripItem TargetItem
        {
            get => GetValue(TargetItemProperty);
            set => SetValue(TargetItemProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            Debug.WriteLine("AddToToolStripBehaviour");
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

            _popupDragMovePreview = new Rectangle()
            {
                Width = AssociatedObject.Bounds.Width,
                Height = AssociatedObject.Bounds.Height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Fill = brush,
                Margin = new Thickness(_dragMoveStartLeft, _dragMoveStartTop, -_dragMoveStartLeft, -_dragMoveStartTop)
            };
            _windowDragMovePreview = new Rectangle()
            {
                Width = AssociatedObject.Bounds.Width,
                Height = AssociatedObject.Bounds.Height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Fill = brush,
                Margin = PopupMarginToWindowMargin(_popupDragMovePreview.Margin)
            };

            Console.WriteLine("START: " + _dragMoveStartLeft + ", " + _dragMoveStartTop);

            //Console.WriteLine("AdornerLayer.GetAdornerLayer(AssociatedObject) != null: " + ( != null));
            /*if (visualRoot is TopLevel topLevel)
            {
                var presenter = topLevel.FindNameScope().Find<ContentPresenter>("PART_ContentPresenter");
                if ((presenter != null) && (presenter.Parent != null) && (presenter.Parent is VisualLayerManager layerMgr))
                {
                    if (layerMgr.OverlayLayer == null)
                    {
                        
                    }
                        //layerMgr.AdornerLayer = new AdornerLayer();
            */
            AdornerLayer.GetAdornerLayer(AssociatedObject).Children.Add(_popupDragMovePreview);
            AdornerLayer.GetAdornerLayer(Owner).Children.Add(_windowDragMovePreview);
                /*}
            }*/
        }

        private void AssociatedObject_DragDelta(object sender, VectorEventArgs e)
        {
            double x = _dragMoveStartLeft + e.Vector.X;
            double y = _dragMoveStartTop + e.Vector.Y;
            _popupDragMovePreview.Margin = new Thickness(x, y, -x, -y);
            _windowDragMovePreview.Margin = PopupMarginToWindowMargin(_popupDragMovePreview.Margin);
        }

        private void AssociatedObject_DragCompleted(object sender, VectorEventArgs e)
        {
            var popupLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            
            if (popupLayer.Children.Contains(_popupDragMovePreview))
                popupLayer.Children.Remove(_popupDragMovePreview);

            var windowLayer = AdornerLayer.GetAdornerLayer(Owner);
            
            if (windowLayer.Children.Contains(_windowDragMovePreview))
                windowLayer.Children.Remove(_windowDragMovePreview);

            _popupDragMovePreview = null;

            Debug.WriteLine("Drag completed");
            Owner.ValidateAddToToolStrip(TargetItem, sender as Visual, e.Vector);
        }

        Thickness PopupMarginToWindowMargin(Thickness popupMargin)
        {
            /*if (Avalonia.VisualTree.VisualExtensions.GetVisualRoot(Owner) is Window win)
                win.OffScreenMargin*/
            
            var pnt = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(Owner).PointToClient(Avalonia.VisualTree.VisualExtensions.GetVisualRoot(AssociatedObject).PointToScreen(new Point(popupMargin.Left, popupMargin.Top)));
            return new Thickness(pnt.X, pnt.Y, -pnt.X, -pnt.Y);
        }
    }
}
