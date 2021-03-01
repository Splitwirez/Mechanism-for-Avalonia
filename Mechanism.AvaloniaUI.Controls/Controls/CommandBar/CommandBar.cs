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
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using System.Diagnostics;

namespace Mechanism.AvaloniaUI.Controls.CommandBar
{
    public class CommandBar : HeaderedItemsControl //OverflowFlyoutItemsControl
    {
        /*public static readonly StyledProperty<object> FarEndContentProperty = AvaloniaProperty.Register<CommandBar, object>(nameof(FarEndContent));

        public object FarEndContent
        {
            get => GetValue(FarEndContentProperty);
            set => SetValue(FarEndContentProperty, value);
        }*/

        /*public static readonly DirectProperty<CommandBar, IEnumerable> EndItemsProperty =
            AvaloniaProperty.RegisterDirect<CommandBar, IEnumerable>(nameof(EndItems), o => o.EndItems, (o, v) => o.EndItems = v);

        private IEnumerable _endItems = new AvaloniaList<object>();
        public IEnumerable EndItems
        {
            get => _endItems;
            set => SetAndRaise(EndItemsProperty, ref _endItems, value);
        }

        public static readonly StyledProperty<IDataTemplate> EndItemTemplateProperty =
            AvaloniaProperty.Register<CommandBar, IDataTemplate>(nameof(EndItemTemplate));

        public IDataTemplate EndItemTemplate
        {
            get => GetValue(EndItemTemplateProperty);
            set => SetValue(EndItemTemplateProperty, value);
        }

        private static readonly FuncTemplate<IPanel> EndDefaultPanel = new FuncTemplate<IPanel>(() => new StackPanel()
        {
            Orientation = Orientation.Horizontal
        });
        public static readonly StyledProperty<ITemplate<IPanel>> EndItemsPanelProperty =
            AvaloniaProperty.Register<ItemsControl, ITemplate<IPanel>>(nameof(EndItemsPanel), EndDefaultPanel);

        public ITemplate<IPanel> EndItemsPanel
        {
            get => GetValue(EndItemsPanelProperty);
            set => SetValue(EndItemsPanelProperty, value);
        }*/

        
        /*static ObservableCollection<CommandBarLayer> DefaultLayersCollection => new ObservableCollection<CommandBarLayer>();
        public static readonly StyledProperty<ObservableCollection<CommandBarLayer>> LayersProperty =
        AvaloniaProperty.Register<CommandBar, ObservableCollection<CommandBarLayer>>(nameof(Layers), defaultValue: DefaultLayersCollection);

        public ObservableCollection<CommandBarLayer> Layers
        {
            get => GetValue(LayersProperty);
            set => SetValue(LayersProperty, value);
        }*/
        public static readonly DirectProperty<CommandBar, ObservableCollection<CommandBarLayer>> LayersProperty =
            AvaloniaProperty.RegisterDirect<CommandBar, ObservableCollection<CommandBarLayer>>(nameof(Layers), o => o.Layers, (o, v) => o.Layers = v);

        private ObservableCollection<CommandBarLayer> _layers = new ObservableCollection<CommandBarLayer>();
        public ObservableCollection<CommandBarLayer> Layers
        {
            get => _layers;
            set => SetAndRaise(LayersProperty, ref _layers, value);
        }


        public static readonly StyledProperty<ChildrenHorizontalAlignment> HorizontalItemsAlignmentProperty =
            AvaloniaProperty.Register<CommandBar, ChildrenHorizontalAlignment>(nameof(HorizontalItemsAlignment), defaultValue: ChildrenHorizontalAlignment.Left);

        public ChildrenHorizontalAlignment HorizontalItemsAlignment
        {
            get => GetValue(HorizontalItemsAlignmentProperty);
            set => SetValue(HorizontalItemsAlignmentProperty, value);
        }


        public static readonly AttachedProperty<string> VisibleOnLayersProperty =
        AvaloniaProperty.RegisterAttached<CommandBar, IControl, string>("VisibleOnLayers", defaultValue: null);

        public static string GetVisibleOnLayers(IControl element)
        {
            return element.GetValue(VisibleOnLayersProperty);
        }

