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

namespace Mechanism.AvaloniaUI.Core
{
    /*public enum PlatformVisibility
    {
        None,
        Show,
        Hide
    }*/

    public class PerPlatformUI : AvaloniaObject
    {
        public static readonly StyledProperty<PlatformSpecializations> CurrentPlatformsProperty =
            AvaloniaProperty.Register<PerPlatformUI, PlatformSpecializations>(nameof(CurrentPlatforms), defaultValue: new PlatformSpecializations());

        public PlatformSpecializations CurrentPlatforms
        {
            get => GetValue(CurrentPlatformsProperty);
            internal set => SetValue(CurrentPlatformsProperty, value);
        }


        public static PerPlatformUI Instance { get; set; } = new PerPlatformUI();

        static PerPlatformUI()
        {
            CurrentPlatformsProperty.Changed.AddClassHandler(new Action<PerPlatformUI, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                sender.CurrentPlatformsChanged?.Invoke(sender, new EventArgs());
                if (e.OldValue != null)
                    ((PlatformSpecializations)e.OldValue).CollectionChanged -= sender.CurrentPlatforms_CollectionChanged;

                if (e.NewValue != null)
                    ((PlatformSpecializations)e.NewValue).CollectionChanged += sender.CurrentPlatforms_CollectionChanged;
            }));

            ShowOnPlatformsProperty.Changed.AddClassHandler<Control>(new Action<Control, AvaloniaPropertyChangedEventArgs>((sender, e) => ManagePerPlatformVisibilityBehavior(sender)));
            HideOnPlatformsProperty.Changed.AddClassHandler<Control>(new Action<Control, AvaloniaPropertyChangedEventArgs>((sender, e) => ManagePerPlatformVisibilityBehavior(sender)));
        }

        private void CurrentPlatforms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CurrentPlatformsChanged?.Invoke(sender, new EventArgs());
        }

        public event EventHandler<EventArgs> CurrentPlatformsChanged;

        private PerPlatformUI()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                CurrentPlatforms.Add("Windows");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                CurrentPlatforms.Add("OSX");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                CurrentPlatforms.Add("Linux");
        }

        public static readonly StyledProperty<PlatformSpecializations> ShowOnPlatformsProperty =
            AvaloniaProperty.RegisterAttached<PerPlatformUI, Control, PlatformSpecializations>("ShowOnPlatforms", null/*, coerce: CoerceOnPlatformsProperties*/);

        public static PlatformSpecializations GetShowOnPlatforms(IAvaloniaObject obj)
        {
            return obj.GetValue(ShowOnPlatformsProperty);
        }

        public static void SetShowOnPlatforms(IAvaloniaObject obj, PlatformSpecializations value)
        {
            obj.SetValue(ShowOnPlatformsProperty, value);
        }


        public static readonly StyledProperty<PlatformSpecializations> HideOnPlatformsProperty =
            AvaloniaProperty.RegisterAttached<PerPlatformUI, Control, PlatformSpecializations>("HideOnPlatforms", null/*, coerce: CoerceOnPlatformsProperties*/);

        public static PlatformSpecializations GetHideOnPlatforms(IAvaloniaObject obj)
        {
            return obj.GetValue(HideOnPlatformsProperty);
        }

        public static void SetHideOnPlatforms(IAvaloniaObject obj, PlatformSpecializations value)
        {
            obj.SetValue(HideOnPlatformsProperty, value);
        }

        internal static bool HasPlatformVisibility(IAvaloniaObject obj)
        {
            return ((GetShowOnPlatforms(obj) != null) && (GetShowOnPlatforms(obj).Count > 0)) ||
                ((GetHideOnPlatforms(obj) != null) && (GetHideOnPlatforms(obj).Count > 0));
        }

        /*static PlatformSpecializations CoerceOnPlatformsProperties(IAvaloniaObject obj, PlatformSpecializations inValue)
        {
            ManagePerPlatformVisibilityBehavior(obj);
            return inValue;
        }*/

        static void ManagePerPlatformVisibilityBehavior(IAvaloniaObject obj)
        {
            if (obj is AvaloniaObject aObj)
            {
                if (HasPlatformVisibility(aObj))
                {
                    if (!Interaction.GetBehaviors(aObj).Any(x => x is PerPlatformVisibilityBehavior))
                        Interaction.GetBehaviors(aObj).Add(new PerPlatformVisibilityBehavior());
                    Debug.WriteLine("Behavior added!");
                }
                else
                {
                    if (Interaction.GetBehaviors(aObj).Any(x => x is PerPlatformVisibilityBehavior))
                    {
                        for (int i = (Interaction.GetBehaviors(aObj).Count - 1); i > 0; i--)
                        {
                            if (Interaction.GetBehaviors(aObj).ElementAt(i) is PerPlatformVisibilityBehavior)
                                Interaction.GetBehaviors(aObj).RemoveAt(i);
                        }
                    }
                }
            }
        }
    }

    public class PlatformSpecializations : AvaloniaList<string>
    {
        public static string AllPlatforms = "*";
        public static string NoPlatforms = "None";

        public PlatformSpecializations()
        {
        }

        public PlatformSpecializations(IEnumerable<string> items) : base(items)
        {
        }

        public PlatformSpecializations(params string[] items) : base(items)
        {
        }

        public static PlatformSpecializations Parse(string s)
        {
            var ret = new PlatformSpecializations(s.Split(','));
            Debug.WriteLine(ret.ToString());
            return ret;
        }

        public bool HasPlatform(string platform)
        {
            /*if (this.Any(x => x.Equals(AllPlatforms, StringComparison.OrdinalIgnoreCase)))
                return true;
            else if (this.Any(x => x.Equals(NoPlatforms, StringComparison.OrdinalIgnoreCase)))
                return false;
            else
                return this.Any(x => x.Equals(platform, StringComparison.OrdinalIgnoreCase));*/
            return Contains(platform);
        }

        public override string ToString()
        {
            string output = string.Empty;
            
            foreach (string s in this)
            {
                output += s + ", ";
            }
            
            //if (output.Contains(", "))
            output = /*output.Substring(0, output.LastIndexOf(", "));*/ output.TrimEnd(", ".ToCharArray());

            return output;
        }
    }
}