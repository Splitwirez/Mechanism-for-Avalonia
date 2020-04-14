using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ToolBar
{
    public class ToolBarTrayPanel : Panel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            return LayoutToolBars(finalSize);
        }

        public Size LayoutToolBars(Size finalSize)
        {
            //double ctrlWidth = 0;
            //double ctrlHeight = 0;
            var retVal = base.ArrangeOverride(finalSize);
            ////Debug.WriteLine("ArrangeOverride");
            IEnumerable<ToolBar> unsortedItems = LogicalChildren.OfType<ToolBar>(); //.Where(x => x is ToolBar).Cast<ToolBar>();
            int bandCount = unsortedItems.Max(x => x.Band) + 1;
            double top = 0;
            //int prevBandsCount = 0;
            for (int i = 0; i <= bandCount; i++)
            {
                List<ToolBar> bandItems = unsortedItems.Where(x => x.Band == i).OrderBy(x => x.BandIndex).ToList();
                double bandThickness = 0;
                if (unsortedItems.Where(x => x.Band == i).Count() > 0)
                {
                    bandThickness = bandItems/*.Where(x => x.Band == i)*/.Max(x => x.DesiredSize.Height);
                    double left = 0;
                    int index = 0;
                    foreach (ToolBar item in bandItems)
                    {
                        item.BandIndex = bandItems.IndexOf(item)/* - prevBandsCount*/;
                        double width = Math.Max(item.MinWidth, Math.Min(item.Width, item.MaxWidth)); //item.Width;
                        /*if ((width <= 0) && (item.DesiredSize.Width > 0))
                            width = item.DesiredSize.Width;*/
                        /*if (width <= 0)
                            width = 100;*/
                        /*if (left + width > Bounds.Width)
                            width -= (left + width) - Bounds.Width;*/
                        if (width < 0)
                            width = 0;
                        if (bandItems.Last() == item)
                        {
                            double targetWidth = Math.Max(Bounds.Width - left, 0);
                            item.Width = targetWidth;
                            item.Arrange(new Rect(left, top, targetWidth, bandThickness));
                            ////Debug.WriteLine("Last item");
                        }
                        else
                            item.Arrange(new Rect(left, top, width, bandThickness));
                        ////Debug.WriteLine("arranged ToolBar " + index + " at X = " + left);
                        left += width;
                        index++;
                    }
                    top += bandThickness;
                }
                //prevBandsCount += bandItems.Count();
            }
            return retVal;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size retVal = base.MeasureOverride(availableSize);

            IEnumerable<ToolBar> unsortedItems = LogicalChildren.OfType<ToolBar>(); //.Where(x => x is ToolBar).Cast<ToolBar>();
            int bandCount = unsortedItems.Max(x => x.Band) + 1;
            double bandThickness = 0;
            while (unsortedItems.Where(x => x.Band == 0).Count() == 0)
            {
                foreach (ToolBar t in unsortedItems)
                    t.Band--;
            }
            for (int i = 0; i <= bandCount; i++)
            {
                IEnumerable<ToolBar> bandItems = unsortedItems.Where(x => x.Band == i)/*.OrderBy(x => x.BandIndex)*/;
                if (bandItems/*.Where(x => x.Band == i)*/.Count() > 0)
                {
                    bandThickness += bandItems.Max(x => x.DesiredSize.Height);
                }
            }

            return new Size(retVal.Width, bandThickness);
        }

        static ToolBarTrayPanel()
        {
            BoundsProperty.Changed.AddClassHandler<ToolBarTrayPanel>(new Action<ToolBarTrayPanel, AvaloniaPropertyChangedEventArgs>((sneder, args) => sneder.LayoutToolBars(sneder.Bounds.Size)));
        }

        public ToolBarTrayPanel()
        {
            ToolBar.ToolbarResized += (sneder, args) =>
            {
                if (Children.Contains(sneder))
                {
                    LayoutToolBars(Bounds.Size);
                    InvalidateMeasure();
                }
            };
        }
    }
}
