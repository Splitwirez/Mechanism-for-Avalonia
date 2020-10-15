using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using System;
using Avalonia.Xaml.Interactivity;
using Avalonia.Layout;
using Mechanism.AvaloniaUI.Behaviors;
using Avalonia.Controls.Generators;
using System.Collections;
using System.Collections.ObjectModel;
using Avalonia.Metadata;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Linq;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class SegmentedControlToolStripItem : Control, IStyleable, IToolStripItem
    {
        private static readonly FuncTemplate<IPanel> DefaultPanel =
            new FuncTemplate<IPanel>(() => 
            {
                var stacc = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };
                Interaction.GetBehaviors(stacc).Add(new StackPanelCornerCurvesBehavior());
                return stacc;
            });
        
        public static readonly StyledProperty<ITemplate<IPanel>> ItemsPanelProperty =
            ItemsControl.ItemsPanelProperty.AddOwner<SegmentedControlToolStripItem>();
        
        public ITemplate<IPanel> ItemsPanel
        {
            get => GetValue(ItemsPanelProperty);
            set => SetValue(ItemsPanelProperty, value);
        }


        public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner<SegmentedControlToolStripItem>();

        public IDataTemplate ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly StyledProperty<object> SelectedItemProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItem, object>(nameof(SelectedItem), null);

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }


        public static readonly StyledProperty<IControlTemplate> TemplateProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItem, IControlTemplate>(nameof(Template));

        public IControlTemplate Template
        {
            get => GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }

        private IEnumerable _items = new ObservableCollection<object>();
        public static readonly DirectProperty<SegmentedControlToolStripItem, IEnumerable> ItemsProperty =
            AvaloniaProperty.RegisterDirect<SegmentedControlToolStripItem, IEnumerable>(nameof(Items), o => o.Items, (o, v) => o.Items = v);
        
        [Content]
        public IEnumerable Items
        {
            get => _items;
            set => SetAndRaise(ItemsProperty, ref _items, value);
        }
        
        public static readonly StyledProperty<string> DisplayNameProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItem, string>(nameof(DisplayName), string.Empty);

        public string DisplayName
        {
            get => GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }


        static string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        static Random _groupRandom = new Random();
        static string GetRandomGroupName()
        {
            /*var stringChars = new char[8];
            
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = _chars[_groupRandom.Next(_chars.Length)];
            }

            return new String(stringChars);*/
            return System.IO.Path.GetRandomFileName();
        }

        public static readonly StyledProperty<string> GroupNameProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItem, string>(nameof(GroupName), string.Empty);

        public string GroupName
        {
            get => GetValue(GroupNameProperty);
            protected set => SetValue(GroupNameProperty, value);
        }

        public static readonly StyledProperty<bool> AllowDuplicatesProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItem, bool>(nameof(AllowDuplicates), false);

        public bool AllowDuplicates
        {
            get => GetValue(AllowDuplicatesProperty);
            set => SetValue(AllowDuplicatesProperty, value);
        }

        public static readonly StyledProperty<ToolStrip> OwnerProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItem, ToolStrip>(nameof(Owner));

        public ToolStrip Owner
        {
            get => GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
            RoutedEvent.Register<SegmentedControlToolStripItem, SelectionChangedEventArgs>("SelectionChanged", RoutingStrategies.Bubble);
        
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        static SegmentedControlToolStripItem()
        {
            ItemsPanelProperty.OverrideDefaultValue<SegmentedControlToolStripItem>(DefaultPanel);
            SelectedItemProperty.Changed.AddClassHandler<SegmentedControlToolStripItem>((sender, e) =>
            {
                List<object> oldSelection = null;
                if (e.OldValue != null)
                    oldSelection = new List<object>()
                    {
                        e.OldValue
                    };
                
                List<object> newSelection = null;
                if (e.NewValue != null)
                    newSelection = new List<object>()
                    {
                        e.NewValue
                    };
                
                sender.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, oldSelection, newSelection));
            });
        }

        internal bool _settingSelectedItem = false;
        internal void SetSelectedItem(object item)
        {
            _settingSelectedItem = true;
            SelectedItem = item;
            _settingSelectedItem = false;
        }

        public SegmentedControlToolStripItem()
        {
            SegmentedControlToolStripItemSegmentPresenter.AnyClicked += SegmentedControlToolStripItemSegmentPresenter_AnyClicked;
            //GroupName = GetRandomGroupName();
        }

        public void SegmentedControlToolStripItemSegmentPresenter_AnyClicked(object sender, EventArgs e)
        {
            var dataContext = (sender as Visual).DataContext;
            var relevantItems = _items.OfType<SegmentedControlToolStripItemSegment>();
            if (relevantItems.Contains(dataContext))
            {
                SelectedItem = dataContext;
                (sender as SegmentedControlToolStripItemSegmentPresenter).IsSelected = true;
                AnySelectionChanged?.Invoke(new object[]
                {
                    this,
                    relevantItems
                }, new EventArgs());
            }
        }

        internal static event EventHandler<EventArgs> AnySelectionChanged;

        Type IStyleable.StyleKey => typeof(SegmentedControlToolStripItem);
    }
}