        public static void SetVisibleOnLayers(IControl element, string value)
        {
            element.SetValue(VisibleOnLayersProperty, value);
        }

        /*public static readonly StyledProperty<bool> IsFlyoutOpenProperty =
            OverflowFlyoutItemsControl.IsFlyoutOpenProperty.AddOwner<CommandBar>();

        public bool IsFlyoutOpen
        {
            get => GetValue(IsFlyoutOpenProperty);
            set => SetValue(IsFlyoutOpenProperty, value);
        }*/

        private static readonly FuncTemplate<IPanel> FlyoutDefaultPanel = new FuncTemplate<IPanel>(() => new StackPanel()
        {
            Orientation = Orientation.Vertical
        });

        static string _endItemsClass = "EndItem";
        static CommandBar()
        {
            LayersProperty.Changed.AddClassHandler<CommandBar>(((sneder, args) =>
            {
                if (args.OldValue != null)
                    (args.NewValue as ObservableCollection<CommandBarLayer>).CollectionChanged -= Layers_CollectionChanged;
                if (args.NewValue != null)
                    (args.NewValue as ObservableCollection<CommandBarLayer>).CollectionChanged += Layers_CollectionChanged;
                sneder.UpdateChildrenVisibity();
            }));
            //FlyoutItemsPanelProperty.OverrideDefaultValue<CommandBar>(FlyoutDefaultPanel);

            /*EndItemsProperty.Changed.AddClassHandler<CommandBar>(new Action<CommandBar, AvaloniaPropertyChangedEventArgs>((sneder, args) =>
            {
                Debug.WriteLine("END ITEMS CHANGED!");
                if (args.OldValue != null)// && (args.OldValue is AvaloniaList<object> old))
                {
                    foreach (StyledElement el in (args.OldValue as IEnumerable<object>).OfType<StyledElement>())
                    {
                        if (el.Classes.Contains(_endItemsClass))
                            el.Classes.Remove(_endItemsClass);
                    }
                }

                if (args.NewValue != null)// && (args.NewValue is AvaloniaList<object> newlist))
                {
                    sneder.UpdateEndItems();
                }
            }));*/
        }

        void UpdateEndItems()
        {
            /*
                //Debug.WriteLine("EndItems.OfType<object>() count: " + EndItems.OfType<object>().Count());
                var enumerator = EndItems.GetEnumerator();
                //foreach (StyledElement el in EndItems.OfType<StyledElement>())
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is StyledElement el)
                    {
                        string adding = "Adding _endItemsClass? ";
                        if (!el.Classes.Contains(_endItemsClass))
                        {
                            el.Classes.Add(_endItemsClass);
                            adding += "Yes";
                        }
                        else
                            adding += "No";
                        Debug.WriteLine(adding);
                    }
                    else
                        Debug.WriteLine("Not a StyledElement!");
                }
            }
            */
        }

        public CommandBar()
        {
            _setChildrenVisibility = new Action<IControl>((c) =>
            {
                /*string visibleOnLayers = GetVisibleOnLayers(c);
                if (string.IsNullOrEmpty(visibleOnLayers) || string.IsNullOrWhiteSpace(visibleOnLayers))
                {
                    var child = Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<IControl>(c);
                    if (child != null)
                        visibleOnLayers = GetVisibleOnLayers(child);
                }
                Debug.WriteLine("" + c + " visibleOnLayers: " + visibleOnLayers);
                if ((!string.IsNullOrEmpty(visibleOnLayers)) && (!string.IsNullOrWhiteSpace(visibleOnLayers)))
                {
                    string[] strings = visibleOnLayers.Replace(" ", string.Empty).Split(',');
                    bool wasVisible = c.IsVisible;
                    if (Layers.Where(x => (!string.IsNullOrEmpty(x.Identifier)) && (!string.IsNullOrWhiteSpace(x.Identifier))).Any(x => x.IsVisible && strings.Contains(x.Identifier)))
                        (c as Control).IsVisible = true;
                    else
                        (c as Control).IsVisible = false;
                    
                    //Debug.WriteLine("IsVisible go from " + wasVisible + " to " + c.IsVisible);
                }*/
                c.IsVisible = ShouldAddItemToPanels(c);
            });

            CommandBarLayer.IsLayerVisibleChanged += (sneder, args) =>
            {
                if (Layers.Contains(sneder))
                    UpdateChildrenVisibity();
            };
        }

