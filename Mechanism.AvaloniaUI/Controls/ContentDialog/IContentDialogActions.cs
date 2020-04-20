using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Mechanism.AvaloniaUI.Controls.ContentDialog
{
    public interface IContentDialogActionSet
    {
        IEnumerable<IContentDialogAction> Actions { get; }
    }

    public interface IContentDialogAction
    {
        string Title { get; }

        string Description { get; }

        object Value { get; }
    }

    public static class DialogActions
    {
        public enum OkCancelButtons
        {
            OK,
            Cancel
        }

        public enum IgnoreRetryAbortButtons
        {
            Ignore,
            Retry,
            Abort
        }

        public enum YesNoButtons
        {
            Yes,
            No
        }
    }

    public class ContentDialogAction : IContentDialogAction
    {
        string _title;
        public string Title => _title;

        string _description;
        public string Description => _description;

        object _value;
        public object Value => _value;

        public ContentDialogAction(string title, string description, object value)
        {
            _title = title;
            _description = description;
            _value = value;
        }
    }

    public class EnumActionSet<T> : IContentDialogActionSet where T : Enum
    {
        public IEnumerable<IContentDialogAction> Actions
        {
            get
            {
                var collection = new ObservableCollection<IContentDialogAction>();
                foreach (string enumName in Enum.GetNames(typeof(T)))
                    collection.Add(new ContentDialogAction(enumName, string.Empty, Enum.Parse(typeof(T), enumName)));
                
                return collection;
            }
        }
    }
}
