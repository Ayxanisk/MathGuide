using MathGuide.Pages;
using MathGuide.Services;
using Microsoft.Extensions.Logging;

namespace MathGuide;

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

        // Сервисы
        builder.Services.AddSingleton<ISolverService, DemoSolverService>();

        // Страницы
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<RulesPage>();
        builder.Services.AddTransient<CalculatorPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
