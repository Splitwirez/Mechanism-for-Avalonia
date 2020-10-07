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
using Avalonia.Animation;

namespace Mechanism.AvaloniaUI.Core
{
    public class Trigger : Behavior<Visual>
    {
        public static readonly StyledProperty<ObservableCollection<TriggerSetter>> SettersProperty =
            AvaloniaProperty.Register<Trigger, ObservableCollection<TriggerSetter>>(nameof(Setters), new ObservableCollection<TriggerSetter>());
        
        
        [Content]
        public ObservableCollection<TriggerSetter> Setters
        {
            get => GetValue(SettersProperty);
            set => SetValue(SettersProperty, value);
        }


        public static readonly StyledProperty<string> SourceNameProperty =
            AvaloniaProperty.Register<Trigger, string>(nameof(SourceName));
        
        public string SourceName
        {
            get => GetValue(SourceNameProperty);
            set => SetValue(SourceNameProperty, value);
        }

        public static readonly StyledProperty<string> TargetPropertyProperty =
            AvaloniaProperty.Register<Trigger, string>(nameof(TargetProperty));
        
        public string TargetProperty
        {
            get => GetValue(TargetPropertyProperty);
            set => SetValue(TargetPropertyProperty, value);
        }


        public static readonly StyledProperty<object> ValueProperty =
            AvaloniaProperty.Register<Trigger, object>(nameof(Value));
        
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        Visual _source = null;
        protected override void OnAttached()
        {
            AssociatedObject.AttachedToLogicalTree += AssociatedObject_AttachedToLogicalTree;
        }

        Visual GetDescendantByName(Visual ancestor, string name)
        {
            var descendants = ancestor.GetLogicalDescendants().ToList();
            Console.WriteLine("Descendant count: " + descendants.Count());
            foreach (var v in descendants)
            {
                Console.WriteLine("TYPE: " + v.GetType());
                if (v is Visual vis)
                {
                    Console.WriteLine("NAME: " + vis.Name);
                    if (vis.Name == name)
                        return vis;
                }
            }
            return null;
            /*return descendants.FirstOrDefault(x => 
            {
                if (x is Visual vis)
                    return vis.Name == name;
                else
                    return false;
            }) as Visual;*/
        }

        void AssociatedObject_AttachedToLogicalTree(object sender, LogicalTreeAttachmentEventArgs e)
        {
            AssociatedObject.AttachedToLogicalTree -= AssociatedObject_AttachedToLogicalTree;
            _source = AssociatedObject;

            //var descendants = AssociatedObject.GetLogicalDescendants();
            if ((!string.IsNullOrEmpty(SourceName)) && (!string.IsNullOrWhiteSpace(SourceName)))
            {
                var obj2 = AssociatedObject.FindNameScope().Get<Visual>(SourceName); //GetDescendantByName(AssociatedObject, SourceName);
                if (obj2 != null)
                    _source = obj2;
            }
            
            _source.PropertyChanged += AssociatedObject_PropertyChanged;
        }

        void AssociatedObject_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => 
            {
                if (TargetProperty == e.Property.Name)
                {
                    Console.WriteLine("Target property changed!");
                    foreach (TriggerSetter s in Setters)
                    {
                        string targetName = SourceName;
                        if ((!string.IsNullOrEmpty(s.TargetName)) && (!string.IsNullOrWhiteSpace(s.TargetName)))
                            targetName = s.TargetName;
                        
                        
                        //Avalonia.VisualTree.VisualExtensions.GetVisualDescendants(AssociatedObject).First(x => (x as AvaloniaObject).Name == targetName);
                        
                        var targetObj = AssociatedObject.FindNameScope().Get<Visual>(targetName); //GetDescendantByName(AssociatedObject, targetName);
                        if (e.NewValue.ToString() == Value.ToString())
                        {
                            Console.WriteLine("targetObj != null: " + (targetObj != null).ToString());
                            var targetType = targetObj.GetType();
                            Console.WriteLine("Matched! Setting for " + targetName);
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
            });
        }
    }
}