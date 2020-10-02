using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Avalonia.Layout;
using Avalonia.Controls.Templates;
using System.Collections;
using Avalonia.Collections;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using System.Diagnostics;
using Avalonia.Data;
using Mechanism.AvaloniaUI.Core;
using Avalonia.Threading;
using Avalonia.Utilities;
using Avalonia.Styling;
using System.Globalization;

namespace Mechanism.AvaloniaUI.Controls.BlenderBar
{
    public class BlenderBarPanel : Panel
    {
        static BlenderBarPanel()
        {
            AffectsRender<BlenderBar>(BarModeProperty);
            AffectsMeasure<BlenderBar>(BarModeProperty);
            AffectsArrange<BlenderBar>(BarModeProperty);

            BarModeProperty.Changed.AddClassHandler<BlenderBarPanel>((sender, args) =>
            {
                sender.InvalidateStyles();
                sender.InvalidateArrange();
                sender.InvalidateMeasure();
                sender.InvalidateVisual();
            });
        }

        public static readonly StyledProperty<double> IconsOnlyItemWidthProperty =
        BlenderBar.IconsOnlyItemWidthProperty.AddOwner<BlenderBarPanel>();

        public static readonly StyledProperty<double> ItemHeightProperty =
        BlenderBar.ItemHeightProperty.AddOwner<BlenderBarPanel>();


        public double IconsOnlyItemWidth
        {
            get => GetValue(IconsOnlyItemWidthProperty);
            set => SetValue(IconsOnlyItemWidthProperty, value); 
        }

        public double ItemHeight
        {
            get => GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public static readonly StyledProperty<BlenderBarMode> BarModeProperty =
        BlenderBar.BarModeProperty.AddOwner<BlenderBarPanel>();

        public BlenderBarMode BarMode
        {
            get => GetValue(BarModeProperty);
            set => SetValue(BarModeProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (BarMode == BlenderBarMode.IconsDoubleColumn)
                return new Size(IconsOnlyItemWidth * 2, ItemHeight * (Children.Count / 2));
            else
                return new Size(base.MeasureOverride(constraint).Width, ItemHeight * Children.Count);
            /*else if ((BarMode == BlenderBarMode.Labels) && (!double.IsNaN(Width))) //if (BarMode == BlenderBarMode.IconsSingleColumn)
                return new Size(Width, ItemHeight * Children.Count);
            else
                return new Size(outWidth, ItemHeight * Children.Count);*/
            

            //return new Size(outWidth, ItemHeight * Children.Count);
        }


        //static BorderPresence _leftBorder = new BorderPresence(true, false, false, false);
        //static BorderPresence _rightBorder = new BorderPresence(false, false, true, false);
        //static BorderPresence _rightBorder = new BorderPresence(false, false, true, false);
        protected override Size ArrangeOverride(Size finalSize)
        {
            var children = Children.OfType<BlenderBarItem>().OrderBy(x => x.GroupIndex).ToList();

            double yTotal = 0;
            double itemHeight = ItemHeight;
            int _prevGroupIndex = 0;

            if (BarMode == BlenderBarMode.IconsDoubleColumn)
            {
                bool isRightColumn = false;
                for (int i = 0, count = children.Count; i < count; i++)
                {
                    var child = children[i];

                    if (_prevGroupIndex != child.GroupIndex)
                    {
                        isRightColumn = false;
                        yTotal += (itemHeight + 5);
                    }


                    var borderPresence = new BorderPresence();
                    borderPresence.Left = !isRightColumn;
                    
                    if (i >= 2)
                        borderPresence.Top = false;
                    else
                        borderPresence.Top = true;

                    //if (i == (count - 1) && )

                    BorderPresence.SetBorderPresence(child, borderPresence);

                    
                    if (isRightColumn)
                    {
                        child.Arrange(new Rect(IconsOnlyItemWidth, yTotal, IconsOnlyItemWidth, itemHeight));
                        yTotal += itemHeight;
                    }
                    else
                        child.Arrange(new Rect(0, yTotal, IconsOnlyItemWidth, itemHeight));

                    isRightColumn = !isRightColumn;

                    
                    _prevGroupIndex = child.GroupIndex;
                }

                /*if (children.Count >= 4)
                {
                    children[0].SetValue(BorderPresence.BorderPresenceProperty, BorderPresence.TopLeftOnly);
                    children[1].SetValue(BorderPresence.BorderPresenceProperty, BorderPresence.TopRightOnly);
                    if (isRightColumn)
                        children[children.Count - 1].SetValue(BorderPresence.BorderPresenceProperty, BorderPresence.TopRightOnly);
                }*/
            }
            else
            {
                double childWidth = IconsOnlyItemWidth;
                if (BarMode == BlenderBarMode.Labels)
                    childWidth = finalSize.Width;

                for (int i = 0, count = children.Count; i < count; i++)
                {
                    var child = children[i];

                    var borderPresence = new BorderPresence();
                    var cornerCurves = new CornerCurves();

                    if (_prevGroupIndex != child.GroupIndex)
                    {
                        yTotal += 5;
                        borderPresence.Top = true;
                    }
                    else if (i == 0)
                        borderPresence.Top = true;
                    else
                        borderPresence.Top = false;

                    BorderPresence.SetBorderPresence(child, borderPresence);

                    
                    if (children.Where(x => x.GroupIndex == child.GroupIndex).Count() == 1)
                        CornerCurves.SetCornerCurves(child, new CornerCurves(true));
                    else if (children.Last(x => x.GroupIndex == child.GroupIndex) == child)
                        CornerCurves.SetCornerCurves(child, new CornerCurves(false, false, true, true));
                    else if (children.First(x => x.GroupIndex == child.GroupIndex) == child)
                        CornerCurves.SetCornerCurves(child, new CornerCurves(true, true, false, false));
                    else
                        CornerCurves.SetCornerCurves(child, new CornerCurves(false));
                    

                    child.Arrange(new Rect(0, yTotal, childWidth, itemHeight));
                    
                    yTotal += itemHeight;
                    
                    _prevGroupIndex = child.GroupIndex;
                }
            }

            return finalSize;
        }
    }
}