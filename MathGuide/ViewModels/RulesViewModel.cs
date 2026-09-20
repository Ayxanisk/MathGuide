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
            Title = "Арифметика", Subtitle = "84 правила", RuleCount = 84,
            Background = G("GradIndigo"), Offset = new Thickness(0, 0, 44, 0)
        },
        new MathSection
        {
            Title = "Алгебра", Subtitle = "146 правил", RuleCount = 146,
            Background = G("GradFire"), Offset = new Thickness(36, -26, 0, 0)
        },
        new MathSection
        {
            Title = "Геометрия", Subtitle = "118 правил", RuleCount = 118,
            Background = G("GradMagenta"), Offset = new Thickness(0, -26, 44, 0)
        },
        new MathSection
        {
            Title = "Тригонометрия", Subtitle = "62 правила", RuleCount = 62,
            Background = G("GradGold"), Offset = new Thickness(36, -26, 0, 0)
        },
        new MathSection
        {
            Title = "Функции и графики", Subtitle = "73 правила", RuleCount = 73,
            Background = G("GradTeal"), Offset = new Thickness(0, -26, 44, 0)
        },
        new MathSection
        {
            Title = "Начала анализа", Subtitle = "55 правил", RuleCount = 55,
            Background = G("GradNight"), Offset = new Thickness(36, -26, 0, 0)
        },
    };
}
