using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using System;
using Avalonia.Xaml.Interactivity;
using Avalonia.Layout;
using Mechanism.AvaloniaUI.Behaviors;
using Avalonia.Controls.Generators;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Mechanism.AvaloniaUI.Controls.ToolStrip
{
    public class SegmentedControlToolStripItemSegment : AvaloniaObject, IToolStripItem//, INotifyPropertyChanged
    {
        //Type IStyleable.StyleKey => typeof(ListBoxItem);

        /*ICommand _command = null;
        public ICommand Command
        {
            get => _command;
            set
            {
                _command = value;
                NotifyPropertyChanged();
            }
        }
        
        object _commandParameter = null;
        public object CommandParameter
        {
            get => _commandParameter;
            set
            {
                _commandParameter = value;
                NotifyPropertyChanged();
            }
        }*/
        
        /*string _displayName = string.Empty;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                NotifyPropertyChanged();
            }
        }

        IControlTemplate _icon = null;
        public IControlTemplate Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyPropertyChanged();
            }
        }*/

        public static readonly StyledProperty<ICommand> CommandProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItemSegment, ICommand>(nameof(Command));

        public ICommand Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly StyledProperty<object> CommandParameterProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItemSegment, object>(nameof(CommandParameter));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly StyledProperty<string> DisplayNameProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItemSegment, string>(nameof(DisplayName), string.Empty);

        public string DisplayName
        {
            get => GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        public static readonly StyledProperty<IControlTemplate> TemplateProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItemSegment, IControlTemplate>(nameof(Template));

        public IControlTemplate Template
        {
            get => GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }

        public bool AllowDuplicates
        {
            get => false;
            set { }
        }
        public static readonly StyledProperty<ToolStrip> OwnerProperty =
            AvaloniaProperty.Register<SegmentedControlToolStripItemSegment, ToolStrip>(nameof(Owner));

        public ToolStrip Owner
        {
            get => GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        /*void NotifyPropertyChanged([CallerMemberName]string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;*/

        public static readonly StyledProperty<bool> IsSelectedProperty =
            ListBoxItem.IsSelectedProperty.AddOwner<SegmentedControlToolStripItemSegment>();
        
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
    }
}
