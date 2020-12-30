using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mechanism.AvaloniaUI.Controls.ToolStrip;

namespace Mechanism.AvaloniaUI.Sample2.Views
{
    public class ToolStripPageView : UserControl
    {
        TextBlock _lastSegmentTextBlock = null;
        
        public ToolStripPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _lastSegmentTextBlock = this.Find<TextBlock>("LastSegmentTextBlock");
        }


        public void TestSegmentedControlToolStripItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string val = "{x:Null}";
            if ((e.AddedItems != null) && (e.AddedItems.Count > 0))
            {
                if (e.AddedItems[0] is SegmentedControlToolStripItemSegment segment)
                    val = segment.DisplayName;
                else if (e.AddedItems[0] is ContentControl ctrl)
                    val = ctrl.ToString();
                else
                    val = e.AddedItems[0].ToString();
            }
            
            _lastSegmentTextBlock.Text = val;
        }
    }
}