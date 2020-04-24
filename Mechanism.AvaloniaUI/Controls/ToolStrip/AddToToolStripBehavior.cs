using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class AddToToolStripBehavior : Behavior<Thumb>
    {
        public static readonly StyledProperty<ToolStrip> OwnerProperty =
            AvaloniaProperty.Register<AddToToolStripBehavior, ToolStrip>(nameof(Owner));

        public ToolStrip Owner
        {
            get => GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        public static readonly StyledProperty<IToolStripItem> TargetItemProperty =
            AvaloniaProperty.Register<AddToToolStripBehavior, IToolStripItem>(nameof(TargetItem));

        public IToolStripItem TargetItem
        {
            get => GetValue(TargetItemProperty);
            set => SetValue(TargetItemProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            Debug.WriteLine("AddToToolStripBehaviour");
            AssociatedObject.DragCompleted += AssociatedObject_DragCompleted;
        }

        private void AssociatedObject_DragCompleted(object sender, Avalonia.Input.VectorEventArgs e)
        {
            Debug.WriteLine("Drag completed");
            Owner.ValidateAddToToolStrip(TargetItem);
        }
    }
}
