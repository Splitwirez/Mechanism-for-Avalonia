using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Avalonia.Data;
using System.Collections.ObjectModel;
using Avalonia.Styling;
using System.Collections.Specialized;

namespace Mechanism.AvaloniaUI.Core
{
    internal class TriggerStyle : Style
    {
        internal TriggerSetter Setter = null;
    }
}