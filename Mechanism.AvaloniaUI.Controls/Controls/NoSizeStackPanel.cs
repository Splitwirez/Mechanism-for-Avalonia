using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls
{
    public class NoSizeStackPanel : StackPanel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            /*if (double.IsInfinity(availableSize.Width))
                return new Size(0, base.MeasureOverride(availableSize).Height);
            else
            {
                Size retSize = base.MeasureOverride(availableSize);
                return new Size(Math.Min(availableSize.Width, retSize.Width), retSize.Height);
            }*/
            if (double.IsInfinity(availableSize.Width))
                return new Size(0, base.MeasureOverride(availableSize).Height);
            else
            {
                Size retSize = base.MeasureOverride(availableSize);
                return new Size(Math.Min(availableSize.Width, retSize.Width), retSize.Height);
            }
            /*Size retSize = base.MeasureOverride(availableSize);
            if (availableSize.Width < retSize.Width)
                return new Size(0, retSize.Height);
            else
                return retSize;*/
        }

        protected override Size MeasureCore(Size availableSize)
        {
            /*if (double.IsInfinity(availableSize.Width))
                return new Size(0, base.MeasureCore(availableSize).Height);
            else
            {
                Size retSize = base.MeasureCore(availableSize);
                return new Size(Math.Min(availableSize.Width, retSize.Width), retSize.Height);
            }*/
            if (double.IsInfinity(availableSize.Width))
                return new Size(0, base.MeasureCore(availableSize).Height);
            else
            {
                Size retSize = base.MeasureCore(availableSize);
                return new Size(Math.Min(availableSize.Width, retSize.Width), retSize.Height);
            }
            /*Size retSize = base.MeasureCore(availableSize);
            if (availableSize.Width < retSize.Width)
                return new Size(0, retSize.Height);
            else
                return retSize;*/
        }
    }
}
