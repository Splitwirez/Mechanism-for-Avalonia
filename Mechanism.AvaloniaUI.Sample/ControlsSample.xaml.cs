using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mechanism.AvaloniaUI.Controls.ContentDialog;
using System.Timers;

namespace Mechanism.AvaloniaUI.Sample
{
    public class ControlsSample : UserControl
    {
        public ControlsSample()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            this.Find<Button>("ShowContentDialogButton").Click += ShowContentDialogButton_Click;
            this.Find<Button>("ShowContentDialog2Button").Click += ShowContentDialog2Button_Click;
            this.Find<Button>("ShowContentDialogWithActionsButton").Click += ShowContentDialogWithActionsButton_Click;
        }

        private async void ShowContentDialogWithActionsButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            DialogActions.OkCancelButtons result = await ContentDialog.ShowWithActionEnum<DialogActions.OkCancelButtons>("ContentDialog with result", "This is a ContentDialog with a result value.");
            this.Find<TextBlock>("LastActionResultTextBlock").Text = "Last result text: " + result.ToString();
        }

        private void ShowContentDialog2Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ContentDialog.Show("ContentDialog", "This is a ContentDialog. Another one will be queued up if you wait one second before clicking OK.");
            Timer timer = new Timer(1000);
            timer.Elapsed += (sneder, args) =>
            {
                if (ContentDialog.IsShowingDialog)
                    ContentDialog.Show("ContentDialog", "This is another basic ContentDialog.");
                timer.Stop();
            };
            timer.Start();
        }

        private void ShowContentDialogButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ContentDialog.Show("ContentDialog", "This is a ContentDialog.");
        }
    }
}
