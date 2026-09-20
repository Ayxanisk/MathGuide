using System.Collections.ObjectModel;
using MathGuide.Models;
using MathGuide.Services;

namespace MathGuide.ViewModels;

public class HomeViewModel
{
    public string UserName => UserSession.IsAuthenticated ? UserSession.Name : "Айхан";
    public int Points { get; } = 300;
    public int WeeklyGoal { get; } = 500;
    public int NewTopicsCount { get; } = 4;

    public ObservableCollection<RuleCard> ContinueList { get; } = new()
    {
        new RuleCard
        {
            Title = "Формулы сокращённого умножения",
            Section = "Алгебра", SectionColor = Color.FromArgb("#4F5BD5"),
            Meta = "12 формул", Progress = 0.6
        },
        new RuleCard
        {
            Title = "Признаки равенства треугольников",
            Section = "Геометрия", SectionColor = Color.FromArgb("#F0453C"),
            Meta = "3 признака", Progress = 0.35
        },
        new RuleCard
        {
            Title = "Основные тригонометрические тождества",
            Section = "Тригонометрия", SectionColor = Color.FromArgb("#B93EC9"),
            Meta = "9 формул", Progress = 0.8
        },
        new RuleCard
        {
            Title = "Действия с обыкновенными дробями",
            Section = "Арифметика", SectionColor = Color.FromArgb("#C09257"),
            Meta = "6 правил", Progress = 1.0
        },
    };

    public ObservableCollection<SectionChip> Sections { get; } = new()
    {
        new SectionChip { Title = "Арифметика",    Background = (Brush)Application.Current!.Resources["GradIndigo"] },
        new SectionChip { Title = "Алгебра",       Background = (Brush)Application.Current!.Resources["GradFire"] },
        new SectionChip { Title = "Геометрия",     Background = (Brush)Application.Current!.Resources["GradMagenta"] },
        new SectionChip { Title = "Тригонометрия", Background = (Brush)Application.Current!.Resources["GradGold"] },
    };

}
