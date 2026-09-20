using MathGuide.Pages;
using MathGuide.Services;
using Microsoft.Extensions.Logging;
using Plugin.Maui.OCR;

namespace MathGuide;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseOcr()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Сервисы
        builder.Services.AddSingleton<ISolverService, PhotoMathSolverService>();
        builder.Services.AddSingleton<PdfManagerService>();

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
