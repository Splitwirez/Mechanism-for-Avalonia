using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using System;
using Avalonia.Xaml.Interactivity;
using Avalonia.Layout;
using Avalonia.Controls.Generators;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.LogicalTree;
using System.Collections.Generic;
using System.Linq;

namespace Mechanism.AvaloniaUI.Controls
{
    public class SegmentedControlToolStripItemSegmentPresenter : TemplatedControl, IStyleable
    {
        /*public static readonly StyledProperty<IControlTemplate> IconProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItemSegmentPresenter, IControlTemplate>(nameof(Icon));

        public IControlTemplate Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }*/

        public static readonly StyledProperty<string> DisplayNameProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItemSegmentPresenter, string>(nameof(DisplayName));

        public string DisplayName
        {
            get => GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        public static readonly StyledProperty<bool> IsSelectedProperty =
            ListBoxItem.IsSelectedProperty.AddOwner<SegmentedControlToolStripItemSegmentPresenter>();
        
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        static SegmentedControlToolStripItemSegmentPresenter()
        {
            IsSelectedProperty.Changed.AddClassHandler<SegmentedControlToolStripItemSegmentPresenter>((sender, e) =>
            {
                
            });
        }

        SegmentedControlToolStripItem _groupOwner = null;
        public SegmentedControlToolStripItemSegmentPresenter()
        {
            /*PointerReleased += (sender, e) =>
            {   
                
            };*/

            PropertyChanged += SegmentedControlToolStripItemSegmentPresenter_PropertyChanged;

            SegmentedControlToolStripItem.AnySelectionChanged += (sender, args) =>
            {
                var senders = (sender as object[]);
                if ((senders[1] is IEnumerable<SegmentedControlToolStripItemSegment> relevantItems) && (senders[0] is SegmentedControlToolStripItem item) && (relevantItems.Contains(DataContext) && (item.SelectedItem != this.DataContext)))
                {
                    IsSelected = false;
                }
            };
        }
        
        
        internal static event EventHandler<EventArgs> AnyClicked;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            e.NameScope.Find<Button>("PART_SegmentPresenterButton").Click += (sender, args) =>
            {
                AnyClicked?.Invoke(this, new EventArgs());
                /*if ((_groupOwner != null) && (_groupOwner .SelectedItem != this))
                    _groupOwner.SelectedItem = this;*/
            };
        }

        SegmentedControlToolStripItem GetOwnerItem()
        {
            IControl parent = this.Parent;
            while ((parent != null) && (!(parent is SegmentedControlToolStripItem)))
            {
                if (parent.Parent == null)
                    parent = parent.TemplatedParent as IControl;
                else
                    parent = parent.Parent;
            }
            if (parent is SegmentedControlToolStripItem owner)
                return owner;
            else
                return null;
        }

        void SegmentedControlToolStripItemSegmentPresenter_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            //SegmentedControlToolStripItemSegmentPresenter.property
            /*if (e.Property == ParentProperty)
            {
                if (e.NewValue != null)
                {
                    _groupOwner = GetOwnerItem(); //Avalonia.LogicalTree.LogicalExtensions.FindLogicalAncestorOfType<SegmentedControlToolStripItem>(this); //Avalonia.VisualTree.VisualExtensions.FindAncestorOfType<SegmentedControlToolStripItem>(this);
                    if (_groupOwner != null)
                        _groupOwner.SelectionChanged += GroupOwner_SelectionChanged;
                    else
                        Console.WriteLine("_groupOwner == null");
                }
            }*/
        }

        void GroupOwner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsSelected = 
                (e.AddedItems != null) && (e.AddedItems.Contains(this));
        }
                /*|| (e.AddedItems == null)
                || ((e.RemovedItems != null) && (e.RemovedItems.Contains(this)));

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            _groupOwner.SelectionChanged -= GroupOwner_SelectionChanged;
            _groupOwner = null;
        }*/

        Type IStyleable.StyleKey => typeof(SegmentedControlToolStripItemSegmentPresenter);
    }
}
