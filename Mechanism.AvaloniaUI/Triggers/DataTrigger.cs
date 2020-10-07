using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Avalonia.Data;
using Avalonia.Styling;
using System.Collections.ObjectModel;
using Avalonia.Styling;
using System.Collections.Specialized;
using Avalonia.Metadata;
using Avalonia.LogicalTree;

namespace Mechanism.AvaloniaUI.Core
{
    public class DataTrigger : Behavior<Visual>
    {
        public static readonly StyledProperty<ObservableCollection<TriggerSetter>> SettersProperty =
            Trigger.SettersProperty.AddOwner<DataTrigger>();
        
        
        [Content]
        public ObservableCollection<TriggerSetter> Setters
        {
            get => GetValue(SettersProperty);
            set => SetValue(SettersProperty, value);
        }


        public static readonly StyledProperty<object> BindingProperty =
            AvaloniaProperty.Register<DataTrigger, object>(nameof(Binding));
        
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

        /*protected override void OnAttached()
        {
            AssociatedObject.AttachedToLogicalTree += AssociatedObject_AttachedToLogicalTree;
        }*/

        void Refresh()
        {
            if ((AssociatedObject != null) && (Avalonia.VisualTree.VisualExtensions.GetVisualRoot(AssociatedObject) != null))
            {
                foreach (TriggerSetter s in Setters)
                {
                    string targetName = /*SourceName;
                    if ((!string.IsNullOrEmpty(s.TargetName)) && (!string.IsNullOrWhiteSpace(s.TargetName)))
                        targetName =*/ s.TargetName;
                
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