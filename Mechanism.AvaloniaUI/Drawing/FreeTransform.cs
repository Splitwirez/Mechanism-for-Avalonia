using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace Mechanism.AvaloniaUI.Drawing
{
    public class FreeTransform
    {
        PointF[] vertex = new PointF[4];
        Vector AB, BC, CD, DA;
        Rectangle rect = new Rectangle();
        ImageData srcCB = new ImageData();
        int srcW = 0;
        int srcH = 0;

        public Bitmap Bitmap
        {
            set
            {
                try
                {
                    srcCB.FromBitmap(value);
                    srcH = value.Height;
                    srcW = value.Width;
                }
                catch
                {
                    srcW = 0; srcH = 0;
                }
            }
            get
            {
                return getTransformedBitmap();
            }
        }

        public Point ImageLocation
        {
            set { rect.Location = value; }
            get { return rect.Location; }
        }

        bool isBilinear = false;
        public bool IsBilinearInterpolation
        {
            set { isBilinear = value; }
            get { return isBilinear; }
        }

        public int ImageWidth
        {
            get { return rect.Width; }
        }

        public int ImageHeight
        {
            get { return rect.Height; }
        }

        public PointF TopLeftVertex
        {
            set { vertex[0] = value; setVertex(); }
            get { return vertex[0]; }
        }

        public PointF TopRightVertex
        {
            set { vertex[1] = value; setVertex(); }
            get { return vertex[1]; }
        }

        public PointF BottomRightVertex
        {
            set { vertex[2] = value; setVertex(); }
            get { return vertex[2]; }
        }

        public PointF BottomLeftVertex
        {
            set { vertex[3] = value; setVertex(); }
            get { return vertex[3]; }
        }

        public PointF[] Vertices
        {
            set { vertex = value; setVertex(); }
            get { return vertex; }
        }

        private void setVertex()
        {
            float xmin = float.MaxValue;
            float ymin = float.MaxValue;
            float xmax = float.MinValue;
            float ymax = float.MinValue;

            for (int i = 0; i < 4; i++)
            {
                xmax = Math.Max(xmax, vertex[i].X);
                ymax = Math.Max(ymax, vertex[i].Y);
                xmin = Math.Min(xmin, vertex[i].X);
                ymin = Math.Min(ymin, vertex[i].Y);
            }

            rect = new Rectangle((int)xmin, (int)ymin, (int)(xmax - xmin), (int)(ymax - ymin));

            AB = new Vector(vertex[0], vertex[1]);
            BC = new Vector(vertex[1], vertex[2]);
            CD = new Vector(vertex[2], vertex[3]);
            DA = new Vector(vertex[3], vertex[0]);

            // get unit vector
            AB /= AB.Magnitude;
            BC /= BC.Magnitude;
            CD /= CD.Magnitude;
            DA /= DA.Magnitude;
        }

        private bool isOnPlaneABCD(PointF pt) //  including point on border
        {
            if (!Vector.IsCCW(pt, vertex[0], vertex[1]))
            {
                if (!Vector.IsCCW(pt, vertex[1], vertex[2]))
                {
                    if (!Vector.IsCCW(pt, vertex[2], vertex[3]))
                    {
                        if (!Vector.IsCCW(pt, vertex[3], vertex[0]))
                            return true;
                    }
                }
            }
            return false;
        }

        private Bitmap getTransformedBitmap()
        {
            if (srcH == 0 || srcW == 0) return null;

            ImageData destCB = new ImageData();
            destCB.A = new byte[rect.Width, rect.Height];
            destCB.B = new byte[rect.Width, rect.Height];
            destCB.G = new byte[rect.Width, rect.Height];
            destCB.R = new byte[rect.Width, rect.Height];




            //for (int y = 0; y < rect.Height; y++)     
            Parallel.For(0, rect.Height, y =>
            {
                //for (int x = 0; x < rect.Width; x++)
                Parallel.For(0, rect.Width, x =>
                {
                    PointF ptInPlane = new PointF();
                    int x1, x2, y1, y2;
                    double dab, dbc, dcd, dda;
                    float dx1, dx2, dy1, dy2, dx1y1, dx1y2, dx2y1, dx2y2, nbyte;

                    Point srcPt = new Point(x, y);
                    srcPt.Offset(this.rect.Location);

                    if (isOnPlaneABCD(srcPt))
                    {
                        dab = Math.Abs((new Vector(vertex[0], srcPt)).CrossProduct(AB));
                        dbc = Math.Abs((new Vector(vertex[1], srcPt)).CrossProduct(BC));
                        dcd = Math.Abs((new Vector(vertex[2], srcPt)).CrossProduct(CD));
                        dda = Math.Abs((new Vector(vertex[3], srcPt)).CrossProduct(DA));
                        ptInPlane.X = (float)(srcW * (dda / (dda + dbc)));
                        ptInPlane.Y = (float)(srcH * (dab / (dab + dcd)));

                        x1 = (int)ptInPlane.X;
                        y1 = (int)ptInPlane.Y;

                        if (x1 >= 0 && x1 < srcW && y1 >= 0 && y1 < srcH)
                        {
                            if (isBilinear)
                            {
                                x2 = (x1 == srcW - 1) ? x1 : x1 + 1;
                                y2 = (y1 == srcH - 1) ? y1 : y1 + 1;

                                dx1 = ptInPlane.X - (float)x1;
                                if (dx1 < 0) dx1 = 0;
                                dx1 = 1f - dx1;
                                dx2 = 1f - dx1;
                                dy1 = ptInPlane.Y - (float)y1;
                                if (dy1 < 0) dy1 = 0;
                                dy1 = 1f - dy1;
                                dy2 = 1f - dy1;

                                dx1y1 = dx1 * dy1;
                                dx1y2 = dx1 * dy2;
                                dx2y1 = dx2 * dy1;
                                dx2y2 = dx2 * dy2;


                                nbyte = srcCB.A[x1, y1] * dx1y1 + srcCB.A[x2, y1] * dx2y1 + srcCB.A[x1, y2] * dx1y2 + srcCB.A[x2, y2] * dx2y2;
                                destCB.A[x, y] = (byte)nbyte;
                                nbyte = srcCB.B[x1, y1] * dx1y1 + srcCB.B[x2, y1] * dx2y1 + srcCB.B[x1, y2] * dx1y2 + srcCB.B[x2, y2] * dx2y2;
                                destCB.B[x, y] = (byte)nbyte;
                                nbyte = srcCB.G[x1, y1] * dx1y1 + srcCB.G[x2, y1] * dx2y1 + srcCB.G[x1, y2] * dx1y2 + srcCB.G[x2, y2] * dx2y2;
                                destCB.G[x, y] = (byte)nbyte;
                                nbyte = srcCB.R[x1, y1] * dx1y1 + srcCB.R[x2, y1] * dx2y1 + srcCB.R[x1, y2] * dx1y2 + srcCB.R[x2, y2] * dx2y2;
                                destCB.R[x, y] = (byte)nbyte;
                            }
                            else
                            {
                                destCB.A[x, y] = srcCB.A[x1, y1];
                                destCB.B[x, y] = srcCB.B[x1, y1];
                                destCB.G[x, y] = srcCB.G[x1, y1];
                                destCB.R[x, y] = srcCB.R[x1, y1];
                            }
                        }
                    }
                });
            });
            return destCB.ToBitmap();
        }
    }

    public struct Vector
    {
        double _x, _y;

        public Vector(double x, double y)
        {
            _x = x; _y = y;
        }
        public Vector(PointF pt)
        {
            _x = pt.X;
            _y = pt.Y;
        }
        public Vector(PointF st, PointF end)
        {
            _x = end.X - st.X;
            _y = end.Y - st.Y;
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Magnitude
        {
            get { return Math.Sqrt(X * X + Y * Y); }
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector operator -(Vector v)
        {
            return new Vector(-v.X, -v.Y);
        }

        public static Vector operator *(double c, Vector v)
        {
            return new Vector(c * v.X, c * v.Y);
        }

        public static Vector operator *(Vector v, double c)
        {
            return new Vector(c * v.X, c * v.Y);
        }

        public static Vector operator /(Vector v, double c)
        {
            return new Vector(v.X / c, v.Y / c);
        }

        // A * B =|A|.|B|.sin(angle AOB)
        public double CrossProduct(Vector v)
        {
            return _x * v.Y - v.X * _y;
        }

        // A. B=|A|.|B|.cos(angle AOB)
        public double DotProduct(Vector v)
        {
            return _x * v.X + _y * v.Y;
        }

        public static bool IsClockwise(PointF pt1, PointF pt2, PointF pt3)
        {
            Vector V21 = new Vector(pt2, pt1);
            Vector v23 = new Vector(pt2, pt3);
            return V21.CrossProduct(v23) < 0; // sin(angle pt1 pt2 pt3) > 0, 0<angle pt1 pt2 pt3 <180
        }

        public static bool IsCCW(PointF pt1, PointF pt2, PointF pt3)
        {
            Vector V21 = new Vector(pt2, pt1);
            Vector v23 = new Vector(pt2, pt3);
            return V21.CrossProduct(v23) > 0;  // sin(angle pt2 pt1 pt3) < 0, 180<angle pt2 pt1 pt3 <360
        }

        public static double DistancePointLine(PointF pt, PointF lnA, PointF lnB)
        {
            Vector v1 = new Vector(lnA, lnB);
            Vector v2 = new Vector(lnA, pt);
            v1 /= v1.Magnitude;
            return Math.Abs(v2.CrossProduct(v1));
        }

        public void Rotate(int Degree)
        {
            double radian = Degree * Math.PI / 180.0;
            double sin = Math.Sin(radian);
            double cos = Math.Cos(radian);
            double nx = _x * cos - _y * sin;
            double ny = _x * sin + _y * cos;
            _x = nx;
            _y = ny;
        }

        public PointF ToPointF()
        {
            return new PointF((float)_x, (float)_y);
        }
    }

    /*using System;
    using Rectangle = System.Drawing.Rectangle;
    using Bitmap = Avalonia.Media.Imaging.Bitmap;
    using System.Threading.Tasks;
    using Avalonia;

    namespace Mechanism.AvaloniaUI.Drawing
    {
        public class FreeTransform
        {
            Point[] vertex = new Point[4];
            Vector AB, BC, CD, DA;
            Rectangle rect = new Rectangle();
            ImageData srcCB = new ImageData();
            int srcW = 0;
            int srcH = 0;

            public Bitmap Bitmap
            {
                set
                {
                    srcCB.FromBitmap(value);
                    srcH = value.PixelSize.Height;
                    srcW = value.PixelSize.Width;
                }
                get
                {
                    return getTransformedBitmap();
                }
            }

            public Point ImageLocation
            {
                set { rect.Location = new System.Drawing.Point((int)value.X, (int)value.Y); }
                get { return new Point(rect.Location.X, rect.Location.Y); }
            }

            bool isBilinear = false;
            public bool IsBilinearInterpolation
            {
                set { isBilinear = value; }
                get { return isBilinear; }
            }

            public int ImageWidth
            {
                get { return rect.Width; }
            }

            public int ImageHeight
            {
                get { return rect.Height; }
            }

            public Point TopLeftVertex
            {
                set { vertex[0] = value; setVertex(); }
                get { return vertex[0]; }
            }

            public Point TopRightVertex
            {
                set { vertex[1] = value; setVertex(); }
                get { return vertex[1]; }
            }

            public Point BottomRightVertex
            {
                set { vertex[2] = value; setVertex(); }
                get { return vertex[2]; }
            }

            public Point BottomLeftVertex
            {
                set { vertex[3] = value; setVertex(); }
                get { return vertex[3]; }
            }

            public Point[] FourCorners
            {
                set { vertex = value; setVertex(); }
                get { return vertex; }
            }

            private void setVertex()
            {
                float xmin = float.MaxValue;
                float ymin = float.MaxValue;
                float xmax = float.MinValue;
                float ymax = float.MinValue;

                for (int i = 0; i < 4; i++)
                {
                    xmax = Math.Max(xmax, (float)vertex[i].X);
                    ymax = Math.Max(ymax, (float)vertex[i].Y);
                    xmin = Math.Min(xmin, (float)vertex[i].X);
                    ymin = Math.Min(ymin, (float)vertex[i].Y);
                }

                rect = new Rectangle((int)xmin, (int)ymin, (int)(xmax - xmin), (int)(ymax - ymin));

                AB = new Vector(vertex[0], vertex[1]);
                BC = new Vector(vertex[1], vertex[2]);
                CD = new Vector(vertex[2], vertex[3]);
                DA = new Vector(vertex[3], vertex[0]);

                // get unit vector
                AB /= AB.Magnitude;
                BC /= BC.Magnitude;
                CD /= CD.Magnitude;
                DA /= DA.Magnitude;
            }

            private bool isOnPlaneABCD(Point pt) //  including point on border
            {
                if (!Vector.IsCCW(pt, vertex[0], vertex[1]))
                {
                    if (!Vector.IsCCW(pt, vertex[1], vertex[2]))
                    {
                        if (!Vector.IsCCW(pt, vertex[2], vertex[3]))
                        {
                            if (!Vector.IsCCW(pt, vertex[3], vertex[0]))
                                return true;
                        }
                    }
                }
                return false;
            }

            private Bitmap getTransformedBitmap()
            {
                if (srcH == 0 || srcW == 0) return null;

                ImageData destCB = new ImageData();
                destCB.A = new double[rect.Width, rect.Height];
                destCB.B = new double[rect.Width, rect.Height];
                destCB.G = new double[rect.Width, rect.Height];
                destCB.R = new double[rect.Width, rect.Height];




                //for (int y = 0; y < rect.Height; y++)     
                Parallel.For(0, rect.Height, y =>
                {
                    //for (int x = 0; x < rect.Width; x++)
                    Parallel.For(0, rect.Width, x =>
                    {
                        Point ptInPlane = new Point();
                        int x1, x2, y1, y2;
                        double dab, dbc, dcd, dda;
                        double dx1, dx2, dy1, dy2, dx1y1, dx1y2, dx2y1, dx2y2, nbyte;

                        Point srcPt = new Point(x, y);
                        //srcPt.Offset();

                        if (isOnPlaneABCD(srcPt))
                        {
                            dab = Math.Abs((new Vector(vertex[0], srcPt)).CrossProduct(AB));
                            dbc = Math.Abs((new Vector(vertex[1], srcPt)).CrossProduct(BC));
                            dcd = Math.Abs((new Vector(vertex[2], srcPt)).CrossProduct(CD));
                            dda = Math.Abs((new Vector(vertex[3], srcPt)).CrossProduct(DA));
                            ptInPlane = ptInPlane.WithX((float)(srcW * (dda / (dda + dbc))));
                            ptInPlane = ptInPlane.WithY((float)(srcH * (dab / (dab + dcd))));

                            x1 = (int)ptInPlane.X;
                            y1 = (int)ptInPlane.Y;

                            if (x1 >= 0 && x1 < srcW && y1 >= 0 && y1 < srcH)
                            {
                                if (isBilinear)
                                {
                                    x2 = (x1 == srcW - 1) ? x1 : x1 + 1;
                                    y2 = (y1 == srcH - 1) ? y1 : y1 + 1;

                                    dx1 = ptInPlane.X - (float)x1;
                                    if (dx1 < 0) dx1 = 0;
                                    dx1 = 1f - dx1;
                                    dx2 = 1f - dx1;
                                    dy1 = ptInPlane.Y - (float)y1;
                                    if (dy1 < 0) dy1 = 0;
                                    dy1 = 1f - dy1;
                                    dy2 = 1f - dy1;

                                    dx1y1 = dx1 * dy1;
                                    dx1y2 = dx1 * dy2;
                                    dx2y1 = dx2 * dy1;
                                    dx2y2 = dx2 * dy2;


                                    nbyte = srcCB.A[x1, y1] * dx1y1 + srcCB.A[x2, y1] * dx2y1 + srcCB.A[x1, y2] * dx1y2 + srcCB.A[x2, y2] * dx2y2;
                                    destCB.A[x, y] = (byte)nbyte;
                                    nbyte = srcCB.B[x1, y1] * dx1y1 + srcCB.B[x2, y1] * dx2y1 + srcCB.B[x1, y2] * dx1y2 + srcCB.B[x2, y2] * dx2y2;
                                    destCB.B[x, y] = (byte)nbyte;
                                    nbyte = srcCB.G[x1, y1] * dx1y1 + srcCB.G[x2, y1] * dx2y1 + srcCB.G[x1, y2] * dx1y2 + srcCB.G[x2, y2] * dx2y2;
                                    destCB.G[x, y] = (byte)nbyte;
                                    nbyte = srcCB.R[x1, y1] * dx1y1 + srcCB.R[x2, y1] * dx2y1 + srcCB.R[x1, y2] * dx1y2 + srcCB.R[x2, y2] * dx2y2;
                                    destCB.R[x, y] = (byte)nbyte;
                                }
                                else
                                {
                                    destCB.A[x, y] = srcCB.A[x1, y1];
                                    destCB.B[x, y] = srcCB.B[x1, y1];
                                    destCB.G[x, y] = srcCB.G[x1, y1];
                                    destCB.R[x, y] = srcCB.R[x1, y1];
                                }
                            }
                        }
                    });
                });
                return destCB.ToBitmap();
            }
        }*/
}