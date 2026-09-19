using MathGuide.Models;
using MathGuide.Services;

namespace MathGuide.Pages;

public partial class CalculatorPage : ContentPage
{
    private readonly ISolverService _solver;

    public List<FaqItem> Faq { get; } = new()
    {
        new FaqItem { Question = "Как решить квадратное уравнение через дискриминант?" },
        new FaqItem { Question = "Чему равна сумма первых 50 натуральных чисел?" },
        new FaqItem { Question = "Как найти площадь трапеции?" },
        new FaqItem { Question = "Формулы приведения — как запомнить?" },
    };

    public CalculatorPage(ISolverService solver)
    {
        InitializeComponent();
        _solver = solver;
        BindingContext = this;
    }

    // Конструктор без параметров нужен, если страница создаётся из Shell DataTemplate
    public CalculatorPage() : this(new DemoSolverService()) { }

    private async void OnSolveText(object? sender, EventArgs e)
    {
        var text = QuestionEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return;

        await RunAsync(() => _solver.SolveTextAsync(text));
    }

    private async void OnTakePhoto(object? sender, EventArgs e)
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            await DisplayAlert("Камера недоступна", "На этом устройстве съёмка не поддерживается.", "Ок");
            return;
        }

        var photo = await MediaPicker.Default.CapturePhotoAsync();
        await HandlePhotoAsync(photo);
    }

    private async void OnPickPhoto(object? sender, EventArgs e)
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        await HandlePhotoAsync(photo);
    }

    private async void OnProfileTapped(object sender, TappedEventArgs e)
        => await Navigation.PushModalAsync(new LoginPage());

    private async Task HandlePhotoAsync(FileResult? photo)
    {
        if (photo is null)
            return;

        // Показываем превью
        var localPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
        await using (var source = await photo.OpenReadAsync())
        await using (var dest = File.Create(localPath))
            await source.CopyToAsync(dest);

        PreviewImage.Source = ImageSource.FromFile(localPath);
        PreviewCard.IsVisible = true;

        await RunAsync(async () =>
        {
            await using var stream = File.OpenRead(localPath);
            return await _solver.SolvePhotoAsync(stream);
        });
    }

    private async Task RunAsync(Func<Task<SolutionResult>> work)
    {
        ResultCard.IsVisible = false;
        Busy.IsVisible = Busy.IsRunning = true;

        try
        {
            var result = await work();
            ShowResult(result);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Не получилось разобрать задачу",
                $"Попробуй сфотографировать ещё раз при хорошем освещении.\n\n{ex.Message}", "Ок");
        }
        finally
        {
            Busy.IsVisible = Busy.IsRunning = false;
        }
    }

    private void ShowResult(SolutionResult result)
    {
        RecognizedLabel.Text = $"Условие: {result.Recognized}";
        AnswerLabel.Text = result.Answer;

        StepsStack.Clear();
        for (var i = 0; i < result.Steps.Count; i++)
        {
            StepsStack.Add(new Label
            {
                Text = $"{i + 1}.  {result.Steps[i]}",
                FontSize = 13.5,
                LineHeight = 1.35,
                TextColor = Color.FromArgb("#5A5A75")
            });
        }

        ResultCard.IsVisible = true;
    }
}
