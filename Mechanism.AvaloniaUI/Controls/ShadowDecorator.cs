using Avalonia.Controls.Utils;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using Avalonia.Media;
using Avalonia;
using Avalonia.Controls;

namespace Mechanism.AvaloniaUI.Controls
{
    public partial class ShadowDecorator : Decorator
    {
        public static readonly StyledProperty<Color> ColorProperty =
            AvaloniaProperty.Register<ShadowDecorator, Color>(nameof(Color));

        public static readonly StyledProperty<bool> UseClippingProperty =
            AvaloniaProperty.Register<ShadowDecorator, bool>(nameof(UseClipping));

        public static readonly StyledProperty<Thickness> ExtentProperty =
            AvaloniaProperty.Register<ShadowDecorator, Thickness>(nameof(Extent));

        public static readonly StyledProperty<Thickness> DepthProperty =
            AvaloniaProperty.Register<ShadowDecorator, Thickness>(nameof(Depth));

        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
            AvaloniaProperty.Register<ShadowDecorator, CornerRadius>(nameof(CornerRadius));

        private readonly ShadowDecoratorRenderHelper _shadowDecoratorRenderHelper = new ShadowDecoratorRenderHelper();

        /// <summary>
        /// Initializes static members of the <see cref="ShadowDecorator"/> class.
        /// </summary>
        static ShadowDecorator()
        {
            AffectsRender<ShadowDecorator>(
                ColorProperty,
                ExtentProperty,
                CornerRadiusProperty,
                UseClippingProperty);
            AffectsMeasure<ShadowDecorator>(ExtentProperty);
        }

        /*public ShadowDecorator()
        {
            Clip = _shadowDecoratorRenderHelper.Update(Bounds.Size, Extent, CornerRadius);
        }*/

        public bool UseClipping
        {
            get => GetValue(UseClippingProperty);
            set => SetValue(UseClippingProperty, value);
        }

        public Color Color
        {
            get => GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public Thickness Extent
        {
            get => GetValue(ExtentProperty);
            set => SetValue(ExtentProperty, value);
        }

        public Thickness Depth
        {
            get => GetValue(DepthProperty);
            set => SetValue(DepthProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            //base.Render(context);
            Clip = _shadowDecoratorRenderHelper.Render(context, Bounds.Size, Extent, Depth, CornerRadius, Color);
        }

        /// <summary>
        /// Measures the control.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The desired size of the control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return LayoutHelper.MeasureChild(Child, availableSize, Padding, Extent);
        }

        /// <summary>
        /// Arranges the control's child.
        /// </summary>
        /// <param name="finalSize">The size allocated to the control.</param>
        /// <returns>The space taken.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Clip = _shadowDecoratorRenderHelper.Update(finalSize, Extent, CornerRadius, UseClipping);

            return LayoutHelper.ArrangeChild(Child, finalSize, Padding, Extent);
        }
    }

    internal class ShadowDecoratorRenderHelper
    {
        private StreamGeometry _backgroundGeometryCache;
        private StreamGeometry _borderGeometryCache;

        public Geometry Update(Size finalSize, Thickness extent, CornerRadius cornerRadius, bool useClipping)
        {
            if (useClipping && (finalSize.Width > 0) && (finalSize.Height > 0))
            {
                var borderGeometry = new StreamGeometry();

                using (var ctx = borderGeometry.Open())
                {
                    CreateEdgeGeometry(ctx, finalSize, extent, cornerRadius);
                }

                _borderGeometryCache = borderGeometry;
            }
            else
            {
                _borderGeometryCache = null;
            }
            return _borderGeometryCache;
        }

