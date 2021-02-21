using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class ToolStripItemPointerOverBehavior : Behavior<Panel>
    {
        public static readonly StyledProperty<ToolStrip> OwnerProperty =
            AvaloniaProperty.Register<ToolStripItemPointerOverBehavior, ToolStrip>(nameof(Owner));

        public ToolStrip Owner
        {
            get => GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        public static readonly StyledProperty<ToolStripItemReference> TargetProperty =
            AvaloniaProperty.Register<ToolStripItemPointerOverBehavior, ToolStripItemReference>(nameof(Target));

        public ToolStripItemReference Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PointerEnter += (sneder, args) =>
            {
                if (Owner != null)
                    Owner.HoverItem = Target;
            };//Owner.HoverItems.Add(TargetItem);
            AssociatedObject.PointerLeave += (sneder, args) =>
            {
                if (Owner != null)
                    Owner.HoverItem = null;
            };
        }
    }
}
