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
    public class ExpandToFillView : SelectingItemsControl
    {
        /// <summary>
        /// Defines the <see cref="ItemHeaderTemplate"/> property.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate> ItemHeaderTemplateProperty =
            AvaloniaProperty.Register<ExpandToFillView, IDataTemplate>(nameof(ItemHeaderTemplate));
        
        /// <summary>
        /// Gets or sets the data template used to display the header content of the items.
        /// </summary>
        public IDataTemplate ItemHeaderTemplate
        {
            get => GetValue(ItemHeaderTemplateProperty);
            set => SetValue(ItemHeaderTemplateProperty, value);
        }



        static ExpandToFillView()
        {
            SelectionModeProperty.OverrideDefaultValue<ExpandToFillView>(SelectionMode.AlwaysSelected);
            SelectionModeProperty.Changed.AddClassHandler<ExpandToFillView>((sneder, e) =>
            {
                if (!((e.NewValue is SelectionMode mode) && (mode == SelectionMode.AlwaysSelected)))
                    throw new Exception("Other selection modes not supported. Please use " + nameof(SelectionMode.AlwaysSelected) + ".");
            });
        }


        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            //return new ItemContainerGenerator<ExpandToFillViewItem>(this, ExpandToFillViewItem.ContentProperty, ExpandToFillViewItem.ContentTemplateProperty);
            return new ExpandToFillViewItemContainerGenerator(this);
        }
    }
}