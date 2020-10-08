using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System;
using System.Linq;
using Avalonia.Styling;
using System.Collections.ObjectModel;
using Avalonia.Metadata;
using Avalonia.LogicalTree;
using Avalonia.Collections;
using System.Collections;

namespace Mechanism.AvaloniaUI.Core
{
    public class Trigger : Behavior<Visual>
    {
        public static readonly DirectProperty<Trigger, IEnumerable> SettersProperty =
            AvaloniaProperty.RegisterDirect<Trigger, IEnumerable>(nameof(Setters), o => o.Setters, (o, v) => o.Setters = v);


        private IEnumerable _setters = new AvaloniaList<TriggerSetter>();
        [Content]
        public IEnumerable Setters
        {
            get => _setters;
            set => SetAndRaise(SettersProperty, ref _setters, value);
        }


        public static readonly StyledProperty<string> SourceNameProperty =
            AvaloniaProperty.Register<Trigger, string>(nameof(SourceName), defaultBindingMode: Avalonia.Data.BindingMode.OneWay);
        
        public string SourceName
        {
            get => GetValue(SourceNameProperty);
            set => SetValue(SourceNameProperty, value);
        }

        public static readonly StyledProperty<string> TargetPropertyProperty =
            AvaloniaProperty.Register<Trigger, string>(nameof(TargetProperty), defaultBindingMode: Avalonia.Data.BindingMode.OneWay);
        
        public string TargetProperty
        {
            get => GetValue(TargetPropertyProperty);
            set => SetValue(TargetPropertyProperty, value);
        }


        public static readonly StyledProperty<object> ValueProperty =
            AvaloniaProperty.Register<Trigger, object>(nameof(Value), defaultBindingMode: Avalonia.Data.BindingMode.OneWay);
        
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }


        static Trigger()
        {
            ValueProperty.Changed.AddClassHandler<Trigger>((sender, e) => sender.ValuePropertyChanged(sender, e));
        }

        Visual _source = null;
        protected override void OnAttached()
        {
            AssociatedObject.AttachedToLogicalTree += AssociatedObject_AttachedToLogicalTree;
        }

        Visual GetDescendantByName(Visual ancestor, string name)
        {
            var descendants = ancestor.GetLogicalDescendants().ToList();
            
            foreach (var v in descendants)
            {
                if (v is Visual vis)
                {
                    if (vis.Name == name)
                        return vis;
                }
            }
            return null;
        }

        void AssociatedObject_AttachedToLogicalTree(object sender, LogicalTreeAttachmentEventArgs e)
        {
            AssociatedObject.AttachedToLogicalTree -= AssociatedObject_AttachedToLogicalTree;
            _source = AssociatedObject;

            if ((!string.IsNullOrEmpty(SourceName)) && (!string.IsNullOrWhiteSpace(SourceName)))
            {
                var obj2 = AssociatedObject.FindNameScope().Get<Visual>(SourceName);
                if (obj2 != null)
                    _source = obj2;
            }
            
            _source.PropertyChanged += AssociatedObject_PropertyChanged;
        }

        void AssociatedObject_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (TargetProperty == e.Property.Name)
            {
                //Console.WriteLine("Target property changed!");
                Refresh(e.NewValue, GetValue(ValueProperty));
            }
        }

        protected void ValuePropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if ((e.NewValue != null) && (AssociatedObject != null) && (Avalonia.VisualTree.VisualExtensions.GetVisualRoot(AssociatedObject) != null))
            {
                Type targetType = _source.GetType();
                var targetProps = AvaloniaPropertyRegistry.Instance.GetRegisteredInherited(targetType).ToList();
                targetProps.AddRange(AvaloniaPropertyRegistry.Instance.GetRegistered(targetType).ToList());
                targetProps.AddRange(AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(targetType).ToList());
                targetProps.AddRange(AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(targetType).ToList());
                
                var targetPropVal = _source.GetValue(targetProps.First(x => x.Name == TargetProperty));
                Refresh(targetPropVal, e.NewValue);
            }
        }

        protected void Refresh(object sourcePropertyValue, object value)
        {
            var sourcePropValue = Convert.ChangeType(sourcePropertyValue, value.GetType());
            foreach (TriggerSetter s in Setters)
            {
                string targetName = SourceName;
                if ((!string.IsNullOrEmpty(s.TargetName)) && (!string.IsNullOrWhiteSpace(s.TargetName)))
                    targetName = s.TargetName;
                
                var targetObj = AssociatedObject.FindNameScope().Get<Visual>(targetName);
                Console.WriteLine("sourcePropValue, value: " + sourcePropValue + ", " + value);
                if (sourcePropValue.Equals(value) || (sourcePropValue == value))
                {
                    var targetType = targetObj.GetType();
                    
                    var style = new TriggerStyle()
                    {
                        Selector = Selectors.OfType(null, targetType).Name(targetName),
                        Setter = s
                    };
                    Console.WriteLine("SELECTOR: " + style.Selector.ToString());
                    var targetProps = AvaloniaPropertyRegistry.Instance.GetRegisteredInherited(targetType).ToList();
                    targetProps.AddRange(AvaloniaPropertyRegistry.Instance.GetRegistered(targetType).ToList());
                    targetProps.AddRange(AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(targetType).ToList());
                    targetProps.AddRange(AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(targetType).ToList());
                    
                    var targetProp = targetProps.First(x => x.Name == s.TargetProperty);
                    
                    style.Setters.Add(new Setter(targetProp, Convert.ChangeType(s.Value, targetProp.PropertyType)));
                    
                    AssociatedObject.Styles.Add(style);
                }
                else
                {
                    Console.WriteLine("Not matched!");
                    var style = AssociatedObject.Styles.OfType<TriggerStyle>().FirstOrDefault(x => x.Setter == s);
                    if (style != null)
                        AssociatedObject.Styles.Remove(style);
                    else
                        Console.WriteLine("Style not found!");
                }
            }
            AssociatedObject.InvalidateVisual();
        }
    }
}