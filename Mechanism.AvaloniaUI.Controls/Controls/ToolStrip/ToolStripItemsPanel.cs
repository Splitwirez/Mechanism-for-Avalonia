using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls
{
    public class ToolStripItemsPanel : StackPanel
    {
        /*public static readonly StyledProperty<double> SpacingProperty =
            StackLayout.SpacingProperty.AddOwner<ToolStripItemsPanel>();

        public double Spacing
        {
            get => GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }*/

        static ToolStripItemsPanel()
        {
            AffectsMeasure<ToolStripItemsPanel>(SpacingProperty);
            OrientationProperty.OverrideDefaultValue<ToolStripItemsPanel>(Orientation.Horizontal);

            BoundsProperty.Changed.AddClassHandler<ToolStripItemsPanel>(new Action<ToolStripItemsPanel, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                if ((e.NewValue != null) && (e.NewValue is Rect rect))
                    sender.ArrangeOverride(rect.Size);
            }));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size retVal = new Size(0, 0);
            if (!double.IsInfinity(finalSize.Width))
                retVal = retVal.WithWidth(finalSize.Width);
            if (!double.IsInfinity(finalSize.Height))
                retVal = retVal.WithHeight(finalSize.Height);
            //Size retVal = new Size(0, 0); //base.ArrangeOverride(finalSize);

            var flexibleSpaces = Children.Where(x => x.DataContext is ToolStripFlexibleSpaceReference); //IsFlexibleSpace(ctrl));
            var inflexibleChildren = Children.Where(x => !flexibleSpaces.Contains(x));

            double inflexibleTotalWidth = 0;
            foreach (Control ctrl in inflexibleChildren)
                inflexibleTotalWidth += ctrl.DesiredSize.Width + Spacing;
            foreach (Control ctrl in flexibleSpaces)
                inflexibleTotalWidth -= Spacing;

            if (flexibleSpaces.Count() > 0)
            {
                double flexibleWidth = Math.Max(0, (Bounds.Width - inflexibleTotalWidth) / flexibleSpaces.Count());

                double prevTotalLeft = 0;
                foreach (Control ctrl in Children)
                {
                    if ((ctrl == null) || !ctrl.IsVisible)
                        continue;

                    double width;
                    double spacing;
                    if (flexibleSpaces.Contains(ctrl))
                    {
                        width = flexibleWidth;
                        if (ctrl == flexibleSpaces.Last())
                            spacing = 0;
                        else
                            spacing = -Spacing;
                    }
                    else
                    {
                        width = ctrl.DesiredSize.Width;
                        if (ctrl == inflexibleChildren.Last())
                            spacing = 0;
                        else
                            spacing = Spacing;
                    }

                    ctrl.Arrange(new Rect(prevTotalLeft, 0, width, Bounds.Height));
                    prevTotalLeft += width + spacing;
                }

                return retVal;
            }
            else
                return base.ArrangeOverride(finalSize);
        }

        /*static string flxSpace = "FLEXIBLE_SPACE";
        static bool IsFlexibleSpace(Control control)
        {
            bool retVal = false;

            if (control.DataContext != null)
                Debug.WriteLine("DataContext type: " + control.DataContext.GetType().FullName);

            if (control is ContentPresenter pres)
            {
                if ((pres.Content != null) && (pres.Content is Control ctrl))
                {
                    Debug.WriteLine("Content type: " + ctrl.GetType().FullName);
                    if (ctrl.Tag != null)
                        retVal = ctrl.Tag.ToString().Equals(flxSpace, StringComparison.OrdinalIgnoreCase);
                }
            }
            Debug.WriteLine("IsFlexibleSpace[" + control.GetType().FullName + "]: " + retVal);
            return retVal;
        }*/
    }
}
