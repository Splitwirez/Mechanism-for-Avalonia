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

namespace Mechanism.AvaloniaUI.Extras
{
    public class ExpandToFillPanel : Panel
    {
        /// <summary>
        /// Defines the <see cref="Spacing"/> property.
        /// </summary>
        public static readonly StyledProperty<double> SpacingProperty =
            StackLayout.SpacingProperty.AddOwner<ExpandToFillPanel>();
        
        /// <summary>
        /// Gets or sets the size of the spacing to place between child controls.
        /// </summary>
        public double Spacing
        {
            get => GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }


        // <summary>
        /// Defines the <see cref="Orientation"/> property.
        /// </summary>
        public static readonly StyledProperty<Orientation> OrientationProperty =
            StackLayout.OrientationProperty.AddOwner<ExpandToFillPanel>();
        
        /// <summary>
        /// Gets or sets the orientation in which child controls will be layed out.
        /// </summary>
        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            bool fHorizontal = Orientation == Orientation.Horizontal;
            double spacing = Spacing;
            var children = Children;

            double parentWidth = 0;   // Our current required width due to children thus far.
            double parentHeight = 0;   // Our current required height due to children thus far.
            double accumulatedWidth = 0;   // Total width consumed by children.
            double accumulatedHeight = 0;   // Total height consumed by children.

            for (int i = 0, count = children.Count; i < count; ++i)
            {
                var child = children[i];
                Size childConstraint;             // Contains the suggested input constraint for this child.
                Size childDesiredSize;            // Contains the return size from child measure.

                if (child == null)
                { continue; }

                // Child constraint is the remaining size; this is total size minus size consumed by previous children.
                childConstraint = new Size(Math.Max(0.0, constraint.Width - accumulatedWidth),
                                           Math.Max(0.0, constraint.Height - accumulatedHeight));

                // Measure child.
                child.Measure(childConstraint);
                childDesiredSize = child.DesiredSize;
                
                if (fHorizontal)
                {
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                    accumulatedWidth += childDesiredSize.Width;
                }
                else
                {
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    accumulatedHeight += (childDesiredSize.Height + spacing);
                }
            }

            // Make sure the final accumulated size is reflected in parentSize.
            parentWidth = Math.Max(parentWidth, accumulatedWidth);
            parentHeight = Math.Max(parentHeight, accumulatedHeight);

