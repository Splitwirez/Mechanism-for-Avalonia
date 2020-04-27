using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls
{
    public partial class ShadowDecorator : Decorator
    {
        public static readonly StyledProperty<Color> ColorProperty =
            AvaloniaProperty.Register<ShadowDecorator, Color>(nameof(Color));

        public static readonly StyledProperty<double> DepthProperty =
            AvaloniaProperty.Register<ShadowDecorator, double>(nameof(Depth));

        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
            AvaloniaProperty.Register<ShadowDecorator, CornerRadius>(nameof(CornerRadius));

        private readonly ShadowRenderHelper _shadowRenderHelper = new ShadowRenderHelper();

        static ShadowDecorator()
        {
            AffectsRender<ShadowDecorator>(
                ColorProperty,
                DepthProperty,
                CornerRadiusProperty);
            AffectsMeasure<ShadowDecorator>(DepthProperty);
        }

        public Color Color
        {
            get { return GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public double Depth
        {
            get { return GetValue(DepthProperty); }
            set { SetValue(DepthProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public override void Render(DrawingContext context)
        {
            _shadowRenderHelper.Render(context, Bounds.Size, Depth, CornerRadius, Color);
        }

        static Thickness ZeroThickness = new Thickness(0);

        protected override Size MeasureOverride(Size availableSize)
        {
            return LayoutHelper.MeasureChild(Child, availableSize, Padding, ZeroThickness);
        }

        /// <summary>
        /// Arranges the control's child.
        /// </summary>
        /// <param name="finalSize">The size allocated to the control.</param>
        /// <returns>The space taken.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _shadowRenderHelper.Update(finalSize, Depth, CornerRadius);

            return LayoutHelper.ArrangeChild(Child, finalSize, Padding, ZeroThickness);
        }
    }

    internal class ShadowRenderHelper
    {
        private StreamGeometry _topLeftGeometryCache;
        private RectangleGeometry _topCenterGeometryCache;
        private StreamGeometry _topRightGeometryCache;

        private RectangleGeometry _middleLeftGeometryCache;
        private StreamGeometry _middleCenterGeometryCache;
        private RectangleGeometry _middleRightGeometryCache;

        private StreamGeometry _bottomLeftGeometryCache;
        private RectangleGeometry _bottomCenterGeometryCache;
        private StreamGeometry _bottomRightGeometryCache;

        public void Update(Size size, double depth, CornerRadius radii)
        {
            var boundRect = new Rect(size);
            var innerRect = boundRect.Deflate(depth);
            BorderGeometryKeypoints backgroundKeypoints = null;
            StreamGeometry backgroundGeometry = null;

            if (innerRect.Width != 0 && innerRect.Height != 0)
            {
                //new RectangleGeometry()
                //backgroundGeometry = new StreamGeometry();
                //backgroundKeypoints = new BorderGeometryKeypoints(innerRect, new Thickness(depth), radii, true);

                /*using (var ctx = backgroundGeometry.Open())
                {
                    CreateGeometry(ctx, innerRect, backgroundKeypoints);
                }*/

                _middleCenterGeometryCache = backgroundGeometry;
            }
            else
            {
                _middleCenterGeometryCache = null;
            }

            /*if (boundRect.Width != 0 && innerRect.Height != 0)
            {*/
            //var borderGeometryKeypoints = new BorderGeometryKeypoints(boundRect, depth, radii, false);
            //borderGeometryKeypoints.
            var borderGeometry = new StreamGeometry();

            /*if (backgroundGeometry != null)
            {
                using (var ctx = borderGeometry.Open())
                {
                    CreateGeometry(ctx, innerRect, backgroundKeypoints);
                }
            }*/

            if (depth > 0)
            {
                double outer = depth + radii.TopLeft;

                _topLeftGeometryCache = GetCornerGeometry(new Point(0, outer), new Point(outer, 0), new Point(outer, depth), new Point(depth, outer), outer, radii.TopLeft, 0, 0);

                double baseCenterWidth = size.Width - (depth * 2);
                _topCenterGeometryCache = new RectangleGeometry(new Rect(outer, 0, baseCenterWidth - (radii.TopLeft + radii.TopRight), depth));

                outer = depth + radii.TopRight;
                _topRightGeometryCache = GetCornerGeometry(new Point(0, 0), new Point(outer, outer), new Point(radii.TopRight, outer), new Point(0, depth), outer, radii.TopRight, size.Width - outer, 0);

                _bottomCenterGeometryCache = new RectangleGeometry(new Rect(outer, size.Height - depth, baseCenterWidth - (radii.BottomLeft + radii.BottomRight), depth));
            }
            else
            {
                RectangleGeometry rectG = new RectangleGeometry(new Rect(0, 0, 0, 0));
                StreamGeometry streamG = new StreamGeometry();
                using (var ctx = streamG.Open())
                {
                    ctx.BeginFigure(new Point(0, 0), true);
                    ctx.EndFigure(true);
                }
                _topLeftGeometryCache = streamG;
                _middleLeftGeometryCache = rectG;
                _topRightGeometryCache = streamG;
                _topCenterGeometryCache = rectG;
                _bottomRightGeometryCache = streamG;
                _middleRightGeometryCache = rectG;
                _bottomLeftGeometryCache = streamG;
                _bottomCenterGeometryCache = rectG;
            }
        }

        private static StreamGeometry GetCornerGeometry(Point outerStart, Point outerEnd, Point innerEnd, Point innerStart, double outerRadius, double innerRadius, double xOffset, double yOffset)
        {
            outerStart = outerStart.WithOffset(xOffset, yOffset);
            outerEnd = outerEnd.WithOffset(xOffset, yOffset);
            innerEnd = innerEnd.WithOffset(xOffset, yOffset);
            innerStart = innerStart.WithOffset(xOffset, yOffset);

            Size outerRd = new Size(outerRadius, outerRadius);
            Size innerRd = new Size(innerRadius, innerRadius);
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(outerStart, true);
                ctx.ArcTo(outerEnd, outerRd, 90, false, SweepDirection.Clockwise);
                //ctx.LineTo(outerEnd);
                ctx.LineTo(innerEnd);
                ctx.ArcTo(innerStart, innerRd, -90, false, SweepDirection.CounterClockwise);
                //ctx.LineTo(innerStart);
                ctx.EndFigure(true);
            }
            //geometry.Transform = new TranslateTransform(xOffset, yOffset);
            return geometry;
        }

        public void Render(DrawingContext context, Size size, double depth, CornerRadius radii, Color bg)
        {
            context.FillRectangle(new SolidColorBrush(Colors.LightSkyBlue), new Rect(size));
            var blue = new SolidColorBrush(Colors.GreenYellow);
            double totalInset = radii.TopLeft + depth;
            context.FillRectangle(blue, new Rect(0, 0, totalInset, totalInset));
            totalInset = radii.TopRight + depth;
            context.FillRectangle(blue, new Rect(size.Width - totalInset, 0, totalInset, totalInset));
            var backgroundGeometry = _middleCenterGeometryCache;
            Color bgTrans = new Color(0, bg.R, bg.G, bg.B);

            GradientStops stops = new GradientStops()
            {
                new GradientStop(bg, 0),
                new GradientStop(bgTrans, 1)
            };

            if (backgroundGeometry != null)
            {
                context.DrawGeometry(new SolidColorBrush(bg), null, backgroundGeometry);
            };

            RelativePoint tlOrigin = new RelativePoint(radii.TopLeft + depth, radii.TopLeft + depth, RelativeUnit.Absolute);
            double innerStopPos = radii.TopLeft / (depth + radii.TopLeft);
            context.DrawGeometry(new RadialGradientBrush()
            {
                Center = tlOrigin,
                GradientOrigin = tlOrigin,
                Radius = 1,
                GradientStops = new GradientStops()
                {
                    new GradientStop(bg, innerStopPos),
                    new GradientStop(bgTrans, 1)
                }
            }, null, _topLeftGeometryCache);

            context.DrawGeometry(new LinearGradientBrush()
            {
                GradientStops = stops,
                StartPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 0, RelativeUnit.Relative)
            }, null, _topCenterGeometryCache);

            tlOrigin = new RelativePoint(size.Width - (radii.TopRight + depth), radii.TopRight + depth, RelativeUnit.Absolute);
            innerStopPos = radii.TopRight / (depth + radii.TopRight);
            context.DrawGeometry(new RadialGradientBrush()
            {
                Center = tlOrigin,
                GradientOrigin = tlOrigin,
                Radius = 1,
                GradientStops = new GradientStops()
                {
                    new GradientStop(bg, innerStopPos),
                    new GradientStop(bgTrans, 1)
                }
            }, null, _topRightGeometryCache);


            context.DrawGeometry(new LinearGradientBrush()
            {
                GradientStops = stops,
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative)
            }, null, _bottomCenterGeometryCache);
        }

        private static void CreateGeometry(StreamGeometryContext context, Rect boundRect, BorderGeometryKeypoints keypoints)
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

        private class BorderGeometryKeypoints
        {
            internal BorderGeometryKeypoints(Rect boundRect, Thickness borderThickness, CornerRadius cornerRadius, bool inner)
            {
                var left = 0.5 * borderThickness.Left;
                var top = 0.5 * borderThickness.Top;
                var right = 0.5 * borderThickness.Right;
                var bottom = 0.5 * borderThickness.Bottom;

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

    static class Extensions
    {
        public static Point WithOffset(this Point point, double x, double y)
        {
            return point.WithX(point.X + x).WithY(point.Y + y);
        }
    }
}
