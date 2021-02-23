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
                //sender._barMovement = sender.Bounds.Width;
            });
            AffectsRender<BlenderBar>(BarModeProperty, ResizeWidthProperty);
            AffectsMeasure<BlenderBar>(BarModeProperty, ResizeWidthProperty);
            AffectsArrange<BlenderBar>(BarModeProperty, ResizeWidthProperty);
            ResizeWidthProperty.Changed.AddClassHandler<BlenderBar>((sneder, e) =>
            {
                if (/*(sneder._barDragging) && */(e.NewValue is double newVal))
                {
                    sneder.RefreshBarMode(newVal);
                }
            });

            BarModeProperty.Changed.AddClassHandler<BlenderBar>((sneder, e) =>
            {
                if ((e.NewValue is BlenderBarMode newMode) && (e.OldValue is BlenderBarMode oldMode))
                {
                    /*if (newMode == BlenderBarMode.IconsSingleColumn)
                        sneder.ResizeWidth = sneder.IconsOnlyItemWidth - 14;
                    else if (newMode == BlenderBarMode.IconsDoubleColumn)
                        sneder.ResizeWidth = sneder.IconsOnlyItemWidth + 14;*/
                    /*if (sneder._barDragging)
                    {
                        if ((oldMode == BlenderBarMode.IconsSingleColumn) && (newMode == BlenderBarMode.IconsDoubleColumn))
                        {
                            sneder.ResizeWidth = sneder.IconsOnlyItemWidth + 14;
                        }
                        else if ((oldMode == BlenderBarMode.Labels) && (newMode == BlenderBarMode.IconsDoubleColumn))
                        {
                            sneder.ResizeWidth = (sneder.IconsOnlyItemWidth * 2) - 14;
                        }
                        else if (newMode == BlenderBarMode.IconsSingleColumn)
                            sneder.ResizeWidth = sneder.IconsOnlyItemWidth - 14;
                    }*/
                }
            });
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

        public static readonly StyledProperty<double> ResizeWidthProperty =
        AvaloniaProperty.Register<BlenderBar, double>(nameof(ResizeWidth), 128, coerce: (sneder, inVal) =>
        {
            if (sneder is BlenderBar bar)
            {
                if (inVal <= bar.IconsOnlyItemWidth)
                    return bar.IconsOnlyItemWidth;
                else if (inVal <= (bar.IconsOnlyItemWidth * 2))
                    return bar.IconsOnlyItemWidth * 2;
            }
            return inVal;
        });

        public double ResizeWidth
        {
            get => GetValue(ResizeWidthProperty);
            set => SetValue(ResizeWidthProperty, value);
        }

        Thumb _barModeThumb = null;
        
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _barModeThumb = e.NameScope.Find<Thumb>("PART_BarModeThumb");
            if (_barModeThumb != null)
            {

                _barModeThumb.DragDelta += (sneder, args) =>
                {
                    double cap = IconsOnlyItemWidth * 3;
                    
                    ResizeWidth = Math.Max(IconsOnlyItemWidth, ResizeWidth + args.Vector.X);
                };
            }
        }

        protected void RefreshBarMode(double width)
        {
            var singleColumnWidth = IconsOnlyItemWidth;
            var doubleColumnWidth = (IconsOnlyItemWidth * 2);
            var labelledWidth = (IconsOnlyItemWidth * 3);
            
            //Debug.WriteLine("RefreshBarMode from mode " + BarMode + " by width: " + width); //+ ", " + adjustedWidth);


            if (width <= singleColumnWidth)
            {
                BarMode = BlenderBarMode.IconsSingleColumn;
            }
            else if (width <= doubleColumnWidth)
            {
                BarMode = BlenderBarMode.IconsDoubleColumn;
            }
            else
            {
                BarMode = BlenderBarMode.Labels;
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



        bool IsDoubleNearOther(double test, double nearTo, double distance)
        {
            return Math.Abs(test - nearTo) < Math.Abs(distance);
            //return ((nearTo - 8) < test) && ((nearTo + 28) > test);

            //test = 40
            //nearTo = 51
                //(23 < 40) && (59 > 40)
            

            //test = 51
            //nearTo = 40
                //(12 < 51) && (48 > 51)


            /*return diff < 8;
            double diff = test - nearTo;
            if (test > nearTo)
                diff = nearTo - test;

            //test = 40
            //nearTo = 51
            return (-28 < (test - nearTo)) && ((nearTo - test) < 8); // (-28 < -11) && (11 < 8);*/
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