        public Geometry Render(DrawingContext context, Size size, Thickness extent, Thickness depth, CornerRadius radii, Color color)
        {
            /*if (_borderGeometryCache != null)
                context.DrawGeometry(new SolidColorBrush(Colors.SkyBlue), null, _borderGeometryCache);*/

            Color transp = new Color(0, color.R, color.G, color.B);

            GradientStops stops = new GradientStops()
            {
                new GradientStop(color, 0),
                new GradientStop(new Color(Convert.ToByte(color.A * 0.375), color.R, color.G, color.B), 0.375),
                //new GradientStop(new Color(Convert.ToByte(color.A * 0.25), color.R, color.G, color.B), 0.5),
                new GradientStop(new Color(Convert.ToByte(color.A * 0.1375), color.R, color.G, color.B), 0.75),
                //new GradientStop(new Color(Convert.ToByte(color.A * 0.125), color.R, color.G, color.B), 0.875),
                new GradientStop(transp, 0.95)
            };
            double cntrWidth = size.Width - (depth.Left + depth.Right);
            double cntrHeight = size.Height - (depth.Top + depth.Bottom);
            double rightInterior = size.Width - depth.Right;
            double bottomInterior = size.Height - depth.Bottom;
            /*var backgroundGeometry = _backgroundGeometryCache;
            if (backgroundGeometry != null)
            {
                context.DrawGeometry(new SolidColorBrush(color), null, backgroundGeometry);
            }*/

            // TOP LEFT //
            RelativePoint center = new RelativePoint(1, 1, RelativeUnit.Relative);
            context.FillRectangle(new RadialGradientBrush()
            {
                Center = center,
                GradientOrigin = center,
                GradientStops = stops,
                Radius = 1
            }, new Rect(0, 0, depth.Left, depth.Top));

            // TOP CENTER //
            context.FillRectangle(new LinearGradientBrush()
            {
                StartPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                GradientStops = stops
            }, new Rect(depth.Left, 0, cntrWidth, depth.Top));

            // TOP RIGHT //
            center = new RelativePoint(rightInterior, depth.Top, RelativeUnit.Absolute);
            context.FillRectangle(new RadialGradientBrush()
            {
                Center = center,
                GradientOrigin = center,
                GradientStops = stops,
                Radius = 1
            }, new Rect(rightInterior, 0, depth.Right, depth.Top));

            // MIDDLE LEFT //
            context.FillRectangle(new LinearGradientBrush()
            {
                StartPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                GradientStops = stops
            }, new Rect(0, depth.Top, depth.Left, cntrHeight));

            // MIDDLE CENTER //
            context.FillRectangle(new SolidColorBrush(color), new Rect(depth.Left, depth.Top, cntrWidth, cntrHeight));

            // MIDDLE RIGHT //
            context.FillRectangle(new LinearGradientBrush()
            {
                StartPoint = new RelativePoint(rightInterior, 0, RelativeUnit.Absolute),
                EndPoint = new RelativePoint(size.Width, 0, RelativeUnit.Absolute),
                GradientStops = stops
            }, new Rect(rightInterior, depth.Top, depth.Right, cntrHeight));

            // BOTTOM LEFT //
            center = new RelativePoint(depth.Left, bottomInterior, RelativeUnit.Absolute);
            context.FillRectangle(new RadialGradientBrush()
            {
                Center = center,
                GradientOrigin = center,
                GradientStops = stops,
                Radius = 1
            }, new Rect(0, bottomInterior, depth.Left, depth.Bottom));

            // BOTTOM CENTER //
            context.FillRectangle(new LinearGradientBrush()
            {
                StartPoint = new RelativePoint(0, bottomInterior, RelativeUnit.Absolute),
                EndPoint = new RelativePoint(0, size.Height, RelativeUnit.Absolute),
                GradientStops = stops
            }, new Rect(depth.Left, bottomInterior, cntrWidth, depth.Bottom));

            // BOTTOM RIGHT //
            center = new RelativePoint(rightInterior, bottomInterior, RelativeUnit.Absolute);
            context.FillRectangle(new RadialGradientBrush()
            {
                Center = center,
                GradientOrigin = center,
                GradientStops = stops,
                Radius = 1
            }, new Rect(rightInterior, bottomInterior, depth.Right, depth.Bottom));

            return _borderGeometryCache;
        }
        
