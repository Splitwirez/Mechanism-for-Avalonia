using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System;
using System.Linq;
using Avalonia.Styling;
using System.Collections.ObjectModel;
using Avalonia.Metadata;
using System.Collections;
using Avalonia.Collections;

namespace Mechanism.AvaloniaUI.Core
{
    public class DataTrigger : Behavior<Visual>
    {
        public static readonly DirectProperty<DataTrigger, IEnumerable> SettersProperty =
            Trigger.SettersProperty.AddOwner<DataTrigger>(o => o.Setters, (o, v) => o.Setters = v);

        private IEnumerable _setters = new AvaloniaList<TriggerSetter>();
        [Content]
        public IEnumerable Setters
        {
            get => _setters;
            set => SetAndRaise(SettersProperty, ref _setters, value);
        }


        public static readonly StyledProperty<object> BindingProperty =
            AvaloniaProperty.Register<DataTrigger, object>(nameof(Binding), defaultBindingMode: Avalonia.Data.BindingMode.OneWay);
        
        public object Binding
        {
            get => GetValue(BindingProperty);
            set => SetValue(BindingProperty, value);
        }


        public static readonly StyledProperty<object> ValueProperty =
            Trigger.ValueProperty.AddOwner<DataTrigger>();
        
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }


        static DataTrigger()
        {
            BindingProperty.Changed.AddClassHandler<DataTrigger>((sender, e) => sender.Refresh());
            ValueProperty.Changed.AddClassHandler<DataTrigger>((sender, e) => sender.Refresh());
        }

        void Refresh()
        {
            if ((AssociatedObject != null) && (Avalonia.VisualTree.VisualExtensions.GetVisualRoot(AssociatedObject) != null))
            {
                foreach (TriggerSetter s in Setters)
                {
                    string targetName = s.TargetName;
                
                    var targetObj = AssociatedObject.FindNameScope().Get<Visual>(targetName);
                    Console.WriteLine("Matched: " + (Binding.ToString() == Value.ToString()).ToString() + ", " + Binding.ToString() + ", " + Value.ToString());
                    if (Binding.ToString() == Value.ToString())
                    {
                        Console.WriteLine("Matched! Setting for " + targetName);
                        var targetType = targetObj.GetType();
                        var style = new TriggerStyle()
                        {
                            Selector = Selectors.Name(Selectors.OfType(null, targetType), targetName),
                            Setter = s
                        };
                        
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
                    }
                }
            }
        }
    }
}