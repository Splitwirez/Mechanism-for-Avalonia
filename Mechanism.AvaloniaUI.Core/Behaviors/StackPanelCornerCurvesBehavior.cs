using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Mechanism.AvaloniaUI.Core;
using System;
using System.Linq;
using Avalonia.Layout;

namespace Mechanism.AvaloniaUI.Behaviors
{
    public class StackPanelCornerCurvesBehavior : Behavior<StackPanel>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PropertyChanged += (sender, e) =>
            {
                 if (e.Property == StackPanel.OrientationProperty)
                    ResetCornerCurves();
            };
            AssociatedObject.Children.CollectionChanged += (sender, e) => ResetCornerCurves();
            ResetCornerCurves();
        }

        public void AssociatedObject_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == StackPanel.OrientationProperty)
                ResetCornerCurves();
        }

        static CornerCurves _noCurves = new CornerCurves(false);
        static CornerCurves _vertFirst = new CornerCurves(true, true, false, false);
        static CornerCurves _vertLast = new CornerCurves(false, false, true, true);
        static CornerCurves _horizFirst = new CornerCurves(true, false, false, true);
        static CornerCurves _horizLast = new CornerCurves(false, true, true, false);
        static CornerCurves _allCurves = new CornerCurves(true);
        public void ResetCornerCurves()
        {
            if (AssociatedObject.Children.Count > 2)
            {
                CornerCurves noCurves = _noCurves;
                foreach (Visual vis in AssociatedObject.Children.Skip(1))
                {
                    CornerCurves.SetCornerCurves(vis, noCurves);
                    /*string typeName = vis.GetType().FullName;
                    if ((vis is IContentControl ctrl) && (ctrl.Content != null))
                        typeName += ", " + ctrl.Content.GetType().FullName;
                    
                    Console.WriteLine("CHILD TYPE: " + typeName);*/
                }
            }

            if (AssociatedObject.Children.Count >= 2)
            {
                if (AssociatedObject.Orientation == Orientation.Vertical)
                {
                    CornerCurves.SetCornerCurves(AssociatedObject.Children.First(), _vertFirst);
                    CornerCurves.SetCornerCurves(AssociatedObject.Children.Last(), _vertLast);
                }
                else
                {
                    CornerCurves.SetCornerCurves(AssociatedObject.Children.First(), _horizFirst);
                    CornerCurves.SetCornerCurves(AssociatedObject.Children.Last(), _horizLast);
                }
            }
            else if (AssociatedObject.Children.Count == 1)
                CornerCurves.SetCornerCurves(AssociatedObject.Children.First(), _allCurves);
        }
    }
}