namespace MathGuide.Models;

/// <summary>Крупная карточка раздела на странице «Правила».</summary>
public class MathSection
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public int RuleCount { get; set; }
    public Brush Background { get; set; } = Brush.Default;
    /// <summary>Сдвиг карточки влево/вправо — даёт «лесенку», как в макете.</summary>
    public Thickness Offset { get; set; } = new(0);
}

/// <summary>Маленькая плитка раздела (сетка 2×2).</summary>
public class SectionChip
{
    public string Title { get; set; } = "";
    public Brush Background { get; set; } = Brush.Default;
}

/// <summary>Карточка правила/темы в списке «Продолжить».</summary>
public class RuleCard
{
    public string Title { get; set; } = "";
    public string Section { get; set; } = "";
    public Color SectionColor { get; set; } = Colors.Gray;
    public string Meta { get; set; } = "";
    public double Progress { get; set; }
}

/// <summary>Класс школы (1–11) для горизонтальной ленты.</summary>
public class GradeItem
{
    public int Number { get; set; }
    public string Label => Number.ToString();
    public bool IsSelected { get; set; }
    public Color Background => IsSelected ? Color.FromArgb("#4F5BD5") : Color.FromArgb("#FFFFFF");
    public Color TextColor => IsSelected ? Colors.White : Color.FromArgb("#5A5A75");
}

/// <summary>Часто задаваемый вопрос на странице калькулятора.</summary>
public class FaqItem
{
    public string Question { get; set; } = "";
}

/// <summary>Результат разбора задачи умным калькулятором.</summary>
public class SolutionResult
{
    public string Recognized { get; set; } = "";
    public string Answer { get; set; } = "";
    public List<string> Steps { get; set; } = new();
}
