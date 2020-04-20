using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ContentDialog
{
    public class ContentDialog : AvaloniaObject
    {
        public static TopLevel LastActiveTopLevel = null;

        public static readonly AttachedProperty<bool> MonitorActiveProperty =
            AvaloniaProperty.RegisterAttached<ContentDialog, TopLevel, bool>("MonitorActive", false);

        public static bool IsShowingDialog { get; internal set; } = false;
        public static bool UseDialogQueue { get; set; } = true;

        public static Queue<Action> DialogQueue { get; internal set; } = new Queue<Action>();

        static ContentDialog()
        {
            ContentDialogFrame.DialogClosed += (sneder, args) =>
            {
                if (UseDialogQueue && (DialogQueue.Count > 0))
                {
                    Action action = DialogQueue.Dequeue();
                    action.Invoke();
                }
            };

            MonitorActiveProperty.Changed.AddClassHandler<TopLevel>(new Action<TopLevel, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                var isMonitoring = (bool)(e.NewValue);
                if (isMonitoring)
                {
                    if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime dskLifetime)
                    {
                        var win = sender as Window;
                        win.Activated += Window_Activated;
                        if (win.IsActive)
                            LastActiveTopLevel = sender;
                    }
                    else
                        LastActiveTopLevel = sender;
                }
                else if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime dskLifetime)
                {
                    var win = sender as Window;
                    win.Activated -= Window_Activated;
                }
            }));
        }

        private static void Window_Activated(object sender, EventArgs e)
        {
            LastActiveTopLevel = (TopLevel)sender;
        }


        public static void Show(string title, string content) => Show(title, content, LastActiveTopLevel);
        public static void Show(string title, string content, TopLevel topLevel, bool activateIfOnDesktop = true)
        {
            if (topLevel == null)
                throw new Exception("topLevel cannot be null!");

            try
            {
                UnsafeShow(title, content, topLevel, activateIfOnDesktop);
            }
            catch (Exception ex)
            {
                if (UseDialogQueue)
                    DialogQueue.Enqueue(new Action(() => UnsafeShow(title, content, topLevel)));
                else
                    throw ex;
            }
        }

        static void UnsafeShow(string title, string content, TopLevel topLevel, bool activateIfOnDesktop = true)
        {
            if (activateIfOnDesktop && (topLevel is Window win))
                win.Activate();

            OverlayLayer.GetOverlayLayer(topLevel).Children.Add(ContentDialogFrame.GetFrame(new BasicMessageDialog()
            {
                Title = title,
                Message = content
            }, topLevel));
        }
    }
}   