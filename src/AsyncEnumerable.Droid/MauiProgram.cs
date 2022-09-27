#if WINDOWS
using Microsoft.Maui.LifecycleEvents;
using WinUIEx;
#endif

namespace AsyncEnumerable.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.OnWindowCreated(window =>
                    {
                        //Set size and center on screen using WinUIEx extension method
                        window.CenterOnScreen(960, 1080);
                    });
                });
            });
#endif

            return builder.Build();
        }
    }
}
