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

        protected override Size MeasureOverride(Size constraint)
        {
            double spacing = Spacing;
            var children = Children;
            int fillIndex = children.IndexOf(children.OfType<ISelectable>().FirstOrDefault(x => x.IsSelected) as IControl);

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
                
                parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                accumulatedHeight += (childDesiredSize.Height + spacing);
            }

            // Make sure the final accumulated size is reflected in parentSize.
            parentWidth = Math.Max(parentWidth, accumulatedWidth);
            parentHeight = Math.Max(parentHeight, accumulatedHeight);

            return (new Size(parentWidth, parentHeight));
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double spacing = Spacing;
            var selected = Children.OfType<ISelectable>().FirstOrDefault(x => x.IsSelected) as IControl;
            int fillIndex = Children.IndexOf(selected);

            var children = Children;
            int totalChildrenCount = children.Count();

            double accumulatedTop = 0;
            double accumulatedBottom = 0;

            if (selected != children.First())
            {
                for (int i = 0; i < fillIndex; i++)
                {
                    IControl child = children.ElementAt(i);
                    
                    if (child == null)
                        continue;

                    Size childDesiredSize = child.DesiredSize;
                    Rect rcChild = new Rect(
                        0,
                        accumulatedTop,
                        Math.Max(0.0, arrangeSize.Width),
                        Math.Max(0.0, arrangeSize.Height - (accumulatedTop + accumulatedBottom)));

                    accumulatedTop += (childDesiredSize.Height + spacing);
                    rcChild = rcChild.WithHeight(childDesiredSize.Height);
                    
                    child.Arrange(rcChild);
                }
            }


            if (selected != children.Last())
            {
                var reverseChildren = children.Reverse();
                for (int i = fillIndex + 1; i < totalChildrenCount; i++)
                {
                    IControl child = reverseChildren.ElementAt(i - (fillIndex + 1));
                    
                    if (child == null)
                        continue;

                    Size childDesiredSize = child.DesiredSize;
                    Rect rcChild = new Rect(
                        0,
                        accumulatedTop,
                        Math.Max(0.0, arrangeSize.Width),
                        Math.Max(0.0, arrangeSize.Height - (accumulatedTop + accumulatedBottom)));

                    accumulatedBottom += (spacing + childDesiredSize.Height);
                    rcChild = rcChild.WithY(Math.Max(0.0, (arrangeSize.Height - accumulatedBottom) + spacing));
                    rcChild = rcChild.WithHeight(childDesiredSize.Height);
                    
                    child.Arrange(rcChild);
                }
            }

            Rect rcFillChild = new Rect(
                0,
                accumulatedTop,
                Math.Max(0.0, arrangeSize.Width),
                Math.Max(0.0, arrangeSize.Height - (accumulatedTop + accumulatedBottom)));
            
            selected.Arrange(rcFillChild);

            return (arrangeSize);
        }
    }
}