using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Utils;
using Avalonia.Styling;

namespace Mechanism.AvaloniaUI.Controls
{
    /// <summary>
    /// Base class for controls that present items inside an <see cref="ItemsControl"/>.
    /// </summary>
    public class OverflowFlyoutItemsPresenter : TemplatedControl, IItemsPresenter
    {
        Func<IControl, bool> _shouldAddToPanels = null;
        public Func<IControl, bool> ShouldAddToPanels
        {
            set => _shouldAddToPanels = value;
        }
        /*public static readonly StyledProperty<bool> IsFlyoutOpenProperty =
        AvaloniaProperty.Register<OverflowFlyoutItemsPresenter, bool>(nameof(IsFlyoutOpen), defaultValue: false);

        public bool IsFlyoutOpen
        {
            get => GetValue(IsFlyoutOpenProperty);
            set => SetValue(IsFlyoutOpenProperty, value);
        }*/

        bool _isFlyoutOpen = false;
        public static readonly DirectProperty<OverflowFlyoutItemsPresenter, bool> IsFlyoutOpenProperty =
            AvaloniaProperty.RegisterDirect<OverflowFlyoutItemsPresenter, bool>(nameof(IsFlyoutOpen),
                pres => pres.IsFlyoutOpen, (pres, isOpen) => pres.IsFlyoutOpen = isOpen);
        
        public bool IsFlyoutOpen
        {
            get => _isFlyoutOpen;
            set => SetAndRaise(IsFlyoutOpenProperty, ref _isFlyoutOpen, value);
        }

        bool _hasFlyoutItems = false;
        public static readonly DirectProperty<OverflowFlyoutItemsPresenter, bool> HasFlyoutItemsProperty =
            AvaloniaProperty.RegisterDirect<OverflowFlyoutItemsPresenter, bool>(nameof(HasFlyoutItems),
                pres => pres.HasFlyoutItems, (pres, isOpen) => pres.HasFlyoutItems = isOpen);
        
        public bool HasFlyoutItems
        {
            get => _hasFlyoutItems;
            set => SetAndRaise(HasFlyoutItemsProperty, ref _hasFlyoutItems, value);
        }


        /// <summary>
        /// Defines the <see cref="Items"/> property.
        /// </summary>
        public static readonly DirectProperty<OverflowFlyoutItemsPresenter, IEnumerable> ItemsProperty =
            ItemsControl.ItemsProperty.AddOwner<OverflowFlyoutItemsPresenter>(o => o.Items, (o, v) => o.Items = v);

        /// <summary>
        /// Defines the <see cref="ItemTemplate"/> property.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner<OverflowFlyoutItemsPresenter>();

        private IEnumerable _items;
        private IDisposable _itemsSubscription;
        private bool _createdPanel;
        private IItemContainerGenerator _generator;

        /// <summary>
        /// Initializes static members of the <see cref="ItemsPresenter"/> class.
        /// </summary>
        static OverflowFlyoutItemsPresenter()
        {
            TemplatedParentProperty.Changed.AddClassHandler<OverflowFlyoutItemsPresenter>((x,e) => x.TemplatedParentChanged(e));
            BoundsProperty.Changed.AddClassHandler<OverflowFlyoutItemsPresenter>((x,e) => x.ItemsChanged());
        }

        /*public OverflowFlyoutItemsPresenter()
        {
            LayoutUpdated += (x, e) => ((OverflowFlyoutItemsPresenter)x).ItemsChanged();
        }*/

        /// <summary>
        /// Gets or sets the items to be displayed.
        /// </summary>
        public IEnumerable Items
        {
            get
            {
                return _items;
            }

            set
            {
                _itemsSubscription?.Dispose();
                _itemsSubscription = null;

                if (!IsHosted && _createdPanel && value is INotifyCollectionChanged incc)
                {
                    _itemsSubscription = incc.WeakSubscribe(ItemsCollectionChanged);
                }

                SetAndRaise(ItemsProperty, ref _items, value);

                if (_createdPanel)
                {
                    ItemsChanged();
                }
            }
        }

