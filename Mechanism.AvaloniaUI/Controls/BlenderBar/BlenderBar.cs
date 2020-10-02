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
    public enum BlenderBarMode
    {
        IconsSingleColumn,
        IconsDoubleColumn,
        Labels
    }
    public class BlenderBar : TreeView
    {
        static BlenderBar()
        {
            BoundsProperty.Changed.AddClassHandler<BlenderBar>((sender, args) =>
            {
                sender._barRight = sender.Bounds.Right;
            });
            AffectsRender<BlenderBar>(BarModeProperty, LabelledWidthProperty, LabelledMinWidthProperty);
            AffectsMeasure<BlenderBar>(BarModeProperty, LabelledWidthProperty, LabelledMinWidthProperty);
            AffectsArrange<BlenderBar>(BarModeProperty, LabelledWidthProperty, LabelledMinWidthProperty);
        }

        public static readonly StyledProperty<BlenderBarMode> BarModeProperty =
        AvaloniaProperty.Register<BlenderBar, BlenderBarMode>(nameof(BarMode), BlenderBarMode.Labels);

        public BlenderBarMode BarMode
        {
            get => GetValue(BarModeProperty);
            set => SetValue(BarModeProperty, value);
        }


        public static readonly StyledProperty<double> IconsOnlyItemWidthProperty =
        AvaloniaProperty.Register<BlenderBar, double>(nameof(IconsOnlyItemWidth), 36);

        public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<BlenderBar, double>(nameof(ItemHeight), 36);


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

        public static readonly StyledProperty<double> LabelledWidthProperty =
        AvaloniaProperty.Register<BlenderBar, double>(nameof(LabelledWidth), 128);

        public double LabelledWidth
        {
            get => GetValue(LabelledWidthProperty);
            set => SetValue(LabelledWidthProperty, value);
        }

        public static readonly StyledProperty<double> LabelledMinWidthProperty =
        AvaloniaProperty.Register<BlenderBar, double>(nameof(LabelledMinWidth), 128);

        public double LabelledMinWidth
        {
            get => GetValue(LabelledMinWidthProperty);
            set => SetValue(LabelledMinWidthProperty, value);
        }

        Thumb _barModeThumb = null;
        double _barRight = 0;
        //double _thumbX = 0;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _barModeThumb = e.NameScope.Find<Thumb>("PART_BarModeThumb");
            if (_barModeThumb != null)
            {
                _barModeThumb.DragStarted += (sneder, args) =>
                {
                    _barRight = Bounds.Right;
                    //_thumbX = _barModeThumb.Bounds.X;
                };

                _barModeThumb.DragDelta += (sneder, args) =>
                {
                    var singleColumnWidth = IconsOnlyItemWidth * 1.5;
                    var doubleColumnWidth = IconsOnlyItemWidth * 2.5;
                    double x = _barRight + args.Vector.X;
                    if (x <= singleColumnWidth)
                        SetBarMode(BlenderBarMode.IconsSingleColumn);
                    else if ((x > singleColumnWidth) && (x <= doubleColumnWidth))
                        SetBarMode(BlenderBarMode.IconsDoubleColumn);
                    else if (x > doubleColumnWidth)
                    {
                        SetBarMode(BlenderBarMode.Labels);
                        LabelledWidth = Math.Max(LabelledMinWidth, LabelledWidth + args.Vector.X);
                    }
                    //Console.WriteLine("x, BarMode, LabelledWidth: " + x.ToString() + ", " + BarMode.ToString() + ", " + LabelledWidth.ToString());
                };
            }
        }

        protected void SetBarMode(BlenderBarMode mode)
        {
            if (mode != BarMode)
            {
                BarMode = mode;
                this.InvalidateMeasure();
                this.InvalidateArrange();
                this.InvalidateVisual();
            }
        }

        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            var result = new TreeItemContainerGenerator<BlenderBarItem>(
                this,
                BlenderBarItem.HeaderProperty,
                BlenderBarItem.ItemTemplateProperty,
                BlenderBarItem.ItemsProperty,
                BlenderBarItem.IsExpandedProperty);
            result.Index.Materialized += ContainerMaterialized;
            return result;
        }

        private void ContainerMaterialized(object sender, ItemContainerEventArgs e)
        {
            var selectedItem = SelectedItem;

            if (selectedItem == null)
            {
                return;
            }

            foreach (var container in e.Containers)
            {
                if (container.Item == selectedItem)
                {
                    ((BlenderBarItem)container.ContainerControl).IsSelected = true;

                    if (AutoScrollToSelectedItem)
                        Dispatcher.UIThread.Post(container.ContainerControl.BringIntoView);

                    break;
                }
            }
        }
    }
    
    /*public class BlenderBarItemContainerGenerator : ItemContainerGenerator<BlenderBarItem>
    {
        public BlenderBarItemContainerGenerator(
            IControl owner, 
            AvaloniaProperty contentProperty,
            AvaloniaProperty contentTemplateProperty)
            : base(owner, contentProperty, contentTemplateProperty)
        { }

        /// <inheritdoc/>
        protected override IControl CreateContainer(object item)
        {
            var container = item as BlenderBarItem;

            if (container != null)
            {
                return container;
            }
            else
            {
                var result = new BlenderBarItem();

                if (ContentTemplateProperty != null)
                {
                    result.SetValue(ContentTemplateProperty, ItemTemplate, BindingPriority.Style);
                }

                if (item is Control ctrl)
                    result.SetValue(AttachedIcon.IconProperty, ctrl.GetValue(AttachedIcon.IconProperty));

                result.SetValue(ContentProperty, item, BindingPriority.Style);

                if (!(item is IControl))
                {
                    result.DataContext = item;
                }

                return result;
            }
        }
    }*/
}