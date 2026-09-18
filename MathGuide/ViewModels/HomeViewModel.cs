using System.Collections.ObjectModel;
using MathGuide.Models;

namespace MathGuide.ViewModels;

public class HomeViewModel
{
    public string UserName { get; } = "Айхан";
    public int Points { get; } = 300;
    public int WeeklyGoal { get; } = 500;
    public int NewTopicsCount { get; } = 4;

    public ObservableCollection<RuleCard> ContinueList { get; } = new()
    {
        new RuleCard
        {
            Title = "Формулы сокращённого умножения",
            Section = "Алгебра", SectionColor = Color.FromArgb("#4F5BD5"),
            Meta = "7 класс · 12 формул", Progress = 0.6
        },
        new RuleCard
        {
            Title = "Признаки равенства треугольников",
            Section = "Геометрия", SectionColor = Color.FromArgb("#F0453C"),
            Meta = "7 класс · 3 признака", Progress = 0.35
        },
        new RuleCard
        {
            Title = "Основные тригонометрические тождества",
            Section = "Тригонометрия", SectionColor = Color.FromArgb("#B93EC9"),
            Meta = "10 класс · 9 формул", Progress = 0.8
        },
        new RuleCard
        {
            Title = "Действия с обыкновенными дробями",
            Section = "Арифметика", SectionColor = Color.FromArgb("#C09257"),
            Meta = "5 класс · 6 правил", Progress = 1.0
        },
    };

    public ObservableCollection<SectionChip> Sections { get; } = new()
    {
        new SectionChip { Title = "Арифметика",    Background = (Brush)Application.Current!.Resources["GradIndigo"] },
        new SectionChip { Title = "Алгебра",       Background = (Brush)Application.Current!.Resources["GradFire"] },
        new SectionChip { Title = "Геометрия",     Background = (Brush)Application.Current!.Resources["GradMagenta"] },
        new SectionChip { Title = "Тригонометрия", Background = (Brush)Application.Current!.Resources["GradGold"] },
    };

    public ObservableCollection<GradeItem> Grades { get; } = new(
        Enumerable.Range(1, 11).Select(n => new GradeItem { Number = n, IsSelected = n == 7 }));
}