            return (new Size(parentWidth, parentHeight));
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            bool fHorizontal = Orientation == Orientation.Horizontal;
            double spacing = Spacing;
            var children = Children.OfType<ExpandToFillViewItem>().ToList();
            var selected = children.FirstOrDefault(x => x.IsSelected);
            if (selected != null)
            {
                int fillIndex = children.IndexOf(selected);
                int totalChildrenCount = children.Count();

                double accumulatedBefore = 0;
                double accumulatedAfter = 0;

                if (selected != children.First())
                {
                    for (int i = 0; i < fillIndex; i++)
                    {
                        IControl child = children.ElementAt(i);
                        
                        if (child == null)
                            continue;

                        Size childDesiredSize = child.DesiredSize;
                        
                        Rect rcChild = GetChildRect(accumulatedBefore, accumulatedAfter, arrangeSize.Width, arrangeSize.Height, fHorizontal);

                        if (fHorizontal)
                        {
                            accumulatedBefore += (childDesiredSize.Width + spacing);
                            rcChild = rcChild.WithWidth(childDesiredSize.Width);
                        }
                        else
                        {
                            accumulatedBefore += (childDesiredSize.Height + spacing);
                            rcChild = rcChild.WithHeight(childDesiredSize.Height);
                        }
                        
                        child.Arrange(rcChild);
                    }
                }


                if (selected != children.Last())
                {
                    var reverseChildren = System.Linq.Enumerable.Reverse(children);
                    for (int i = fillIndex + 1; i < totalChildrenCount; i++)
                    {
                        IControl child = reverseChildren.ElementAt(i - (fillIndex + 1));
                        
                        if (child == null)
                            continue;

                        Size childDesiredSize = child.DesiredSize;
                        /*double rcBefore = (accumulatedBefore + accumulatedAfter);
                        
                        double rcWidth = arrangeSize.Width;
                        double rcHeight = arrangeSize.Height;
                        if (fHorizontal)
                            rcWidth -= rcBefore;
                        else
                            rcHeight -= rcBefore;
                        
                        Rect rcChild = new Rect(
                            0,
                            accumulatedBefore,
                            Math.Max(0.0, rcWidth),
                            Math.Max(0.0, rcHeight));*/
                        Rect rcChild = GetChildRect(accumulatedBefore, accumulatedAfter, arrangeSize.Width, arrangeSize.Height, fHorizontal);

                        /*accumulatedAfter += (spacing + childDesiredSize.Height);
                        rcChild = rcChild.WithY(Math.Max(0.0, (arrangeSize.Height - accumulatedAfter) + spacing));
                        rcChild = rcChild.WithHeight(childDesiredSize.Height);*/

                        if (fHorizontal)
                        {
                            accumulatedAfter += (childDesiredSize.Width + spacing);
                            //rcChild = rcChild.WithWidth(childDesiredSize.Width);
                            rcChild = rcChild.WithX(Math.Max(0.0, (arrangeSize.Width - accumulatedAfter) + spacing)).WithWidth(childDesiredSize.Width);
                        }
                        else
                        {
                            accumulatedAfter += (childDesiredSize.Height + spacing);
                            //rcChild = rcChild.WithHeight(childDesiredSize.Height);
                            rcChild = rcChild.WithY(Math.Max(0.0, (arrangeSize.Height - accumulatedAfter) + spacing)).WithHeight(childDesiredSize.Height);
                        }
                        
                        child.Arrange(rcChild);
                    }
                }

                Rect rcFillChild = GetChildRect(accumulatedBefore, accumulatedAfter, arrangeSize.Width, arrangeSize.Height, fHorizontal);
                selected.Arrange(rcFillChild);
            }
            else
            {
                Rect rcChild = new Rect(arrangeSize);
                double previousChildSize = 0.0;

                for (int i = 0, count = children.Count; i < count; ++i)
                {
                    var child = children[i];

                    if (child == null || !child.IsVisible)
                    { continue; }

                    if (fHorizontal)
                    {
                        rcChild = rcChild.WithX(rcChild.X + previousChildSize);
                        previousChildSize = child.DesiredSize.Width;
                        rcChild = rcChild.WithWidth(previousChildSize);
                        rcChild = rcChild.WithHeight(Math.Max(arrangeSize.Height, child.DesiredSize.Height));
                        previousChildSize += spacing;
                    }
                    else
                    {
                        rcChild = rcChild.WithY(rcChild.Y + previousChildSize);
                        previousChildSize = child.DesiredSize.Height;
                        rcChild = rcChild.WithHeight(previousChildSize);
                        rcChild = rcChild.WithWidth(Math.Max(arrangeSize.Width, child.DesiredSize.Width));
                        previousChildSize += spacing;
                    }

                    child.Arrange(rcChild);
                }
            }


            return (arrangeSize);
        }

        Rect GetChildRect(double before, double after, double desWidth, double desHeight, bool horizontal)
        {
            double off = before + after;

            double rcsX = 0;
            double rcsY = 0;
            double rcsWidth = desWidth;
            double rcsHeight = desHeight;
            
            if (horizontal)
            {
                rcsX = before;
                rcsWidth -= off;
            }
            else
            {
                rcsY = before;
                rcsHeight -= off;
            }
            
            return new Rect(
                rcsX,
                rcsY,
                Math.Max(0.0, rcsWidth),
                Math.Max(0.0, rcsHeight));   
        }
    }
}