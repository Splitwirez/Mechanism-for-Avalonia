using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class ButtonToolStripItem : Control, IStyleable, IToolStripItem
    {
        private static readonly IControlTemplate DefaultTemplate = new FuncControlTemplate((tctrl, namescope) => new Button()
        {
            Classes = new Classes("ToolStripButton"),
            DataContext = new Binding("."),
            Content = new TemplatedControl()
            {
                [!TemplatedControl.TemplateProperty] = new Binding()
                {
                    Path = "Icon",
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                    {
                        AncestorType = typeof(ButtonToolStripItem)
                    }
                }//new TemplateBinding(IconProperty)
            }
        });

        private static IControlTemplate DefaultTemplate2 => (IControlTemplate)Application.Current.FindResource("ButtonToolStripItemTemplate");

        public static readonly StyledProperty<IControlTemplate> TemplateProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, IControlTemplate>(nameof(Template)/*, DefaultTemplate2*/);

        public IControlTemplate Template
        {
            get => GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }

        /*public static readonly StyledProperty<IControlTemplate> IconProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, IControlTemplate>(nameof(Icon), null);

        public IControlTemplate Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }*/

        /*private static readonly ICommand DefaultCommand = new Func<object>(() =>
        {
            return null;
        }).Method;*/
        public static readonly StyledProperty<ICommand> CommandProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, ICommand>(nameof(Command), null);
        /*private ICommand _command;
        public static readonly DirectProperty<ButtonToolStripItem, ICommand> CommandProperty =
            AvaloniaProperty.RegisterDirect<ButtonToolStripItem, ICommand>(nameof(Command), button => button.Command, (button, command) => button.Command = command, enableDataValidation: true);*/

        public ICommand Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
            /*get => _command;
            set => SetAndRaise(CommandProperty, ref _command, value);*/
        }

        public static readonly StyledProperty<object> CommandParameterProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, object>(nameof(CommandParameter));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly StyledProperty<string> DisplayNameProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, string>(nameof(DisplayName), string.Empty);

        public string DisplayName
        {
            get => GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        public static readonly StyledProperty<bool> AllowDuplicatesProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, bool>(nameof(AllowDuplicates), false);

        public bool AllowDuplicates
        {
            get => GetValue(AllowDuplicatesProperty);
            set => SetValue(AllowDuplicatesProperty, value);
        }

        public static readonly StyledProperty<ToolStrip> OwnerProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, ToolStrip>(nameof(Owner));

        public ToolStrip Owner
        {
            get => GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        public static readonly RoutedEvent<RoutedEventArgs> ClickEvent = RoutedEvent.Register<Button, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

        public event EventHandler<RoutedEventArgs> Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public void OnClick()
        {
            Debug.WriteLine("OnClick");
            var e = new RoutedEventArgs(ClickEvent);
            RaiseEvent(e);

            if (!e.Handled && Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
                e.Handled = true;
                Debug.WriteLine("Command executed!");
            }
            else if (Command == null)
            {
                Debug.WriteLine("Command == null");
            }
        }

        Type IStyleable.StyleKey => typeof(ButtonToolStripItem);

       /*static ButtonToolStripItem()
        {
            OwnerProperty.Changed.AddClassHandler<ButtonToolStripItem>(new Action<ButtonToolStripItem, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                if ((e.NewValue != null) && (e.NewValue is ToolStrip owner))
                {
                    sender.Command.CanExecute 
                    //new Selector()
                    //sender.Styles.Add((IStyle)owner.Styles[typeof(ButtonToolStripItem)]);
                    //sender.Styles.Add((IStyle)owner.Styles.)
                    //sender.Template = (IControlTemplate)owner.FindResource("ButtonToolStripItemTemplate");
                    //(IControlTemplate)owner.Resources["ButtonToolStripItemTemplate"];
                }
            }));
        }*/

        public ButtonToolStripItem()
        {
            /*this.Template = new FuncControlTemplate((tctrl, namescope) => new Button()
            {
                DataContext = this,
                Classes = new Classes("ToolStripButton"),
                [!Button.CommandProperty] = new Binding("Command"),
                [!Button.CommandParameterProperty] = new Binding("CommandParameter"),
                Content = new TemplatedControl()
                {
                    [!TemplatedControl.TemplateProperty] = new Binding("Icon")
                }
            });*/
            //Bind(TemplateProperty, );
            //this[!ButtonToolStripItem.TemplateProperty] = new DynamicResourceExtension("ButtonToolStripItemTemplate");
            this.Bind(TemplateProperty, new DynamicResourceExtension("ButtonToolStripItemTemplate"));
            //this.prop
            //this.Template = (IControlTemplate)this.FindResource("ButtonToolStripItemTemplate");
            
        }
    }
}
