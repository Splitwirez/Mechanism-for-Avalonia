using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Mechanism.AvaloniaUI.Core
{
    public static class NativeMethodsWindows
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool enabled);

        public static bool DwmIsCompositionEnabled()
        {
            DwmIsCompositionEnabled(out bool enabled);
            return enabled;
        }

        [DllImport("dwmapi.dll")]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            public DWM_BB dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;

            public DWM_BLURBEHIND(bool enabled)
            {
                fEnable = enabled ? true : false;
                hRgnBlur = IntPtr.Zero;
                fTransitionOnMaximized = true;
                dwFlags = DWM_BB.Enable;
            }

            public System.Drawing.Region Region
            {
                get { return System.Drawing.Region.FromHrgn(hRgnBlur); }
            }

            public bool TransitionOnMaximized
            {
                get { return fTransitionOnMaximized; }
                set
                {
                    fTransitionOnMaximized = value ? true : false;
                    dwFlags |= DWM_BB.TransitionMaximized;
                }
            }

            public void SetRegion(System.Drawing.Graphics graphics, System.Drawing.Region region)
            {
                hRgnBlur = region.GetHrgn(graphics);
                dwFlags |= DWM_BB.BlurRegion;
            }
        }

        [Flags]
        public enum DWM_BB
        {
            Enable = 1,
            BlurRegion = 2,
            TransitionMaximized = 4
        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        public static int GetWindowLongInt(IntPtr hWnd, Int32 nIndex)
        {
            IntPtr prevStylePtr = GetWindowLong(hWnd, nIndex);
            int prevStyle = 0;
            if (Environment.Is64BitOperatingSystem)
                prevStyle = (int)prevStylePtr.ToInt64();
            else
                prevStyle = prevStylePtr.ToInt32();
            return prevStyle;
        }
        
        public static IntPtr GetWindowLong(IntPtr hWnd, Int32 nIndex)/* => IntPtr.Size == 8
        ? GetWindowLongPtr64(hWnd, nIndex)
        : GetWindowLongPtr32(hWnd, nIndex);*/
        {
            if (IntPtr.Size == 4)
                return GetWindowLong32(hWnd, nIndex);
            else
                return GetWindowLongPtr64(hWnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, Int32 nIndex, Int32 dwNewLong)/* => IntPtr.Size == 8
        ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
        : SetWindowLong32(hWnd, nIndex, dwNewLong);*/
        {
            if (IntPtr.Size == 4)
                return SetWindowLong32(hWnd, nIndex, dwNewLong);
            else
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        static extern IntPtr SetWindowLong32(IntPtr hWnd, Int32 nIndex, Int32 dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, Int32 nIndex, Int32 dwNewLong);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

        [DllImport("gdi32.dll")]
        public static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, CombineRgnStyles fnCombineMode);

        public enum CombineRgnStyles : int
        {
            RGN_AND = 1,
            RGN_OR = 2,
            RGN_XOR = 3,
            RGN_DIFF = 4,
            RGN_COPY = 5,
            RGN_MIN = RGN_AND,
            RGN_MAX = RGN_COPY
        }


        [DllImport("user32.dll")]
        public static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        public enum RegionFlags
        {
            ERROR = 0,
            NULLREGION = 1,
            SIMPLEREGION = 2,
            COMPLEXREGION = 3,
        }

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);


        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int nxDest, int nyDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pptSrc, uint crKey, [In] ref BLENDFUNCTION pblend, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);


        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);


        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;

            public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
            {
                BlendOp = op;
                BlendFlags = flags;
                SourceConstantAlpha = alpha;
                AlphaFormat = format;
            }
        }


        public const Int32
            GwlStyle = -16,
            GwlExstyle = -20;

        public const Int32
            WsCaption = 0x00C00000,
            WsBorder = 0x00800000,
            WsSizeBox = 0x00040000;

        public const Int32
            WsExToolwindow = 0x00000080,
            WsExTransparent = 0x00000020,
            WsExLayered = 0x00080000,
            WsExNoActivate = 0x08000000;

        public const Int32
            LwaAlpha = 0x2,
            LwaColorKey = 0x1;

        public const Int32
            SrcCopy = 0x00CC0020,
            CaptureBlt = 0x40000000;
    }
}
