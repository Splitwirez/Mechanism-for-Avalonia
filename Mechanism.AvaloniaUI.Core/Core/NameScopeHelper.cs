using Avalonia.Controls;
using Avalonia.LogicalTree;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Core
{
    public static class NameScopeHelper
    {
        public static bool TryGet<T>(this INameScope scope, string name, out T result) where T : class
        {
            try
            {
                result = scope.Get<T>(name);
            }
            catch
            {
                result = null;
            }
            return result != null;
        }

        public static bool TryFind<T>(this ILogical scope, string name, out T result) where T : class
        {
            try
            {
                result = scope.Find<T>(name);
            }
            catch
            {
                result = null;
            }
            return result != null;
        }
    }
}
