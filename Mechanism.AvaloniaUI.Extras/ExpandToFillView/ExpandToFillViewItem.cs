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
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Metadata;

namespace Mechanism.AvaloniaUI.Extras
{
    [PseudoClasses(":selected")]
    public class ExpandToFillViewItem : HeaderedContentControl, ISelectable
    {
        /// <summary>
        /// Defines the <see cref="IsSelected"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsSelectedProperty = ListBoxItem.IsSelectedProperty.AddOwner<ExpandToFillViewItem>();
        

        /// <summary>
        /// Gets or sets the selection state of the item.
        /// </summary>
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        static ExpandToFillViewItem()
        {
            SelectableMixin.Attach<ExpandToFillViewItem>(IsSelectedProperty);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            e.NameScope.Find<ToggleButton>("PART_Header").Click += (sneder, args) => 
            {
                if (!IsSelected)
                    IsSelected = true;
            };
            //this.HeaderTemplate
        }
    }
}