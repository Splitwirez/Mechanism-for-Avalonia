using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Text;
using Mechanism.AvaloniaUI.Drawing;
using System.IO;
using Image = System.Drawing.Image;
using DrawBitmap = System.Drawing.Bitmap;
using PointF = System.Drawing.PointF;

namespace Mechanism.AvaloniaUI.Themes.Fluent
{
    public class ButtonPressRotator : TemplatedControl
    {
        /*public static readonly StyledProperty<bool> IsVisuallyPressedProperty =
            AvaloniaProperty.Register<ButtonPressRotator, bool>(nameof(IsVisuallyPressed), defaultValue: false);

        public bool IsVisuallyPressed
        {
            get => GetValue(IsVisuallyPressedProperty);
            set => SetValue(IsVisuallyPressedProperty, value);
        }*/

        public static readonly StyledProperty<float> InsetMultiplierProperty =
            AvaloniaProperty.Register<ButtonPressRotator, float>(nameof(InsetMultiplier), defaultValue: 0);

        public float InsetMultiplier
        {
            get => GetValue(InsetMultiplierProperty);
            set => SetValue(InsetMultiplierProperty, value);
        }

        public static readonly StyledProperty<Control> TargetElementProperty =
            AvaloniaProperty.Register<ButtonPressRotator, Control>(nameof(TargetElement));

        public Control TargetElement
        {
            get => GetValue(TargetElementProperty);
            set => SetValue(TargetElementProperty, value);
        }

