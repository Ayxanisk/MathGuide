using MathGuide.Models;

namespace MathGuide.Services;

/// <summary>
/// Умный калькулятор. Реализацию можно подменить: локальная модель,
/// собственный backend или облачный API распознавания формул.
/// </summary>
public interface ISolverService
{
    Task<SolutionResult> SolveTextAsync(string question, CancellationToken ct = default);
    Task<SolutionResult> SolvePhotoAsync(Stream photo, CancellationToken ct = default);
}

/// <summary>Заглушка для отладки интерфейса без сети.</summary>
public class DemoSolverService : ISolverService
{
    public async Task<SolutionResult> SolveTextAsync(string question, CancellationToken ct = default)
    {
        await Task.Delay(700, ct);
        return new SolutionResult
        {
            Recognized = question,
            Answer = "x = 4",
            Steps = new List<string>
            {
                "Переносим слагаемые с x в левую часть",
                "Приводим подобные: 3x = 12",
                "Делим обе части на 3: x = 4"
            }
        };
    }

    public async Task<SolutionResult> SolvePhotoAsync(Stream photo, CancellationToken ct = default)
    {
        await Task.Delay(1200, ct);
        return new SolutionResult
        {
            Recognized = "5x − 8 = 2x + 4",
            Answer = "x = 4",
            Steps = new List<string>
            {
                "Распознано с фото: 5x − 8 = 2x + 4",
                "Переносим: 5x − 2x = 4 + 8",
                "3x = 12  →  x = 4"
            }
        };
    }
}
