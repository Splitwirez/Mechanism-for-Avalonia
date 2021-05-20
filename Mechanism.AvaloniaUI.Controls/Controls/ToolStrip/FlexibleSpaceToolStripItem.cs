using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Mechanism.AvaloniaUI.Controls
{
    public class FlexibleSpaceToolStripItem : Control, IToolStripItem
    {
        public static readonly StyledProperty<IControlTemplate> TemplateProperty =
            AvaloniaProperty.Register<FlexibleSpaceToolStripItem, IControlTemplate>(nameof(Template));

        public IControlTemplate Template
        {
            get => GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }

        public string DisplayName
        {
            get => "Flexible space";
            set { }
        }

        public bool AllowDuplicates
        {
            get => true;
            set { }
        }

        public static readonly StyledProperty<ToolStrip> OwnerProperty =
            AvaloniaProperty.Register<FlexibleSpaceToolStripItem, ToolStrip>(nameof(Owner));

        public ToolStrip Owner
        {
            get => GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }
    }
}
