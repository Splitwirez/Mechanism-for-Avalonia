using System;
using Avalonia;
using Avalonia.Dialogs;
using Avalonia.Controls.ApplicationLifetimes;
//using Avalonia.Logging.Serilog;

namespace Mechanism.AvaloniaUI.Sample
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            var builder = AppBuilder.Configure<App>()
                .UsePlatformDetect()
                //.UseDirect2D1()
                //.UseManagedSystemDialogs()
                //.UseManagedDialogs()
                //.With(new AvaloniaNativePlatformOptions() { UseDeferredRendering = false })
/*#if DEBUG
                .LogToDebug()
#endif*/
                ;
            ManagedFileDialogExtensions.UseManagedSystemDialogs(builder);
            return builder;
        }
        
    }
}
