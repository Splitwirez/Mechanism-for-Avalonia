using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Templates;
using Avalonia.Data;

namespace Mechanism.AvaloniaUI.Extras
{
    /// <summary>
    /// Creates containers for items and maintains a list of created containers.
    /// </summary>
    /// <typeparam name="T">The type of the container.</typeparam>
    public class ExpandToFillViewItemContainerGenerator : ItemContainerGenerator<ExpandToFillViewItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemContainerGenerator{T}"/> class.
        /// </summary>
        /// <param name="owner">The owner control.</param>
        /// <param name="contentProperty">The container's Content property.</param>
        /// <param name="contentTemplateProperty">The container's ContentTemplate property.</param>
        public ExpandToFillViewItemContainerGenerator(IControl owner)
            : base(owner, ExpandToFillViewItem.ContentProperty, ExpandToFillViewItem.ContentTemplateProperty)
        {
        }

        /// <summary>
        /// Gets the container's Header property.
        /// </summary>
        protected AvaloniaProperty HeaderProperty = ExpandToFillViewItem.HeaderProperty;

        /// <summary>
        /// Gets the container's HeaderTemplate property.
        /// </summary>
        protected AvaloniaProperty HeaderTemplateProperty = ExpandToFillViewItem.HeaderTemplateProperty;

        /// <inheritdoc/>
        protected override IControl CreateContainer(object item)
        {
            var container = item as ExpandToFillViewItem;

            if (container != null)
            {
                return container;
            }
            else
            {
                var result = new ExpandToFillViewItem();

                result.SetValue(ContentProperty, item, BindingPriority.Style);
                result.SetValue(ContentTemplateProperty, ItemTemplate, BindingPriority.Style);
                
                result.SetValue(HeaderProperty, item, BindingPriority.Style);
                result.SetValue(HeaderTemplateProperty, (Owner as ExpandToFillView).ItemHeaderTemplate, BindingPriority.Style);


                if (!(item is IControl))
                {
                    result.DataContext = item;
                }

                return result;
            }
        }
    }
}
