using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class ButtonCommandBehavior : Behavior<Button>
    {
        public static readonly StyledProperty<ButtonToolStripItem> TargetItemProperty =
            AvaloniaProperty.Register<ButtonCommandBehavior, ButtonToolStripItem>(nameof(TargetItem));

        public ButtonToolStripItem TargetItem
        {
            get => GetValue(TargetItemProperty);
            set => SetValue(TargetItemProperty, value);
        }

        /*private ICommand _command;
        public static readonly DirectProperty<ButtonCommandBehavior, ICommand> CommandProperty =
            AvaloniaProperty.RegisterDirect<ButtonCommandBehavior, ICommand>(nameof(Command), button => button.Command, (button, command) => button.Command = command, enableDataValidation: true);

        public ICommand Command
        {
            get => _command; //GetValue(CommandProperty);
            set => SetAndRaise(CommandProperty, ref _command, value); //SetValue(CommandProperty, value);
        }

        public static readonly StyledProperty<object> CommandParameterProperty =
            AvaloniaProperty.Register<ButtonCommandBehavior, object>(nameof(CommandParameter));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }*/

        static ButtonCommandBehavior()
        {
            /*CommandProperty.Changed.AddClassHandler<ButtonCommandBehavior>(new Action<ButtonCommandBehavior, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                if (e.NewValue != null)
                {
                    (e.NewValue as ICommand).CanExecuteChanged += Command_CanExecuteChanged;
                    //sender.AssociatedObject[!Button.IsEnabledProperty] = 
                }
                if (e.OldValue != null)
                {
                    (e.NewValue as ICommand).CanExecuteChanged -= Command_CanExecuteChanged;
                    sender.AssociatedObject.IsEnabled = true;
                }
            }));*/
            TargetItemProperty.Changed.AddClassHandler<ButtonCommandBehavior>(new Action<ButtonCommandBehavior, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                sender.UpdateIsEnabled();
            }));
        }

        /*private static void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            var bhv = (sender as ButtonCommandBehavior);
        }*/

        public void UpdateIsEnabled()
        {
            if ((TargetItem != null) && (TargetItem.Command != null))
                AssociatedObject[!Button.IsEnabledProperty] = TargetItem.GetObservable(ButtonToolStripItem.CommandProperty).Select(x => x.CanExecute(TargetItem.CommandParameter)).ToBinding();
            else
                AssociatedObject.IsEnabled = true;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            //TargetItem.Command.CanExecuteChanged += (sender, e) => AssociatedObject.IsEffectivelyEnabled = 
            if (false)
            {
                if (AssociatedObject.DataContext is ButtonToolStripItem item)
                {
                    Debug.WriteLine("AssociatedObject.DataContext is ButtonToolStripItem item");
                    AssociatedObject[!Button.IsEnabledProperty] = item.GetObservable(ButtonToolStripItem.CommandProperty).Select(x => x.CanExecute(item.CommandParameter)).ToBinding();
                    AssociatedObject.Click += (sneder, args) => item.Command.Execute(item.CommandParameter);
                }
                else
                    Debug.WriteLine("AssociatedObject.DataContext IS " + AssociatedObject.DataContext.GetType().FullName);
            };
            Debug.WriteLine("TargetItem be like " + (TargetItem != null).ToString());
            AssociatedObject.Click += (sneder, args) => TargetItem.OnClick();
        }
    }
}
