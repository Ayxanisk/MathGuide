using System.Collections.ObjectModel;
using MathGuide.Models;

namespace MathGuide.ViewModels;

public class RulesViewModel
{
    private static Brush G(string key) => (Brush)Application.Current!.Resources[key];

    public ObservableCollection<MathSection> Sections { get; } = new()
    {
        new MathSection
        {
            Title = "Арифметика", Subtitle = "1–6 класс", RuleCount = 84,
            Background = G("GradIndigo"), Offset = new Thickness(0, 0, 44, 0)
        },
        new MathSection
        {
            Title = "Алгебра", Subtitle = "7–11 класс", RuleCount = 146,
            Background = G("GradFire"), Offset = new Thickness(36, -26, 0, 0)
        },
        new MathSection
        {
            Title = "Геометрия", Subtitle = "7–11 класс", RuleCount = 118,
            Background = G("GradMagenta"), Offset = new Thickness(0, -26, 44, 0)
        },
        new MathSection
        {
            Title = "Тригонометрия", Subtitle = "9–11 класс", RuleCount = 62,
            Background = G("GradGold"), Offset = new Thickness(36, -26, 0, 0)
        },
        new MathSection
        {
            Title = "Функции и графики", Subtitle = "7–11 класс", RuleCount = 73,
            Background = G("GradTeal"), Offset = new Thickness(0, -26, 44, 0)
        },
        new MathSection
        {
            Title = "Начала анализа", Subtitle = "10–11 класс", RuleCount = 55,
            Background = G("GradNight"), Offset = new Thickness(36, -26, 0, 0)
        },
    };
}
