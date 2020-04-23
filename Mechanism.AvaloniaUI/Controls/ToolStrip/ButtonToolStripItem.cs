using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
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

        public static readonly StyledProperty<IControlTemplate> TemplateProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, IControlTemplate>(nameof(Template), null);

        public IControlTemplate Template
        {
            get => GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }

        public static readonly StyledProperty<IControlTemplate> IconProperty =
            AvaloniaProperty.Register<ButtonToolStripItem, IControlTemplate>(nameof(Icon), null);

        public IControlTemplate Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        private ICommand _command;
        public static readonly DirectProperty<Button, ICommand> CommandProperty =
            AvaloniaProperty.RegisterDirect<Button, ICommand>(nameof(Command), button => button.Command, (button, command) => button.Command = command, enableDataValidation: true);

        public ICommand Command
        {
            get => _command;
            set => SetAndRaise(CommandProperty, ref _command, value);
        }

        public static readonly StyledProperty<object> CommandParameterProperty =
            AvaloniaProperty.Register<Button, object>(nameof(CommandParameter));

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

        //Type IStyleable.StyleKey => typeof(ButtonToolStripItem);

        public ButtonToolStripItem()
        {
            this.Template = new FuncControlTemplate((tctrl, namescope) => new Button()
            {
                DataContext = this,
                Classes = new Classes("ToolStripButton"),
                [!Button.CommandProperty] = new Binding("Command"),
                [!Button.CommandParameterProperty] = new Binding("CommandParameter"),
                Content = new TemplatedControl()
                {
                    [!TemplatedControl.TemplateProperty] = new Binding("Icon")
                }
            });
        }
    }
}
