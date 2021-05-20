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

namespace Mechanism.AvaloniaUI.Controls
{
    public class ToolBarTray : ItemsControl
    {

        public static readonly StyledProperty<bool> IsLockedProperty =
        AvaloniaProperty.Register<ToolBarTray, bool>(nameof(IsLocked), defaultValue: false);

        public bool IsLocked
        {
            get => GetValue(IsLockedProperty);
            set => SetValue(IsLockedProperty, value);
        }
        
        
        static ToolBarTray()
        {
            AffectsArrange<ToolBarTray>(IsLockedProperty);
            AffectsMeasure<ToolBarTray>(IsLockedProperty);
            AffectsRender<ToolBarTray>(IsLockedProperty);
        }
        
        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ItemContainerGenerator<ToolBar>(this, ToolBar.ContentProperty, ToolBar.ContentTemplateProperty);
        }

        /*protected int GetBandCount()
        {
            if (Items.OfType<ToolBar>().Count() > 0)
                return Items.OfType<ToolBar>().Max(x => /*GetBand(x.GetParentTrayItem()*x.Band);
            else
                return 0;
        }*/

        protected override void ItemsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            /*IEnumerable<ToolBar> newBars = (e.NewValue as IEnumerable).OfType<ToolBar>();
            int lastBandIndex = 0;
            if (Items.OfType<ToolBar>().Count() > 0)
                lastBandIndex = Items.OfType<ToolBar>().Where(x => x.Band == GetBandCount()).Max(x => x.BandIndex);
            for (int i = 0; i < newBars.Count(); i++)
            {
                newBars.ElementAt(i).Band = GetBandCount();
                //newBars.ElementAt(i).BandIndex = i + lastBandIndex;
                //Debug.WriteLine("Band info set!");
            }
            base.ItemsChanged(e);*/
        }
    }
}
