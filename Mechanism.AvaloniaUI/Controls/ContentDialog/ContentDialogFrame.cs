using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ContentDialog
{
    public class ContentDialogFrame : ContentControl
    {
        private ContentDialogFrame() { }

        static ContentDialogFrame()
        {
            IsVisibleProperty.Changed.AddClassHandler<ContentDialogFrame>(new Action<ContentDialogFrame, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                if (!(bool)e.NewValue)
                {
                    (sender.Parent as OverlayLayer).Children.Remove(sender);
                    DialogClosed?.Invoke(sender, new EventArgs());
                }
            }));
        }

        public static event EventHandler<EventArgs> DialogClosed;

        static IInputElement LastFocusedElement = null;
        public static ContentDialogFrame GetFrame(object content, TopLevel topLevel)
        {
            if (ContentDialog.IsShowingDialog)
                throw new Exception("Cannot show more than one content dialog simultaneously!");
            else
            {
                LastFocusedElement = FocusManager.Instance.Current;

                var layer = OverlayLayer.GetOverlayLayer(topLevel);
                System.Diagnostics.Debug.WriteLine("layer == null: " + (layer == null).ToString());
                return new ContentDialogFrame()
                {
                    Content = content,
                    [!Button.WidthProperty] = layer.GetObservable(OverlayLayer.BoundsProperty).Select(x => x.Width).ToBinding(),
                    [!Button.HeightProperty] = layer.GetObservable(OverlayLayer.BoundsProperty).Select(x => x.Height).ToBinding()
                };
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            ContentDialog.IsShowingDialog = true;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            LastFocusedElement.Focus();
            ContentDialog.IsShowingDialog = false;
        }
    }
}