        private static void CreateEdgeGeometry(StreamGeometryContext context, Size size, Thickness extent, CornerRadius radius)
        {
            Size innerSize = size.Deflate(extent);
            Point start = new Point(0, extent.Top + radius.TopLeft);
            context.BeginFigure(start, true);
            context.LineTo(new Point(0, 0));
            context.LineTo(new Point(size.Width, 0));
            context.LineTo(new Point(size.Width, size.Height));
            context.LineTo(new Point(0, size.Height));
            context.LineTo(start);
            context.LineTo(new Point(extent.Left, extent.Top + radius.TopLeft));
            context.ArcTo(new Point(extent.Left + radius.TopLeft, extent.Top), new Size(radius.TopLeft, radius.TopLeft), 90, false, SweepDirection.Clockwise);
            context.LineTo(new Point((innerSize.Width + extent.Left) - radius.TopRight, extent.Top));
            context.ArcTo(new Point(innerSize.Width + extent.Left, extent.Top + radius.TopRight), new Size(radius.TopRight, radius.TopRight), 90, false, SweepDirection.Clockwise);
            context.LineTo(new Point(innerSize.Width + extent.Left, (innerSize.Height + extent.Top) - radius.BottomRight));
            context.ArcTo(new Point((innerSize.Width + extent.Left) - radius.BottomRight, innerSize.Height + extent.Top), new Size(radius.BottomRight, radius.BottomRight), 90, false, SweepDirection.Clockwise);
            context.LineTo(new Point(extent.Left + radius.BottomRight, innerSize.Height + extent.Top));
            context.ArcTo(new Point(extent.Left, (innerSize.Height + extent.Top) - radius.BottomRight), new Size(radius.BottomLeft, radius.BottomLeft), 90, false, SweepDirection.Clockwise);
            context.LineTo(new Point(extent.Left, extent.Top + radius.TopLeft));
            context.EndFigure(true);
        }

        private static void CreateGeometry(StreamGeometryContext context, Rect boundRect, ShadowDecoratorGeometryKeypoints keypoints)
        {
            context.BeginFigure(keypoints.TopLeft, true);

            // Top
            context.LineTo(keypoints.TopRight);

            // TopRight corner
            var radiusX = boundRect.TopRight.X - keypoints.TopRight.X;
            var radiusY = keypoints.RightTop.Y - boundRect.TopRight.Y;
            if (radiusX != 0 || radiusY != 0)
            {
                context.ArcTo(keypoints.RightTop, new Size(radiusY, radiusY), 0, false, SweepDirection.Clockwise);
            }

            // Right
            context.LineTo(keypoints.RightBottom);

            // BottomRight corner
            radiusX = boundRect.BottomRight.X - keypoints.BottomRight.X;
            radiusY = boundRect.BottomRight.Y - keypoints.RightBottom.Y;
            if (radiusX != 0 || radiusY != 0)
            {
                context.ArcTo(keypoints.BottomRight, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise);
            }

            // Bottom
            context.LineTo(keypoints.BottomLeft);

            // BottomLeft corner
            radiusX = keypoints.BottomLeft.X - boundRect.BottomLeft.X;
            radiusY = boundRect.BottomLeft.Y - keypoints.LeftBottom.Y;
            if (radiusX != 0 || radiusY != 0)
            {
                context.ArcTo(keypoints.LeftBottom, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise);
            }

            // Left
            context.LineTo(keypoints.LeftTop);

            // TopLeft corner
            radiusX = keypoints.TopLeft.X - boundRect.TopLeft.X;
            radiusY = keypoints.LeftTop.Y - boundRect.TopLeft.Y;

            if (radiusX != 0 || radiusY != 0)
            {
                context.ArcTo(keypoints.TopLeft, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise);
            }

            context.EndFigure(true);
        }

