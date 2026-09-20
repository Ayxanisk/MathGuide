using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MathGuide.Models;

namespace MathGuide.ViewModels;

public partial class RulesListViewModel : ObservableObject
{
    private static readonly IReadOnlyList<MathRule> AllRules =
    [
        new MathRule { Subject = "Арифметика", Title = "Действия с дробями", Description = "Сложение, вычитание, умножение и деление обыкновенных дробей.", PdfFileName = "arithmetic_fractions.pdf" },
        new MathRule { Subject = "Арифметика", Title = "Проценты", Description = "Основные правила вычисления процентов и процентных изменений.", PdfFileName = "arithmetic_percentages.pdf" },
        new MathRule { Subject = "Алгебра", Title = "Формулы сокращённого умножения", Description = "Квадрат суммы, квадрат разности и разность квадратов.", PdfFileName = "algebra_short_multiplication.pdf" },
        new MathRule { Subject = "Алгебра", Title = "Квадратные уравнения", Description = "Дискриминант, корни и теорема Виета.", PdfFileName = "algebra_quadratic_equations.pdf" },
        new MathRule { Subject = "Геометрия", Title = "Признаки равенства треугольников", Description = "Три основных признака равенства треугольников.", PdfFileName = "geometry_triangles.pdf" },
        new MathRule { Subject = "Тригонометрия", Title = "Основные тождества", Description = "Базовые тригонометрические тождества и их применение.", PdfFileName = "trigonometry_identities.pdf" },
        new MathRule { Subject = "Функции и графики", Title = "Линейная функция", Description = "График, коэффициенты и свойства линейной функции.", PdfFileName = "functions_linear.pdf" },
        new MathRule { Subject = "Начала анализа", Title = "Производная функции", Description = "Определение производной и таблица основных производных.", PdfFileName = "analysis_derivative.pdf" }
    ];

    public ObservableCollection<MathRule> Rules { get; } = new()
    {
    };

    public string Subject { get; }

    public RulesListViewModel(string subject)
    {
        Subject = subject;
        foreach (var rule in AllRules)
        {
            if (string.Equals(rule.Subject, subject, StringComparison.Ordinal))
                Rules.Add(rule);
        }
    }

    [RelayCommand]
    private async Task OpenPdfAsync(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        try
        {
            var safeFileName = Path.GetFileName(fileName);
            var cachedFilePath = Path.Combine(FileSystem.CacheDirectory, safeFileName);

            await using var source = await FileSystem.OpenAppPackageFileAsync(safeFileName);
            await using var destination = File.Create(cachedFilePath);
            await source.CopyToAsync(destination);

            var opened = await Launcher.Default.OpenAsync(
                new OpenFileRequest(Path.GetFileName(cachedFilePath), new ReadOnlyFile(cachedFilePath)));

            if (!opened)
                await Shell.Current.DisplayAlert("Не удалось открыть файл", "На устройстве не найдено приложение для чтения PDF.", "Ок");
        }
        catch (FileNotFoundException)
        {
            await Shell.Current.DisplayAlert(
                "Файл пока недоступен",
                $"Добавь {Path.GetFileName(fileName)} в Resources/Raw проекта.",
                "Ок");
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Не удалось открыть PDF '{fileName}': {exception}");
            await Shell.Current.DisplayAlert("Ошибка", "Не удалось открыть файл с правилом.", "Ок");
        }
    }
}
