using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ToolBar
{
    public class ToolBarTray : ItemsControl
    {
        /*public static readonly AvaloniaProperty<int> BandProperty = AvaloniaProperty.RegisterAttached<ToolBarTray, ToolBarTrayItem, int>("Band");
        public static int GetBand(ToolBarTrayItem bar) => bar.GetValue(BandProperty);
        public static void SetBand(ToolBarTrayItem bar, int value) => bar.SetValue(BandProperty, value);

        public static readonly AvaloniaProperty<int> BandIndexProperty = AvaloniaProperty.RegisterAttached<ToolBarTray, ToolBarTrayItem, int>("BandIndex");
        public static int GetBandIndex(ToolBarTrayItem bar) => bar.GetValue(BandIndexProperty);
        public static void SetBandIndex(ToolBarTrayItem bar, int value) => bar.SetValue(BandIndexProperty, value);
        
        public static readonly AvaloniaProperty<double> BandSizeProperty = AvaloniaProperty.RegisterAttached<ToolBarTray, ToolBarTrayItem, double>("BandSize", defaultValue: 0);
        public static double GetBandSize(ToolBarTrayItem bar)
        {
            double val = bar.GetValue(BandSizeProperty);
            if (val < 0)
                SetBandSize(bar, bar.DesiredSize.Width);
            return bar.GetValue(BandSizeProperty);
        }
        public static void SetBandSize(ToolBarTrayItem bar, double value) => bar.SetValue(BandSizeProperty, value);

        static ToolBarTray()
        {
            AffectsArrange<ToolBar>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsMeasure<ToolBar>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsRender<ToolBar>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsArrange<ToolBarTrayItem>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsMeasure<ToolBarTrayItem>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsRender<ToolBarTrayItem>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsArrange<ToolBarTrayPanel>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsMeasure<ToolBarTrayPanel>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsRender<ToolBarTrayPanel>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsArrange<ToolBarTray>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsMeasure<ToolBarTray>(BandProperty, BandIndexProperty, BandSizeProperty);
            AffectsRender<ToolBarTray>(BandProperty, BandIndexProperty, BandSizeProperty);
        }*/

        public ToolBarTray()
        {
            ToolBar.ToolbarResized += (sneder, args) =>
            {
                if (Items.OfType<ToolBar>().Contains(sneder))
                    Measure(new Size(Bounds.Width, double.PositiveInfinity));
            };
        }

        /*protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ItemContainerGenerator<ToolBarTrayItem>(this, ToolBarTrayItem.ContentProperty, ToolBarTrayItem.ContentTemplateProperty);
        }*/

        /*protected override void LogicalChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                int lastBand = 0;
                if (LogicalChildren.Count > 0)
                    lastBand = LogicalChildren.Max(x => GetBand(x as ToolBar));
                foreach (object o in e.NewItems)
                {
                    SetBand(o as ToolBar, lastBand);
                    SetBandIndex(o as ToolBar, LogicalChildren.Where(x => GetBand(x as ToolBar) == lastBand).Max(x => GetBandIndex(x as ToolBar)) + 1);
                }
            }
            base.LogicalChildrenCollectionChanged(sender, e);
        }*/

        public static readonly StyledProperty<bool> IsLockedProperty =
        AvaloniaProperty.Register<ToolBarTray, bool>(nameof(IsLocked), defaultValue: false);

        public bool IsLocked
        {
            get { return GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        public int GetBandCount()
        {
            if (Items.OfType<ToolBar>().Count() > 0)
                return Items.OfType<ToolBar>().Max(x => /*GetBand(x.GetParentTrayItem()*/x.Band);
            else
                return 0;
        }

        protected override void ItemsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            /*IEnumerable<ToolBar> newBars = (e.NewValue as IEnumerable).OfType<ToolBar>();
            int lastBandIndex = 0;
            if (Items.OfType<ToolBar>().Count() > 0)
                lastBandIndex = Items.OfType<ToolBar>().Where(x => GetBand(x.GetParentTrayItem()) == GetBandCount()).Max(x => GetBandIndex(x.GetParentTrayItem()));
            for (int i = 0; i < newBars.Count(); i++)
            {
                SetBand(newBars.ElementAt(i).GetParentTrayItem(), GetBandCount());
                SetBandIndex(newBars.ElementAt(i).GetParentTrayItem(), i + lastBandIndex);
                Debug.WriteLine("Band info set!");
            }*/
            IEnumerable<ToolBar> newBars = (e.NewValue as IEnumerable).OfType<ToolBar>();
            int lastBandIndex = 0;
            if (Items.OfType<ToolBar>().Count() > 0)
                lastBandIndex = Items.OfType<ToolBar>().Where(x => x.Band == GetBandCount()).Max(x => x.BandIndex);
            for (int i = 0; i < newBars.Count(); i++)
            {
                newBars.ElementAt(i).Band = GetBandCount();
                //newBars.ElementAt(i).BandIndex = i + lastBandIndex;
                Debug.WriteLine("Band info set!");
            }
            base.ItemsChanged(e);
        }
    }
}
