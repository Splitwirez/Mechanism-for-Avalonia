using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mechanism.AvaloniaUI.Controls
{
    public enum ChildrenHorizontalAlignment
    {
        Left,
        Right
    }

    public class AlignableStackPanel : StackPanel
    {
        public static readonly StyledProperty<ChildrenHorizontalAlignment> HorizontalChildrenAlignmentProperty =
            AvaloniaProperty.Register<AlignableStackPanel, ChildrenHorizontalAlignment>(nameof(HorizontalChildrenAlignment), defaultValue: ChildrenHorizontalAlignment.Left);

        public ChildrenHorizontalAlignment HorizontalChildrenAlignment
        {
            get => GetValue(HorizontalChildrenAlignmentProperty);
            set => SetValue(HorizontalChildrenAlignmentProperty, value);
        }

        static AlignableStackPanel()
        {
            AffectsMeasure<AlignableStackPanel>(HorizontalChildrenAlignmentProperty);
            AffectsArrange<AlignableStackPanel>(HorizontalChildrenAlignmentProperty);
            AffectsRender<AlignableStackPanel>(HorizontalChildrenAlignmentProperty);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            bool fHorizontal = (Orientation == Orientation.Horizontal);
            bool fRight = (HorizontalChildrenAlignment == ChildrenHorizontalAlignment.Right);
            if (fHorizontal)
            {
                if (fRight)
                {
                    var children = Children.Reverse();
                    Rect rcChild = new Rect(finalSize);
                    double previousChildSize = 0.0;
                    var spacing = Spacing;

                    for (int i = 0, count = children.Count(); i < count; ++i)
                    {
                        var child = children.ElementAt(i);

                        if (child == null || !child.IsVisible)
                        { continue; }


                        previousChildSize += child.DesiredSize.Width;
                        rcChild = rcChild.WithX(finalSize.Width - previousChildSize);
                        rcChild = rcChild.WithWidth(child.DesiredSize.Width);
                        rcChild = rcChild.WithHeight(Math.Max(finalSize.Height, child.DesiredSize.Height));
                        previousChildSize += spacing;

                        ArrangeChild(child, rcChild);
                    }
                }
                else
                {
                    var children = Children;
                    Rect rcChild = new Rect(finalSize);
                    double previousChildSize = 0.0;
                    var spacing = Spacing;

                    for (int i = 0, count = children.Count; i < count; ++i)
                    {
                        var child = children[i];

                        if (child == null || !child.IsVisible)
                        { continue; }


                        rcChild = rcChild.WithX(rcChild.X + previousChildSize);
                        previousChildSize = child.DesiredSize.Width;
                        rcChild = rcChild.WithWidth(previousChildSize);
                        rcChild = rcChild.WithHeight(Math.Max(finalSize.Height, child.DesiredSize.Height));
                        previousChildSize += spacing;

                        ArrangeChild(child, rcChild);
                    }
                }
            }
            else
            {
                var children = Children;
                Rect rcChild = new Rect(finalSize);
                double previousChildSize = 0.0;
                var spacing = Spacing;

                for (int i = 0, count = children.Count; i < count; ++i)
                {
                    var child = children[i];

                    if (child == null || !child.IsVisible)
                    { continue; }
                    rcChild = rcChild.WithY(rcChild.Y + previousChildSize);
                    previousChildSize = child.DesiredSize.Height;
                    rcChild = rcChild.WithHeight(previousChildSize);
                    rcChild = rcChild.WithWidth(Math.Max(finalSize.Width, child.DesiredSize.Width));
                    previousChildSize += spacing;

                    ArrangeChild(child, rcChild);
                }
            }

            return finalSize;
        }

        void ArrangeChild(IControl child, Rect rect)
        {
            child.Arrange(rect);
        }

        /*protected override Size MeasureCore(Size availableSize)
        {
            var retVal = base.MeasureCore(availableSize);
            if (ForceZeroDesiredSize)
                return Size.Empty;
            else
                return retVal;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var retVal = base.MeasureOverride(availableSize);
            if (ForceZeroDesiredSize)
                return Size.Empty;
            else
                return retVal;
        }*/
    }
}
