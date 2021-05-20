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

namespace Mechanism.AvaloniaUI.Controls
{
    public class ButtonToolStripItem : Control, IStyleable, IToolStripItem
    {
        /*private static readonly IControlTemplate DefaultTemplate = new FuncControlTemplate((tctrl, namescope) => new Button()
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
                }
            }
        });

        private static IControlTemplate DefaultTemplate2 => (IControlTemplate)Application.Current.FindResource("ButtonToolStripItemTemplate");*/

        public static readonly StyledProperty<IControlTemplate> TemplateProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, IControlTemplate>(nameof(Template));

        public IControlTemplate Template
        {
            get => GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }
        public static readonly StyledProperty<ICommand> CommandProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, ICommand>(nameof(Command), null);

        public ICommand Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
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

        /*public ButtonToolStripItem()
        {
            this.Bind(TemplateProperty, new DynamicResourceExtension("ButtonToolStripItemTemplate"));
        }*/
    }
}