        /// <summary>
        /// Gets the item container generator.
        /// </summary>
        public IItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                if (_generator == null)
                {
                    _generator = CreateItemContainerGenerator();
                }

                return _generator;
            }

            internal set
            {
                if (_generator != null)
                {
                    throw new InvalidOperationException("ItemContainerGenerator already created.");
                }

                _generator = value;
            }
        }

        /// <summary>
        /// Gets or sets the data template used to display the items in the control.
        /// </summary>
        public IDataTemplate ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }


        private DataTemplates _flyoutDataTemplates;
        //public DataTemplates FlyoutDataTemplates => _flyoutDataTemplates ?? (_flyoutDataTemplates = new DataTemplates());
        public static readonly DirectProperty<OverflowFlyoutItemsPresenter, DataTemplates> FlyoutDataTemplatesProperty =
            AvaloniaProperty.RegisterDirect<OverflowFlyoutItemsPresenter, DataTemplates>(nameof(FlyoutDataTemplates),
                pres => pres.FlyoutDataTemplates, (pres, templates) => pres.FlyoutDataTemplates = templates);
        
        public DataTemplates FlyoutDataTemplates
        {
            get => _flyoutDataTemplates;
            set => SetAndRaise(FlyoutDataTemplatesProperty, ref _flyoutDataTemplates, value);
        }

        /// <summary>
        /// Gets the panel used to display the items.
        /// </summary>
        public IPanel Panel
        {
            get => _mainItemsPanel;
            private set => _mainItemsPanel = (StackPanel)value;
        }

        protected bool IsHosted => TemplatedParent is IItemsPresenterHost;

        StackPanel _mainItemsPanel = null;
        IPanel _flyoutItemsPanel = null;
        //ToggleButton _flyoutButton = null;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            _mainItemsPanel = e.NameScope.Find<StackPanel>("PART_MainItemsPanel");
            _flyoutItemsPanel = e.NameScope.Find<IPanel>("PART_FlyoutItemsPanel");
            //_flyoutButton = e.NameScope.Find<ToggleButton>("PART_FlyoutToggleButton");
            if ((_mainItemsPanel != null) && (_flyoutItemsPanel != null))
                _createdPanel = true;
            else
                _createdPanel = false;

            ItemsChanged();
        }

        /*protected override Size MeasureOverride(Size availableSize)
        {
            ItemsChanged();
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ItemsChanged();
            return base.ArrangeOverride(finalSize);
        }*/

        /// <inheritdoc/>
        public virtual void ScrollIntoView(int index)
        {
        }

        /// <inheritdoc/>
        void IItemsPresenter.ItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_mainItemsPanel != null)
            {
                ItemsChanged(e);
            }
        }

        /// <summary>
        /// Creates the <see cref="ItemContainerGenerator"/> for the control.
        /// </summary>
        /// <returns>
        /// An <see cref="IItemContainerGenerator"/> or null.
        /// </returns>
        protected virtual IItemContainerGenerator CreateItemContainerGenerator()
        {
            var i = TemplatedParent as ItemsControl;
            var result = i?.ItemContainerGenerator;

            if (result == null)
            {
                result = new ItemContainerGenerator(this);
                result.ItemTemplate = ItemTemplate;
            }

            return result;
        }

        /*
        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            Panel.Measure(availableSize);
            return Panel.DesiredSize;
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Panel.Arrange(new Rect(finalSize));
            return finalSize;
        }
        */

        /// <summary>
        /// Called when the <see cref="Panel"/> is created.
        /// </summary>
        /// <param name="panel">The panel.</param>
        protected virtual void PanelCreated(IPanel panel)
        {
        }

        /// <summary>
        /// Called when the items for the presenter change, either because <see cref="Items"/>
        /// has been set, the items collection has been modified, or the panel has been created.
        /// </summary>
        /// <param name="e">A description of the change.</param>
        /// <remarks>
        /// The panel is guaranteed to be created when this method is called.
        /// </remarks>
        
        public virtual void ItemsChanged()
        {
            ItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        protected virtual void ItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            ItemsChanged(this, Items, e);
        }

        protected static void ItemsChanged(
            OverflowFlyoutItemsPresenter owner,
            IEnumerable items,
            NotifyCollectionChangedEventArgs e)
        {
            var generator = owner.ItemContainerGenerator;
            var mainPanel = owner._mainItemsPanel;
            var flyoutPanel = owner._flyoutItemsPanel;

            if (!owner._createdPanel)
            {
                return;
            }

            void Add()
            {
                if (e.NewStartingIndex + e.NewItems.Count < items.OfType<object>().Count())
                {
                    generator.InsertSpace(e.NewStartingIndex, e.NewItems.Count);
                }

                AddContainers(owner, e.NewStartingIndex, e.NewItems);
            }

            void Remove()
            {
                RemoveContainers(generator.RemoveRange(e.OldStartingIndex, e.OldItems.Count), mainPanel, flyoutPanel);
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add();
                    break;

                case NotifyCollectionChangedAction.Remove:
                    Remove();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    RemoveContainers(generator.Dematerialize(e.OldStartingIndex, e.OldItems.Count), mainPanel, flyoutPanel);
                    var containers = AddContainers(owner, e.NewStartingIndex, e.NewItems);

                    var i = e.NewStartingIndex;

                    foreach (var container in containers)
                    {
                        mainPanel.Children[i++] = container.ContainerControl;
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    Remove();
                    Add();
                    break;

                case NotifyCollectionChangedAction.Reset:
                    RemoveContainers(generator.Clear(), mainPanel, flyoutPanel);

                    if (items != null)
                    {
                        AddContainers(owner, 0, items);
                    }

                    break;
            }
        }

        private static IList<ItemContainerInfo> AddContainers(
            OverflowFlyoutItemsPresenter owner,
            int index,
            IEnumerable items)
        {
            //Debug.WriteLine("Adding containers...");
            var generator = owner.ItemContainerGenerator;
            var result = new List<ItemContainerInfo>();
            
            StackPanel mainPanel = owner._mainItemsPanel as StackPanel;
            var flyoutPanel = owner._flyoutItemsPanel;

            //double leftGap = Math.Max(0, (mainPanel.Bounds.Left - owner.Bounds.Left) + mainPanel.Margin.Right);
            //double rightGap = Math.Max(0, (owner.Bounds.Right - owner._flyoutButton.Bounds.Left) + owner._flyoutButton.Margin.Left);
            //double mainPanelPotentialArea = owner.Bounds.Width - (leftGap + rightGap);

            int itemIndex = 0;
            double totalItemsSize = 0;
            foreach (var item in items)
            {
                var i = generator.Materialize(index++, item);
                bool shouldAdd = true;
                if (owner._shouldAddToPanels != null)
                {
                    shouldAdd = owner._shouldAddToPanels(i.ContainerControl);
                }
                if (shouldAdd)
                {
                    i.ContainerControl.Measure(mainPanel.Bounds.Size);
                    totalItemsSize += i.ContainerControl.DesiredSize.Width;
                }
                itemIndex++;
                
                //Debug.WriteLine("totalItemsSize at " + itemIndex + ": " + totalItemsSize);
                if (shouldAdd && (i.ContainerControl != null))
                {
                    if (totalItemsSize <= mainPanel.Bounds.Width)
                    {
                        if (i.Index < mainPanel.Children.Count)
                        {
                            mainPanel.Children.Insert(i.Index, i.ContainerControl);
                        }
                        else
                        {
                            mainPanel.Children.Add(i.ContainerControl);
                        }
                    }
                    else
                    {
                        int flyoutIndex = i.Index - mainPanel.Children.Count;
                        if (flyoutIndex < flyoutPanel.Children.Count)
                        {
                            flyoutPanel.Children.Insert(flyoutIndex, i.ContainerControl);
                        }
                        else
                        {
                            flyoutPanel.Children.Add(i.ContainerControl);
                        }
                    }

                    result.Add(i);
                }
            }
            
            owner.HasFlyoutItems = flyoutPanel.Children.Count > 0;
            return result;
        }

        private static void RemoveContainers(
            IEnumerable<ItemContainerInfo> items,
            params IPanel[] panels)
        {
            //Debug.WriteLine("Removing containers...");
            foreach (var p in panels)
            {
                foreach (var i in items)
                {
                    if (i.ContainerControl != null)
                    {
                        var contCtrl = i.ContainerControl;
                        if (p.Children.Contains(contCtrl))
                            p.Children.Remove(contCtrl);
                    }
                }
            }
        }

        /*
        /// <summary>
        /// Creates the <see cref="Panel"/> when <see cref="ApplyTemplate"/> is called for the first
        /// time.
        /// </summary>
        private void CreatePanel()
        {
            Panel = ItemsPanel.Build();
            Panel.SetValue(TemplatedParentProperty, TemplatedParent);

            LogicalChildren.Clear();
            VisualChildren.Clear();
            LogicalChildren.Add(Panel);
            VisualChildren.Add(Panel);

            _createdPanel = true;

            if (!IsHosted && _itemsSubscription == null && Items is INotifyCollectionChanged incc)
            {
                _itemsSubscription = incc.WeakSubscribe(ItemsCollectionChanged);
            }

            PanelCreated(Panel);
        }*/

        /// <summary>
        /// Called when the <see cref="Items"/> collection changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_createdPanel)
            {
                ItemsChanged(e);
            }
        }

        private void TemplatedParentChanged(AvaloniaPropertyChangedEventArgs e)
        {
            (e.NewValue as IItemsPresenterHost)?.RegisterItemsPresenter(this);
        }

        public void RunOnItemContainers(Action<IControl> action)
        {
            if (_createdPanel)
            {
                /*foreach (ItemContainerInfo info in _generator.Containers)
                {
                    if (info.Item is IControl ctrl)
                    {
                        Debug.WriteLine("1: " + ctrl.GetType().FullName);
                        action(ctrl);
                    }
                    else if (info.ContainerControl != null)
                    {
                        Debug.WriteLine("2: " + info.ContainerControl.GetType().FullName);
                        action(info.ContainerControl);
                    }
                }*/
                foreach (ItemContainerInfo info in _generator.Containers.Where(x => x.ContainerControl != null))
                {
                    //Debug.WriteLine("Info: " + info.Item + ", " + info.ContainerControl + ", " + info.ContainerControl.DataContext);
                    bool dataContextIsItem = (info.ContainerControl.DataContext == info.Item);
                    //bool two = (info.ContainerControl is ContentPresenter pres) && (pres.Content is IControl prCtrl);
                    //Debug.WriteLine(dataContextIsItem + "; " + two);
                    ContentPresenter presenter = null;
                    IControl child = null;

                    if (info.ContainerControl is ContentPresenter pres)
                    {
                        presenter = pres;
                        child = Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<IControl>(presenter);

                    }
                        //Debug.WriteLine("Content type: " + prese.Content);
                    if (dataContextIsItem && (child != null))
                    {
                        //Debug.WriteLine("ContentPresenter go brrrr");
                        action(child); //(info.ContainerControl as ContentPresenter).Content as IControl);
                    }
                    else
                    {
                        //Debug.WriteLine("ContentPresentern't");
                        action(info.ContainerControl);
                    }
                }

                /*foreach (IControl ctrl in _mainItemsPanel.Children.Concat(_flyoutItemsPanel.Children))
                {
                    var descendent = Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<object>(ctrl);
                    Debug.WriteLine("CONTROL TYPE: " + ctrl + "; " + descendent);
                    action(ctrl);
                }*/

                ItemsChanged();
            }
        }
    }
}
