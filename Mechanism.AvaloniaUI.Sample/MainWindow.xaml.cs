using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mechanism.AvaloniaUI.Controls.Windows;

namespace Mechanism.AvaloniaUI.Sample
{
    public class MainWindow : DecoratableWindow
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            //Closing += MainWindow_Closing;
            HasSystemDecorations = true;
        }

        /*private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }*/

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.Find<Button>("FileListmakerDialogButton").Click += FileListmakerDialogButton_Click;
        }

        private void FileListmakerDialogButton_Click(object sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowAsync(this);
        }
    }
}
