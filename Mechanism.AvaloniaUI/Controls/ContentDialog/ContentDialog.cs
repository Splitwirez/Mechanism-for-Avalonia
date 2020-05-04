using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mechanism.AvaloniaUI.Controls.ContentDialog
{
    public class ContentDialog : AvaloniaObject
    {
        public static TopLevel LastActiveTopLevel = null;

        public static readonly AttachedProperty<bool> MonitorActiveProperty =
            AvaloniaProperty.RegisterAttached<ContentDialog, TopLevel, bool>("MonitorActive", false);

        public static bool IsShowingDialog { get; internal set; } = false;
        public static bool UseDialogQueue { get; set; } = true;

        public static Queue<Task> DialogQueue { get; internal set; } = new Queue<Task>();

        static ContentDialog()
        {
            ContentDialogFrame.DialogClosed += (sneder, args) =>
            {
                if (UseDialogQueue && (DialogQueue.Count > 0))
                {
                    Task task = DialogQueue.Dequeue();
                    task.Start();
                    //action.Invoke();
                }
            };

            MonitorActiveProperty.Changed.AddClassHandler<TopLevel>(new Action<TopLevel, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                var isMonitoring = (bool)(e.NewValue);
                if (isMonitoring)
                {
                    if ((Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime dskLifetime) && (sender is Window win))
                    {
                        //var win = sender as Window;
                        win.Activated += Window_Activated;
                        win.Closed += (sneder, args) => win.SetValue(MonitorActiveProperty, false);
                        if (win.IsActive)
                            LastActiveTopLevel = sender;
                    }
                    else
                        LastActiveTopLevel = sender;
                }
                else if ((Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime dskLifetime) && (sender is Window win))
                {
                    //var win = sender as Window;
                    win.Activated -= Window_Activated;
                }
            }));
        }

        private static void Window_Activated(object sender, EventArgs e)
        {
            LastActiveTopLevel = (TopLevel)sender;
        }


        public static void Show(string title, string content, bool activateIfOnDesktop = true) => Show(title, content, LastActiveTopLevel, activateIfOnDesktop);
        public static void Show(string title, string content, TopLevel topLevel, bool activateIfOnDesktop = true)
        {
            if (topLevel == null)
                throw new Exception("topLevel cannot be null!");

            if (!IsShowingDialog)
                UnsafeShow(title, content, topLevel, activateIfOnDesktop);
            else if (UseDialogQueue)
            {
                DialogQueue.Enqueue(new Task<object>(() =>
                {
                    UnsafeShow(title, content, topLevel);
                    return null;
                }));
            }
            else
                throw new Exception();
        }

        public static async Task<T> ShowWithActionEnum<T>(string title, string content, bool activateIfOnDesktop = true) where T : Enum => await ShowWithActionEnum<T>(title, content, LastActiveTopLevel, activateIfOnDesktop);
        public static async Task<T> ShowWithActionEnum<T>(string title, string content, TopLevel topLevel, bool activateIfOnDesktop = true) where T : Enum
        {
            if (topLevel == null)
                throw new Exception("topLevel cannot be null!");

            Task<T> task = UnsafeShowWithActionEnum<T>(title, content, topLevel, activateIfOnDesktop);
            if (!IsShowingDialog)
            {
                //Debug.wrote
                //task.Start();
                return await task;
            }
            //catch (Exception ex)
            else if (UseDialogQueue)
            {
                DialogQueue.Enqueue(task);
                //task.GetAwaiter().IsCompleted;
                await Task.Run(new Action(() =>
                {
                    while (!task.IsCompleted) { }
                }));
                return task.Result;
            }
            else
                throw new Exception();
        }

        static async Task<T> UnsafeShowWithActionEnum<T>(string title, string content, TopLevel topLevel, bool activateIfOnDesktop = true) where T : Enum
        {
            if (activateIfOnDesktop && (topLevel is Window win))
                win.Activate();

            var actionSet = new EnumActionSet<T>();
            var messageActionDialog = new MessageActionDialog(actionSet)
            {
                Title = title,
                Message = content
            };

            /*await Task.Run(*/
            var task = new Task<T>(new Func<T>(() =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    var layer = OverlayLayer.GetOverlayLayer(topLevel);
                    var frame = ContentDialogFrame.GetFrame(messageActionDialog, topLevel);
                    layer.Children.Add(frame);
                });

                T value = default;
                bool assigned = false;
                messageActionDialog.ActionSelected += (sneder, args) =>
                {
                    value = (T)args.Value;
                    assigned = true;
                };
                while (!assigned)
                { }
                return value;
            }));

            return await task;
        }

        static void UnsafeShow(string title, string content, TopLevel topLevel, bool activateIfOnDesktop = true)
        {
            Dispatcher.UIThread.Post(new Action(() =>
            {
                if (activateIfOnDesktop && (topLevel is Window win))
                    win.Activate();

                OverlayLayer.GetOverlayLayer(topLevel).Children.Add(ContentDialogFrame.GetFrame(new BasicMessageDialog()
                {
                    Title = title,
                    Message = content
                }, topLevel));
            }));
        }
    }
}   