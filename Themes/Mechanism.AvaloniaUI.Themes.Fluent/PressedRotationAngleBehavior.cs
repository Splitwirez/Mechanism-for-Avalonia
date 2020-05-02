using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mechanism.AvaloniaUI.Themes.Fluent
{
    public class PressedRotationAngleBehavior : Behavior<Control>
    {
        SkewTransform _skewTransform = new SkewTransform();
        ScaleTransform _scaleTransform = new ScaleTransform();
        MatrixTransform _transform = new MatrixTransform();
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.RenderTransform = /*_transform; */new TransformGroup()
            {
                Children =
                {
                    _skewTransform,
                    _scaleTransform
                }
            };
            AssociatedObject.PointerPressed += AssociatedObject_PointerPressed;
            AssociatedObject.PointerReleased += AssociatedObject_PointerReleased;
        }

        private void AssociatedObject_PointerReleased(object sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            //_transform.Matrix = Matrix.CreateTranslation(0, 0); //new Matrix(0, 0, 0, 0, 0, 0);
            //Debug.WriteLine("NO TRANSLATION MATRIX: " + _transform.Matrix.ToString());
            _skewTransform.AngleX = 0;
            _skewTransform.AngleY = 0;
            _scaleTransform.ScaleX = 1;
            _scaleTransform.ScaleY = 1;
        }

        private void AssociatedObject_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            Point curPoint = e.GetPosition(AssociatedObject);
            bool right = curPoint.X > (AssociatedObject.Bounds.Width / 2);
            bool bottom = curPoint.Y > (AssociatedObject.Bounds.Height / 2);
            /*double h = (curPoint.X / AssociatedObject.Bounds.Width) / 100;
            double v = (curPoint.Y / AssociatedObject.Bounds.Height) / 100;


            _transform.Matrix = new Matrix(1 - Math.Abs(h - 0.5), v, h, 1 - Math.Abs(v - 0.5), 0, 0);
            Debug.WriteLine("INFO: " + curPoint.ToString() + "\n" + _transform.Matrix.ToString());

            //NO TRANSLATION MATRIX: { {M11:1 M12:0} {M21:0 M22:1} {M31:0 M32:0} }
            */
            if (right == bottom)
            {
                _skewTransform.AngleX = -1;
                _skewTransform.AngleY = -1;
            }
            else
            {
                _skewTransform.AngleX = 1;
                _skewTransform.AngleY = 1;
            }
            Debug.WriteLine("MATRIX: " + _skewTransform.Value.ToString());
            _scaleTransform.ScaleX = (AssociatedObject.Bounds.Width - Math.Abs(_skewTransform.Value.M21 * 10)) / AssociatedObject.Bounds.Width;
            _scaleTransform.ScaleY = (AssociatedObject.Bounds.Height - Math.Abs(_skewTransform.Value.M12 * 10)) / AssociatedObject.Bounds.Height;
            //_scaleTransform.Value.
            //_scaleTransform.ScaleX = AssociatedObject.Bounds.Width / (_skewTransform.AngleX / AssociatedObject.Bounds.Width);
            //Matrix.CreateSkew(Matrix.ToRadians)

            //_scaleTransform.ScaleY = AssociatedObject.Bounds.Height / AssociatedObject.TransformedBounds.Value.Bounds.Height;
        }
    }
}