        Action<IControl> _setChildrenVisibility = null;

        public void UpdateChildrenVisibity()
        {
            //_itemsPresenter?.RunOnItemContainers(_setChildrenVisibility);
            _itemsPresenter?.ItemsChanged();
        }

        protected override void ItemsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.ItemsChanged(e);
            UpdateChildrenVisibity();
        }

        protected override void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.ItemsCollectionChanged(sender, e);
            UpdateChildrenVisibity();
        }

        private static void Layers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            (sender as CommandBar).UpdateChildrenVisibity();
        }

        //ItemsPresenter _endItemsPresenter = null;
        OverflowFlyoutItemsPresenter _itemsPresenter = null;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            //_endItemsPresenter = e.NameScope.Find<ItemsPresenter>("PART_EndItemsPresenter");
            _itemsPresenter = e.NameScope.Find<OverflowFlyoutItemsPresenter>("PART_ItemsPresenter");
            _itemsPresenter.ShouldAddToPanels = ShouldAddItemToPanels;
        }

        bool ShouldAddItemToPanels(IControl ctrl)
        {
            IControl c = ctrl;
            /*_setChildrenVisibility(ctrl);
            return ctrl.IsVisible;*/
            //string visibleOnLayers = GetVisibleOnLayers(c);
            string visibleOnLayers = GetVisibleOnLayers(c);

            //bool doChild = string.IsNullOrEmpty(visibleOnLayers) || string.IsNullOrWhiteSpace(visibleOnLayers);
            //Debug.WriteLine("doChild: " + doChild);
            ctrl.Measure(Bounds.Size);
            if (ctrl is ContentPresenter pres)
            {
                c = Avalonia.VisualTree.VisualExtensions.FindDescendantOfType<IControl>(pres);
                //c = Avalonia.LogicalTree.LogicalExtensions.GetLogicalDescendants(c).OfType<IControl>().FirstOrDefault(); //.FindDescendantOfType<IControl>(c);
                //Debug.WriteLine("" + c);
                if (c != null)
                {
                    string newVisibleOnLayers = GetVisibleOnLayers(c);
                    if ((!string.IsNullOrEmpty(newVisibleOnLayers)) && (!string.IsNullOrWhiteSpace(newVisibleOnLayers)))
                        visibleOnLayers = newVisibleOnLayers;
                    //Debug.WriteLine("Yes: " + visibleOnLayers);
                }
                else
                {
                    c = ctrl;
                    //Debug.WriteLine("No");
                }
            }
            if (string.IsNullOrEmpty(visibleOnLayers) || string.IsNullOrWhiteSpace(visibleOnLayers))
                return true;
            
            //Debug.WriteLine("" + c + " visibleOnLayers: " + visibleOnLayers);
            if ((!string.IsNullOrEmpty(visibleOnLayers)) && (!string.IsNullOrWhiteSpace(visibleOnLayers)))
            {
                string[] strings = visibleOnLayers.Replace(" ", string.Empty).Split(',');
                bool wasVisible = c.IsVisible;
                if (Layers.Where(x => (!string.IsNullOrEmpty(x.Identifier)) && (!string.IsNullOrWhiteSpace(x.Identifier))).Any(x => x.IsVisible && strings.Contains(x.Identifier)))
                    return true;
            }
            return false;
        }

        /*protected override double GetBaseWidth()
        {
            return base.GetBaseWidth() + _endItemsPresenter.DesiredSize.Width;
        }*/

        /*protected override void LogicalChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.LogicalChildrenCollectionChanged(sender, e);
            if (e.OldItems != null)
            {
                foreach (StyledElement el in e.OldItems.OfType<StyledElement>())
                {
                    if (el.Classes.Contains(_endItemsClass))
                        el.Classes.Remove(_endItemsClass);
                }
            }

            if (e.NewItems != null)
            {
                foreach (StyledElement el in e.NewItems.OfType<StyledElement>())
                {
                    if (!el.Classes.Contains(_endItemsClass))
                        el.Classes.Add(_endItemsClass);
                }
            }
        }*/
    }
}
