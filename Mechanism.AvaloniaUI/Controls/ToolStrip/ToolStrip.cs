using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class ToolStrip : TemplatedControl
    {
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
        }

        private IEnumerable _availableItems = new AvaloniaList<IToolStripItem>();
        public static readonly DirectProperty<ToolStrip, IEnumerable> AvailableItemsProperty =
            AvaloniaProperty.RegisterDirect<ToolStrip, IEnumerable>(nameof(AvailableItems), o => o.AvailableItems, (o, v) => o.AvailableItems = v);

        [Content]
        public IEnumerable AvailableItems
        {
            get { return _availableItems; }
            set { SetAndRaise(AvailableItemsProperty, ref _availableItems, value); }
        }

        private IEnumerable _defaultItems = new AvaloniaList<ToolStripItemReference>();
        public static readonly DirectProperty<ToolStrip, IEnumerable> DefaultItemsProperty =
            AvaloniaProperty.RegisterDirect<ToolStrip, IEnumerable>(nameof(DefaultItems), o => o.DefaultItems, (o, v) => o.DefaultItems = v);

        public IEnumerable DefaultItems
        {
            get { return _defaultItems; }
            set { SetAndRaise(DefaultItemsProperty, ref _defaultItems, value); }
        }

        private IEnumerable _currentItems = new AvaloniaList<ToolStripItemReference>();
        public static readonly DirectProperty<ToolStrip, IEnumerable> CurrentItemsProperty =
            AvaloniaProperty.RegisterDirect<ToolStrip, IEnumerable>(nameof(CurrentItems), o => o.CurrentItems, (o, v) => o.CurrentItems = v);

        public IEnumerable CurrentItems
        {
            get { return _currentItems; }
            set { SetAndRaise(CurrentItemsProperty, ref _currentItems, value); }
        }

        public ToolStrip()
        {
            //CurrentItems = DefaultItems;
        }
    }
}
