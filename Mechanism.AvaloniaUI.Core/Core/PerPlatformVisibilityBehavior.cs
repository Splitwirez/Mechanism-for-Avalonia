using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mechanism.AvaloniaUI.Core
{
    internal class PerPlatformVisibilityBehavior : Behavior<Control>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            PerPlatformUI.Instance.CurrentPlatformsChanged += (sender, e) => UpdatePlatformVisibility();
            UpdatePlatformVisibility();
        }

        public void UpdatePlatformVisibility()
        {
            if (PerPlatformUI.HasPlatformVisibility(AssociatedObject))
            {
                Debug.WriteLine("PerPlatformUI.HasPlatformVisibility(AssociatedObject)");
                /*foreach (string platform in PerPlatformUI.Instance.CurrentPlatforms)
                {*/
                bool? showOnThisPlatform = false;
                bool? hideOnThisPlatform = false;
                Debug.WriteLine("CurrentPlatforms: " + PerPlatformUI.Instance.CurrentPlatforms.ToString());
                if (PerPlatformUI.GetShowOnPlatforms(AssociatedObject) != null)
                {
                    Debug.WriteLine("PerPlatformUI.GetShowOnPlatforms(AssociatedObject) != null");
                    showOnThisPlatform = PerPlatformUI.GetShowOnPlatforms(AssociatedObject).Any(x => PerPlatformUI.Instance.CurrentPlatforms.HasPlatform(x));
                }
                else
                    showOnThisPlatform = null;

                if (PerPlatformUI.GetHideOnPlatforms(AssociatedObject) != null)
                {
                    Debug.WriteLine("PerPlatformUI.GetHideOnPlatforms(AssociatedObject) != null");
                    hideOnThisPlatform = PerPlatformUI.GetHideOnPlatforms(AssociatedObject).Any(x => PerPlatformUI.Instance.CurrentPlatforms.HasPlatform(x));
                }
                else
                    hideOnThisPlatform = null;

                Debug.WriteLine("showOnThisPlatform: " + showOnThisPlatform + "\nhideOnThisPlatform: " + hideOnThisPlatform);

                if (showOnThisPlatform.HasValue && hideOnThisPlatform.HasValue)
                {
                    if (showOnThisPlatform.Value && hideOnThisPlatform.Value)
                        AssociatedObject.IsVisible = true; //TODO: Add conflict resolution option
                    else if (showOnThisPlatform.Value)
                        AssociatedObject.IsVisible = true;
                    else if (hideOnThisPlatform.Value)
                        AssociatedObject.IsVisible = false;
                }
                else if (showOnThisPlatform.HasValue)
                    AssociatedObject.IsVisible = showOnThisPlatform.Value;
                else if (hideOnThisPlatform.HasValue)
                    AssociatedObject.IsVisible = !hideOnThisPlatform.Value;
            }
            else
                Debug.WriteLine("false == PerPlatformUI.HasPlatformVisibility(AssociatedObject)");
        }
    }
}