        static ButtonPressRotator()
        {
            InsetMultiplierProperty.Changed.AddClassHandler<ButtonPressRotator>(new Action<ButtonPressRotator, AvaloniaPropertyChangedEventArgs>((sender, e) => sender.OnInsetMultiplierPropertyChanged()));
            TargetElementProperty.Changed.AddClassHandler<ButtonPressRotator>(new Action<ButtonPressRotator, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                if (e.OldValue != null)
                    (e.OldValue as Control).PointerMoved -= sender.TargetElement_PointerMoved;

                if (e.NewValue != null)
                     (e.NewValue as Control).PointerMoved += sender.TargetElement_PointerMoved;
            }));
        }

        static Brush _background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));

        float xPercentage = 0.5f;
        float yPercentage = 0.5f;
        public ButtonPressRotator()
        {
            //Background = _background;
        }

        private void TargetElement_PointerMoved(object sender, Avalonia.Input.PointerEventArgs e)
        {
            var pnt = e.GetPosition(TargetElement);
            xPercentage = (float)(pnt.X / TargetElement.Bounds.Width);
            yPercentage = (float)(pnt.Y / TargetElement.Bounds.Height);
        }

        bool IsPressed => Math.Round(InsetMultiplier, 1) > 0.0f;

        void OnInsetMultiplierPropertyChanged()
        {
            InvalidateVisual();
            if (IsPressed)
                (TargetElement.Parent as Visual).Opacity = 0.001;
            else
                (TargetElement.Parent as Visual).Opacity = 1;
            /*if (value)
            {

            }*/
        }

        //bool _saved = false;
        RenderTargetBitmap _bmp = null;
        public override void Render(DrawingContext context)
        {
            base.Render(context);
            if (IsPressed)
            {
                _bmp = new RenderTargetBitmap(PixelSize.FromSize(TargetElement.Bounds.Size, VisualRoot.RenderScaling));
                _bmp.Render(TargetElement);
                var transform = new FreeTransform()
                {
                    //IsBilinearInterpolation = true,
                    TopLeftVertex = new PointF(0, 0),
                    TopRightVertex = new PointF((float)TargetElement.Bounds.Width, 0),
                    BottomRightVertex = new PointF((float)TargetElement.Bounds.Width, (float)TargetElement.Bounds.Height),
                    BottomLeftVertex = new PointF(0, (float)TargetElement.Bounds.Height)
                };

                transform.Bitmap = _bmp.ToDrawingBitmap();
                //float InsetMultiplier = 5;
                float halfInsetMultiplier = InsetMultiplier / 2;
                float minDimension = (float)Math.Min(TargetElement.Bounds.Width, TargetElement.Bounds.Height);

                /*float invX = (float)(-xPercentage + 1);
                float invY = (float)(-yPercentage + 1);

                float targetWidth = (float)TargetElement.Bounds.Width;
                float targetHeight = (float)TargetElement.Bounds.Height;*/
                /*float xMin = InsetMultiplier * invX;
                float yMin = InsetMultiplier * invY;
                float xMax = (float)TargetElement.Bounds.Width - (InsetMultiplier * xPercentage);
                float yMax = (float)TargetElement.Bounds.Height - (InsetMultiplier * yPercentage);*/
                /*float nearX = invX * InsetMultiplier;
                float nearY = invY * InsetMultiplier;
                float farX = yPercentage * InsetMultiplier;
                float farY = xPercentage * InsetMultiplier;
                transform.TopLeftVertex = new Point(nearX, nearY).ToDrawingPointF();
                transform.TopRightVertex = new Point(targetWidth - farX, nearY).ToDrawingPointF();
                transform.BottomRightVertex = new Point(targetWidth - farX, targetWidth - farY).ToDrawingPointF();
                transform.BottomLeftVertex = new Point(nearX, targetWidth - farY).ToDrawingPointF();*/

                float inset = (minDimension / 100) * InsetMultiplier;
                float halfInset = (minDimension / 100) * halfInsetMultiplier;
                if ((xPercentage <= 0.5) && (yPercentage <= 0.5)) //TOP LEFT
                {
                    transform.TopLeftVertex = new PointF(inset, inset);
                    transform.TopRightVertex = new Point(transform.TopRightVertex.X - halfInset, halfInset).ToDrawingPointF();
                    transform.BottomLeftVertex = new Point(halfInset, transform.BottomLeftVertex.Y - halfInset).ToDrawingPointF();
                    //transform.BottomRightVertex = new Point(transform.BottomRightVertex.X - inset, transform.BottomRightVertex.Y - inset).ToDrawingPointF();
                }
                else if ((xPercentage > 0.5) && (yPercentage < 0.5)) //TOP RIGHT
                {
                    transform.TopLeftVertex = new PointF(halfInset, halfInset);
                    transform.TopRightVertex = new Point(transform.TopRightVertex.X - inset, inset).ToDrawingPointF();
                    //transform.BottomLeftVertex = new Point(halfInset, transform.BottomLeftVertex.Y - halfInset).ToDrawingPointF();
                    transform.BottomRightVertex = new Point(transform.BottomRightVertex.X - halfInset, transform.BottomRightVertex.Y - halfInset).ToDrawingPointF();
                }
                else if ((xPercentage > 0.5) && (yPercentage > 0.5)) //BOTTOM RIGHT
                {
                    transform.TopRightVertex = new Point(transform.TopRightVertex.X - halfInset, halfInset).ToDrawingPointF();
                    transform.BottomLeftVertex = new Point(halfInset, transform.BottomLeftVertex.Y - halfInset).ToDrawingPointF();
                    transform.BottomRightVertex = new Point(transform.BottomRightVertex.X - inset, transform.BottomRightVertex.Y - inset).ToDrawingPointF();
                }
                else if ((xPercentage < 0.5) && (yPercentage > 0.5)) //BOTTOM LEFT
                {
                    transform.TopLeftVertex = new PointF(halfInset, halfInset);
                    //transform.TopRightVertex = new Point(transform.TopRightVertex.X - halfInset, halfInset).ToDrawingPointF();
                    transform.BottomLeftVertex = new Point(inset, transform.BottomLeftVertex.Y - inset).ToDrawingPointF();
                    transform.BottomRightVertex = new Point(transform.BottomRightVertex.X - halfInset, transform.BottomRightVertex.Y - halfInset).ToDrawingPointF();
                }
                /*transform.Vertices = new PointF[]
                {
                    new PointF(0, 0),
                    new PointF
                };*/
                /*transform.TopLeftVertex = new PointF(10, 10);
                transform.TopRightVertex = new PointF((float)(TargetElement.Bounds.Size.Width - 5), 10);
                transform.BottomRightVertex = new PointF((float)TargetElement.Bounds.Size.Width, (float)TargetElement.Bounds.Size.Height);
                transform.BottomLeftVertex = new PointF(10, (float)(TargetElement.Bounds.Size.Width - 5));*/

                //VertexLeftTop = 
                //};
                var distorted = transform.Bitmap.ToMediaBitmap();
                context.DrawImage(distorted, /*1, */new Rect(0, 0, distorted.PixelSize.Width, distorted.PixelSize.Height), new Rect(Bounds.Size));
            }
            /*else if (!_saved) //FOR DIAGNOSTIC PURPOSES ONLY
            {
                _bmp = new RenderTargetBitmap(PixelSize.FromSize(TargetElement.Bounds.Size, VisualRoot.RenderScaling));
                _bmp.Render(TargetElement);
                _bmp.Save(Environment.ExpandEnvironmentVariables(@"%userprofile%\Pictures\AVALONIA_TEXT_RENDER_TEST.png"));
                _saved = true;
            }*/
        }
    }
}
