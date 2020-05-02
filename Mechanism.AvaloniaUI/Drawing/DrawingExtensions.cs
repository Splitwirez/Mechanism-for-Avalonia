using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mechanism.AvaloniaUI.Drawing
{
    public static class DrawingExtensions
    {
        public static System.Drawing.Image ToDrawingImage(this Avalonia.Media.Imaging.Bitmap bmp)
        {
            System.Drawing.Image ret;
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream);
                ret = System.Drawing.Image.FromStream(stream);
            }
            return ret;
        }

        public static System.Drawing.Bitmap ToDrawingBitmap(this Avalonia.Media.Imaging.Bitmap bmp) => (System.Drawing.Bitmap)bmp.ToDrawingImage();

        public static Avalonia.Media.Imaging.Bitmap ToMediaBitmap(this System.Drawing.Bitmap bmp)
        {
            Avalonia.Media.Imaging.Bitmap ret;
            /*var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                ret = new Avalonia.Media.Imaging.Bitmap(stream);
            }
            return ret;*/
            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                stream.Position = 0;

                //var factory = Avalonia.AvaloniaLocator.Current.GetService(typeof(Avalonia.Platform.IPlatformRenderInterface));

                //ret = factory.LoadBitmap(stream);
                ret = new Avalonia.Media.Imaging.Bitmap(stream);
            }
            return ret;
        }

        public static System.Drawing.PointF ToDrawingPointF(this Avalonia.Point pnt)
        {
            return new System.Drawing.PointF((float)pnt.X, (float)pnt.Y);
        }

        public static Avalonia.Point ToAvaloniaPoint(this System.Drawing.PointF pnt)
        {
            return new Avalonia.Point(pnt.X, pnt.Y);
        }
    }
}