        private class ShadowDecoratorGeometryKeypoints
        {
            internal CornerRadius originalRadius;
            internal ShadowDecoratorGeometryKeypoints(Rect boundRect, Thickness shadowDecoratorThickness, CornerRadius cornerRadius, bool inner)
            {
                originalRadius = cornerRadius;
                var left = 0.5 * shadowDecoratorThickness.Left;
                var top = 0.5 * shadowDecoratorThickness.Top;
                var right = 0.5 * shadowDecoratorThickness.Right;
                var bottom = 0.5 * shadowDecoratorThickness.Bottom;

                double leftTopY;
                double topLeftX;
                double topRightX;
                double rightTopY;
                double rightBottomY;
                double bottomRightX;
                double bottomLeftX;
                double leftBottomY;

                if (inner)
                {
                    leftTopY = Math.Max(0, cornerRadius.TopLeft - top) + boundRect.TopLeft.Y;
                    topLeftX = Math.Max(0, cornerRadius.TopLeft - left) + boundRect.TopLeft.X;
                    topRightX = boundRect.Width - Math.Max(0, cornerRadius.TopRight - top) + boundRect.TopLeft.X;
                    rightTopY = Math.Max(0, cornerRadius.TopRight - right) + boundRect.TopLeft.Y;
                    rightBottomY = boundRect.Height - Math.Max(0, cornerRadius.BottomRight - bottom) + boundRect.TopLeft.Y;
                    bottomRightX = boundRect.Width - Math.Max(0, cornerRadius.BottomRight - right) + boundRect.TopLeft.X;
                    bottomLeftX = Math.Max(0, cornerRadius.BottomLeft - left) + boundRect.TopLeft.X;
                    leftBottomY = boundRect.Height - Math.Max(0, cornerRadius.BottomLeft - bottom) + boundRect.TopLeft.Y;
                }
                else
                {
                    leftTopY = cornerRadius.TopLeft + top + boundRect.TopLeft.Y;
                    topLeftX = cornerRadius.TopLeft + left + boundRect.TopLeft.X;
                    topRightX = boundRect.Width - (cornerRadius.TopRight + right) + boundRect.TopLeft.X;
                    rightTopY = cornerRadius.TopRight + top + boundRect.TopLeft.Y;
                    rightBottomY = boundRect.Height - (cornerRadius.BottomRight + bottom) + boundRect.TopLeft.Y;
                    bottomRightX = boundRect.Width - (cornerRadius.BottomRight + right) + boundRect.TopLeft.X;
                    bottomLeftX = cornerRadius.BottomLeft + left + boundRect.TopLeft.X;
                    leftBottomY = boundRect.Height - (cornerRadius.BottomLeft + bottom) + boundRect.TopLeft.Y;
                }

                var leftTopX = boundRect.TopLeft.X;
                var topLeftY = boundRect.TopLeft.Y;
                var topRightY = boundRect.TopLeft.Y;
                var rightTopX = boundRect.Width + boundRect.TopLeft.X;
                var rightBottomX = boundRect.Width + boundRect.TopLeft.X;
                var bottomRightY = boundRect.Height + boundRect.TopLeft.Y;
                var bottomLeftY = boundRect.Height + boundRect.TopLeft.Y;
                var leftBottomX = boundRect.TopLeft.X;

                LeftTop = new Point(leftTopX, leftTopY);
                TopLeft = new Point(topLeftX, topLeftY);
                TopRight = new Point(topRightX, topRightY);
                RightTop = new Point(rightTopX, rightTopY);
                RightBottom = new Point(rightBottomX, rightBottomY);
                BottomRight = new Point(bottomRightX, bottomRightY);
                BottomLeft = new Point(bottomLeftX, bottomLeftY);
                LeftBottom = new Point(leftBottomX, leftBottomY);

                // Fix overlap
                if (TopLeft.X > TopRight.X)
                {
                    var scaledX = topLeftX / (topLeftX + topRightX) * boundRect.Width;
                    TopLeft = new Point(scaledX, TopLeft.Y);
                    TopRight = new Point(scaledX, TopRight.Y);
                }

                if (RightTop.Y > RightBottom.Y)
                {
                    var scaledY = rightBottomY / (rightTopY + rightBottomY) * boundRect.Height;
                    RightTop = new Point(RightTop.X, scaledY);
                    RightBottom = new Point(RightBottom.X, scaledY);
                }

                if (BottomRight.X < BottomLeft.X)
                {
                    var scaledX = bottomLeftX / (bottomLeftX + bottomRightX) * boundRect.Width;
                    BottomRight = new Point(scaledX, BottomRight.Y);
                    BottomLeft = new Point(scaledX, BottomLeft.Y);
                }

                if (LeftBottom.Y < LeftTop.Y)
                {
                    var scaledY = leftTopY / (leftTopY + leftBottomY) * boundRect.Height;
                    LeftBottom = new Point(LeftBottom.X, scaledY);
                    LeftTop = new Point(LeftTop.X, scaledY);
                }
            }

            internal Point LeftTop { get; }

            internal Point TopLeft { get; }

            internal Point TopRight { get; }

            internal Point RightTop { get; }

            internal Point RightBottom { get; }

            internal Point BottomRight { get; }

            internal Point BottomLeft { get; }

            internal Point LeftBottom { get; }
        }
    }
}
