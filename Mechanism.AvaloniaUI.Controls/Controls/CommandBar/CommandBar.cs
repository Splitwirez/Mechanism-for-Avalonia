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
    public class CommandBar : OverflowFlyoutItemsControl
    {
        /*public static readonly StyledProperty<object> FarEndContentProperty = AvaloniaProperty.Register<CommandBar, object>(nameof(FarEndContent));

        public object FarEndContent
        {
            get => GetValue(FarEndContentProperty);
            set => SetValue(FarEndContentProperty, value);
        }*/

        public static readonly DirectProperty<CommandBar, IEnumerable> EndItemsProperty =
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
        }

        public static readonly StyledProperty<ObservableCollection<CommandBarLayer>> LayersProperty =
        AvaloniaProperty.Register<OverflowFlyoutItemsControl, ObservableCollection<CommandBarLayer>>(nameof(Layers), defaultValue: new ObservableCollection<CommandBarLayer>());

        public ObservableCollection<CommandBarLayer> Layers
        {
            get => GetValue(LayersProperty);
            set => SetValue(LayersProperty, value);
        }

        public static readonly AttachedProperty<string> VisibleOnLayersProperty =
        AvaloniaProperty.RegisterAttached<CommandBar, Control, string>("VisibleOnLayers", defaultValue: null);

        public static string GetVisibleOnLayers(Control element)
        {
            return element.GetValue(VisibleOnLayersProperty);
        }

        public static void SetVisibleOnLayers(Control element, string value)
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
            LayersProperty.Changed.AddClassHandler<CommandBar>(new Action<CommandBar, AvaloniaPropertyChangedEventArgs>((sneder, args) =>
            {
                if (args.OldValue != null)
                    (args.NewValue as ObservableCollection<CommandBarLayer>).CollectionChanged -= Layers_CollectionChanged;
                if (args.NewValue != null)
                    (args.NewValue as ObservableCollection<CommandBarLayer>).CollectionChanged += Layers_CollectionChanged;
                sneder.UpdateChildrenVisibity();
            }));
            FlyoutItemsPanelProperty.OverrideDefaultValue<CommandBar>(FlyoutDefaultPanel);

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

        public CommandBar()
        {
            CommandBarLayer.IsLayerVisibleChanged += (sneder, args) =>
            {
                if (Layers.Contains(sneder))
                    UpdateChildrenVisibity();
            };
            //UpdateEndItems();
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

        public void UpdateChildrenVisibity()
        {
            foreach (Control c in Items.OfType<Control>()/*.Where(x => (!string.IsNullOrEmpty(GetVisibleOnLayers(x))) && (!string.IsNullOrWhiteSpace(GetVisibleOnLayers(x))))*/)
            {
                string visibleOnLayers = GetVisibleOnLayers(c);
                if ((!string.IsNullOrEmpty(visibleOnLayers)) && (!string.IsNullOrWhiteSpace(visibleOnLayers)))
                {
                    string[] strings = visibleOnLayers.Replace(" ", string.Empty).Split(',');

                    if (Layers.Where(x => (!string.IsNullOrEmpty(x.Identifier)) && (!string.IsNullOrWhiteSpace(x.Identifier))).Any(x => x.IsVisible && strings.Contains(x.Identifier)))
                        c.IsVisible = true;
                    else
                        c.IsVisible = false;
                }
            }
        }

        ItemsPresenter _endItemsPresenter = null;
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            _endItemsPresenter = e.NameScope.Find<ItemsPresenter>("PART_CommandBarEndItemsPresenter");
        }

        protected override double GetBaseWidth()
        {
            return base.GetBaseWidth() + _endItemsPresenter.DesiredSize.Width;
        }

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
