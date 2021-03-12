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
            
            var retVal = base.ArrangeOverride(finalSize);
            var children = Children.OfType<ToolBar>();
            //LayoutToolBars(retVal);
            foreach (ToolBar bar in children.Where(x => double.IsNaN(x.BandLength)))
            {
                bar.Measure(Size.Infinity);
                bar.BandLength = bar.DesiredSize.Width;
                //Debug.WriteLine("LENGTH SET IN PANEL: " + bar.BandLength);
            }


            int bandCount = children.Max(x => x.Band) + 1;
            
            double topOffset = 0;
            for (int band = 0; band < bandCount; band++)
            {
                var bandBars = children.Where(x => x.Band == band);
                if (bandBars.Count() > 0)
                {
                    double bandHeight = bandBars.Max(x => x.DesiredSize.Height);

                    /*int nIndex = Math.Max(-1, bandBars.Max(x => x.BandIndex));
                    foreach (ToolBar bar in bandBars.Where(x => x.BandIndex < 0))
                    {
                        nIndex++;
                        bar.BandIndex = nIndex;
                    }*/

                    bandBars = bandBars.OrderBy(x => x.BandIndex);
                    double leftOffset = 0;
                    var lastBar = bandBars.Last();
                    
                    if (bandBars.Count() > 1)
                    {
                        bandBars = bandBars.Take(bandBars.Count() - 1);
                        foreach (ToolBar bar in bandBars)
                        {
                            double width = bar.BandLength;
                            if ((leftOffset + width) > (finalSize.Width - lastBar.MinWidth))
                            {
                                width -= lastBar.MinWidth;
                            }
                            bar.Arrange(new Rect(leftOffset, topOffset, Math.Max(0, width), bandHeight));
                            leftOffset += width;
                        }
                    }

                    lastBar.Arrange(new Rect(leftOffset, topOffset, Math.Max(0, retVal.Width - leftOffset), bandHeight));

                    topOffset += bandHeight;
                }
            }

            return retVal;
        }

        public void LayoutToolBars(Size finalSize)
        {
            //double ctrlWidth = 0;
            //double ctrlHeight = 0;
            ////Debug.WriteLine("ArrangeOverride");
            IEnumerable<ToolBar> unsortedItems = Children.OfType<ToolBar>(); //.Where(x => x is ToolBar).Cast<ToolBar>();
            
            int bandCount = unsortedItems.Max(x => x.Band) + 1;
            double top = 0;
            //int prevBandsCount = 0;
            for (int i = 0; i <= bandCount; i++)
            {
                IEnumerable<ToolBar> iBandItems = unsortedItems.Where(x => x.Band == i);
                
                if (iBandItems.Count() > 0)
                {
                    int nIndex = iBandItems.Max(x => x.BandIndex);
                    foreach (ToolBar bar in iBandItems.Where(x => x.BandIndex < 0))
                    {
                        nIndex++;
                        bar.BandIndex = nIndex;
                    }
                }

                
                List<ToolBar> bandItems = iBandItems.OrderBy(x => x.BandIndex).ToList();
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
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size retVal = base.MeasureOverride(availableSize);

            IEnumerable<ToolBar> unsortedItems = Children.OfType<ToolBar>(); //.Where(x => x is ToolBar).Cast<ToolBar>();
            double bandThickness = 0;
            if (unsortedItems.Count() > 0)
            {
                int bandCount = unsortedItems.Max(x => x.Band) + 1;
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
            }

            return new Size(retVal.Width, bandThickness);
        }

        static ToolBarTrayPanel()
        {
            //BoundsProperty.Changed.AddClassHandler<ToolBarTrayPanel>(new Action<ToolBarTrayPanel, AvaloniaPropertyChangedEventArgs>((sneder, args) => sneder.LayoutToolBars(sneder.Bounds.Size)));
        }
    }
}
