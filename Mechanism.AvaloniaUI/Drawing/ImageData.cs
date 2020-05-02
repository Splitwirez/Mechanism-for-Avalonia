/*using System;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mechanism.AvaloniaUI.Drawing
{
    /// <summary>
    /// Using InteropServices.Marshal mathods to get image channels (R,G,B,A) byte
    /// </summary>
    public class ImageData : IDisposable
    {
        private byte[,] _red, _green, _blue, _alpha;
        private bool _disposed = false;

        public byte[,] A
        {
            get { return _alpha; }
            set { _alpha = value; }
        }
        public byte[,] B
        {
            get { return _blue; }
            set { _blue = value; }
        }
        public byte[,] G
        {
            get { return _green; }
            set { _green = value; }
        }
        public byte[,] R
        {
            get { return _red; }
            set { _red = value; }
        }

        public ImageData Clone()
        {
            ImageData cb = new ImageData();
            cb.A = (byte[,])_alpha.Clone();
            cb.B = (byte[,])_blue.Clone();
            cb.G = (byte[,])_green.Clone();
            cb.R = (byte[,])_red.Clone();
            return cb;
        }

        # region InteropServices.Marshal mathods
        public void FromBitmap(Bitmap srcBmp)
        {
            int w = srcBmp.Width;
            int h = srcBmp.Height;

            _alpha = new byte[w, h];
            _blue = new byte[w, h];
            _green = new byte[w, h];
            _red = new byte[w, h];

            // Lock the bitmap's bits.  
            System.Drawing.Imaging.BitmapData bmpData = srcBmp.LockBits(new Rectangle(0, 0, w, h),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * srcBmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            int offset = bmpData.Stride - w * 4;

            int index = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    _blue[x, y] = rgbValues[index];
                    _green[x, y] = rgbValues[index + 1];
                    _red[x, y] = rgbValues[index + 2];
                    _alpha[x, y] = rgbValues[index + 3];
                    index += 4;
                }
                index += offset;
            }

            // Unlock the bits.
            srcBmp.UnlockBits(bmpData);
        }

        public Bitmap ToBitmap()
        {
            int width = 0, height = 0;
            if (_alpha != null)
            {
                width = Math.Max(width, _alpha.GetLength(0));
                height = Math.Max(height, _alpha.GetLength(1));
            }
            if (_blue != null)
            {
                width = Math.Max(width, _blue.GetLength(0));
                height = Math.Max(height, _blue.GetLength(1));
            }
            if (_green != null)
            {
                width = Math.Max(width, _green.GetLength(0));
                height = Math.Max(height, _green.GetLength(1));
            }
            if (_red != null)
            {
                width = Math.Max(width, _red.GetLength(0));
                height = Math.Max(height, _red.GetLength(1));
            }
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // set rgbValues
            int offset = bmpData.Stride - width * 4;
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    rgbValues[i] = checkArray(_blue, x, y) ? _blue[x, y] : (byte)0;
                    rgbValues[i + 1] = checkArray(_green, x, y) ? _green[x, y] : (byte)0;
                    rgbValues[i + 2] = checkArray(_red, x, y) ? _red[x, y] : (byte)0;
                    rgbValues[i + 3] = checkArray(_alpha, x, y) ? _alpha[x, y] : (byte)255;
                    i += 4;
                }
                i += offset;
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return bmp;
        }
        # endregion

        private static bool checkArray(byte[,] array, int x, int y)
        {
            if (array == null) return false;
            if (x < array.GetLength(0) && y < array.GetLength(1))
                return true;
            else return false;
        }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            if (!_disposed)
            {
                if (disposing)
                {
                    _alpha = null;
                    _blue = null;
                    _green = null;
                    _red = null;
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }


    /*public class ImageData : IDisposable
    {
        private double[,] _red, _green, _blue, _alpha;
        private bool _disposed = false;

        public double[,] A
        {
            get { return _alpha; }
            set { _alpha = value; }
        }
        public double[,] B
        {
            get { return _blue; }
            set { _blue = value; }
        }
        public double[,] G
        {
            get { return _green; }
            set { _green = value; }
        }
        public double[,] R
        {
            get { return _red; }
            set { _red = value; }
        }

        public ImageData Clone()
        {
            ImageData cb = new ImageData();
            cb.A = (double[,])_alpha.Clone();
            cb.B = (double[,])_blue.Clone();
            cb.G = (double[,])_green.Clone();
            cb.R = (double[,])_red.Clone();
            return cb;
        }

        # region InteropServices.Marshal mathods
        public void FromBitmap(Bitmap srcBmp)
        {
            int w = srcBmp.PixelSize.Width;
            int h = srcBmp.PixelSize.Height;

            _alpha = new double[w, h];
            _blue = new double[w, h];
            _green = new double[w, h];
            _red = new double[w, h];

            // Lock the bitmap's bits.  
            /*System.Drawing.Imaging.BitmapData bmpData = srcBmp.LockBits(new Rectangle(0, 0, w, h),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);*
            // Get the address of the first line.
            //IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            //int bytes = bmpData.Stride * srcBmp.Height;
            int bytes = (srcBmp.PixelSize.Width * srcBmp.PixelSize.Height) * 4 + 4;
            double[] rgbValues = new double[bytes];

            // Copy the RGB values
            //System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            //int offset = bmpData.Stride - w * 4;

            int index = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    _blue[x, y] = rgbValues[index];
                    _green[x, y] = rgbValues[index + 1];
                    _red[x, y] = rgbValues[index + 2];
                    _alpha[x, y] = rgbValues[index + 3];
                    index += 4;
                }
                index += 4; //offset;
            }

            // Unlock the bits.
            //srcBmp.UnlockBits(bmpData);
        }

        public Bitmap ToBitmap()
        {
            int width = 0, height = 0;
            if (_alpha != null)
            {
                width = Math.Max(width, _alpha.GetLength(0));
                height = Math.Max(height, _alpha.GetLength(1));
            }
            if (_blue != null)
            {
                width = Math.Max(width, _blue.GetLength(0));
                height = Math.Max(height, _blue.GetLength(1));
            }
            if (_green != null)
            {
                width = Math.Max(width, _green.GetLength(0));
                height = Math.Max(height, _green.GetLength(1));
            }
            if (_red != null)
            {
                width = Math.Max(width, _red.GetLength(0));
                height = Math.Max(height, _red.GetLength(1));
            }
            /*Bitmap bmp = new Bitmap(pixel width, height, PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);*
            WriteableBitmap bmp = new WriteableBitmap(new PixelSize(width, height), new Avalonia.Vector(96, 96));
            ILockedFramebuffer bmpData = bmp.Lock();
            // Get the address of the first line.
            IntPtr ptr = bmpData.Address;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.RowBytes * bmp.PixelSize.Height;
            double[] rgbValues = new double[bytes];

            // set rgbValues
            int offset = bmpData.RowBytes - width * 4;
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    rgbValues[i] = checkArray(_blue, x, y) ? _blue[x, y] : (byte)0;
                    rgbValues[i + 1] = checkArray(_green, x, y) ? _green[x, y] : (byte)0;
                    rgbValues[i + 2] = checkArray(_red, x, y) ? _red[x, y] : (byte)0;
                    rgbValues[i + 3] = checkArray(_alpha, x, y) ? _alpha[x, y] : (byte)255;
                    i += 4;
                }
                i += offset;
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            //bmp.Dispose .UnlockBits(bmpData);
            return bmp;
        }
        # endregion

        private static bool checkArray(double[,] array, int x, int y)
        {
            if (array == null) return false;
            if (x < array.GetLength(0) && y < array.GetLength(1))
                return true;
            else return false;
        }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            if (!_disposed)
            {
                if (disposing)
                {
                    _alpha = null;
                    _blue = null;
                    _green = null;
                    _red = null;
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }*/
}