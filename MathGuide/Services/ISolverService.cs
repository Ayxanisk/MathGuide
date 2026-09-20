using System.Globalization;
using MathGuide.Models;
using Plugin.Maui.OCR;

namespace MathGuide.Services;

public interface ISolverService
{
    Task<SolutionResult> SolveTextAsync(string question, CancellationToken ct = default);
    Task<SolutionResult> SolvePhotoAsync(Stream photo, CancellationToken ct = default);
}

public sealed class DemoSolverService : ISolverService
{
    public Task<SolutionResult> SolveTextAsync(string question, CancellationToken ct = default) =>
        Task.FromResult(ArithmeticSolver.Solve(question));

    public Task<SolutionResult> SolvePhotoAsync(Stream photo, CancellationToken ct = default) =>
        Task.FromException<SolutionResult>(
            new InvalidOperationException("Для распознавания фото запусти приложение через DI-контейнер."));
}

public sealed class PhotoMathSolverService : ISolverService
{
    private readonly IOcrService _ocr;

    public PhotoMathSolverService(IOcrService ocr) => _ocr = ocr;

    public Task<SolutionResult> SolveTextAsync(string question, CancellationToken ct = default) =>
        Task.FromResult(ArithmeticSolver.Solve(question));

    public async Task<SolutionResult> SolvePhotoAsync(Stream photo, CancellationToken ct = default)
    {
        using var memory = new MemoryStream();
        await photo.CopyToAsync(memory, ct);
        var result = await _ocr.RecognizeTextAsync(memory.ToArray());

        if (!result.Success || string.IsNullOrWhiteSpace(result.AllText))
            throw new InvalidOperationException("На фото не удалось распознать арифметический пример.");

        return ArithmeticSolver.Solve(ArithmeticSolver.NormalizeOcrText(result.AllText));
    }
}

internal static class ArithmeticSolver
{
    public static SolutionResult Solve(string input)
    {
        var expression = NormalizeOcrText(input);
        if (expression.Contains('='))
            return EquationSolver.Solve(expression);

        var parser = new ArithmeticParser(expression);
        var value = parser.Parse();
        var formatted = Format(value);

        return new SolutionResult
        {
            Recognized = expression,
            Answer = formatted,
            Steps = parser.Steps
        };
    }

    public static string NormalizeOcrText(string text)
    {
        var normalized = text
            .Replace('\r', ' ')
            .Replace('\n', ' ')
            .Replace('×', '*')
            .Replace('х', 'x')
            .Replace('Х', 'x')
            .Replace('X', 'x')
            .Replace('÷', '/')
            .Replace('−', '-')
            .Replace('–', '-')
            .Replace(',', '.');

        return new string(normalized
            .Where(c => char.IsDigit(c) || "xX+-*/^=(). ".Contains(c))
            .ToArray())
            .Replace(" ", string.Empty);
    }

    internal static class EquationSolver
    {
        public static SolutionResult Solve(string equation)
        {
            var parts = equation.Split('=', 2);
            if (parts.Length != 2)
                throw new FormatException("Проверь знак равенства в уравнении.");

            var left = PolynomialParser.Parse(parts[0]);
            var right = PolynomialParser.Parse(parts[1]);
            var polynomial = left - right;
            var steps = new List<string>
            {
                $"Переносим всё в левую часть: {polynomial}",
                "Приводим подобные слагаемые."
            };

            if (Math.Abs(polynomial.A) < 1e-10)
            {
                if (Math.Abs(polynomial.B) < 1e-10)
                    return new SolutionResult { Recognized = equation, Answer = "Бесконечно много решений", Steps = steps };
                steps.Add($"Линейное уравнение: {Format(polynomial.B)}x + {Format(polynomial.C)} = 0.");
                var root = -polynomial.C / polynomial.B;
                steps.Add($"x = -c / b = {Format(root)}");
                return new SolutionResult { Recognized = equation, Answer = $"x = {Format(root)}", Steps = steps };
            }

            var discriminant = polynomial.B * polynomial.B - 4 * polynomial.A * polynomial.C;
            steps.Add($"D = b² - 4ac = {Format(discriminant)}.");
            if (discriminant < -1e-10)
                return new SolutionResult { Recognized = equation, Answer = "Действительных решений нет", Steps = steps };

            if (Math.Abs(discriminant) < 1e-10)
            {
                var root = -polynomial.B / (2 * polynomial.A);
                steps.Add($"x = -b / 2a = {Format(root)}");
                return new SolutionResult { Recognized = equation, Answer = $"x = {Format(root)}", Steps = steps };
            }

            var squareRoot = Math.Sqrt(discriminant);
            var first = (-polynomial.B + squareRoot) / (2 * polynomial.A);
            var second = (-polynomial.B - squareRoot) / (2 * polynomial.A);
            steps.Add($"x₁ = (-b + √D) / 2a = {Format(first)}");
            steps.Add($"x₂ = (-b - √D) / 2a = {Format(second)}");
            return new SolutionResult
            {
                Recognized = equation,
                Answer = $"x₁ = {Format(first)}, x₂ = {Format(second)}",
                Steps = steps
            };
        }

        private static string Format(double value) =>
            value.ToString("0.##########", CultureInfo.InvariantCulture);
    }

    internal readonly record struct Polynomial(double A, double B, double C)
    {
        public static Polynomial operator +(Polynomial left, Polynomial right) =>
            new(left.A + right.A, left.B + right.B, left.C + right.C);

        public static Polynomial operator -(Polynomial left, Polynomial right) =>
            new(left.A - right.A, left.B - right.B, left.C - right.C);

        public static Polynomial operator *(Polynomial left, Polynomial right)
        {
            if (Math.Abs(left.A * right.B + left.B * right.A) > 1e-10)
                throw new FormatException("Поддерживаются уравнения не выше второй степени.");
            return new(left.A * right.C + left.B * right.B + left.C * right.A,
                left.B * right.C + left.C * right.B, left.C * right.C);
        }

        public static Polynomial operator /(Polynomial value, double divisor)
        {
            if (Math.Abs(divisor) < 1e-10)
                throw new DivideByZeroException("Деление на ноль невозможно.");
            return new(value.A / divisor, value.B / divisor, value.C / divisor);
        }

        public override string ToString() =>
            $"{A:0.##########}x² + {B:0.##########}x + {C:0.##########}";
    }

    internal sealed class PolynomialParser
    {
        private readonly string _text;
        private int _position;

        private PolynomialParser(string text) => _text = text.Replace(" ", string.Empty);

        public static Polynomial Parse(string text)
        {
            var parser = new PolynomialParser(text);
            var result = parser.ParseExpression();
            if (parser._position != parser._text.Length)
                throw new FormatException("Уравнение содержит неподдерживаемые символы.");
            return result;
        }

        private Polynomial ParseExpression()
        {
            var result = ParseTerm();
            while (TryRead('+') || TryRead('-'))
            {
                var operation = _text[_position - 1];
                var term = ParseTerm();
                result = operation == '+' ? result + term : result - term;
            }
            return result;
        }

        private Polynomial ParseTerm()
        {
            var result = ParseFactor();
            while (_position < _text.Length)
            {
                if (TryRead('*'))
                    result *= ParseFactor();
                else if (TryRead('/'))
                {
                    var divisor = ParseFactor();
                    if (Math.Abs(divisor.A) > 1e-10 || Math.Abs(divisor.B) > 1e-10)
                        throw new FormatException("Делить можно только на число.");
                    result /= divisor.C;
                }
                else if (CanStartFactor())
                    result *= ParseFactor();
                else
                    break;
            }
            return result;
        }

        private Polynomial ParseFactor()
        {
            if (TryRead('+'))
                return ParseFactor();
            if (TryRead('-'))
                return new Polynomial(0, 0, 0) - ParseFactor();

            Polynomial result;
            if (TryRead('('))
            {
                result = ParseExpression();
                if (!TryRead(')'))
                    throw new FormatException("Проверь скобки в уравнении.");
            }
            else if (TryRead('x'))
                result = new(0, 1, 0);
            else
            {
                var start = _position;
                while (_position < _text.Length &&
                       (char.IsDigit(_text[_position]) || _text[_position] == '.'))
                    _position++;
                if (start == _position ||
                    !double.TryParse(_text[start.._position], NumberStyles.Float,
                        CultureInfo.InvariantCulture, out var number))
                    throw new FormatException("Не удалось прочитать число в уравнении.");
                result = new(0, 0, number);
            }

            if (TryRead('^'))
            {
                var start = _position;
                while (_position < _text.Length && char.IsDigit(_text[_position]))
                    _position++;
                if (!int.TryParse(_text[start.._position], out var power) || power is < 0 or > 2)
                    throw new FormatException("Поддерживаются степени от 0 до 2.");
                if (power == 2)
                    result *= result;
            }
            return result;
        }

        private bool CanStartFactor() =>
            _position < _text.Length &&
            (_text[_position] == '(' || _text[_position] == 'x' || char.IsDigit(_text[_position]));

        private bool TryRead(char expected)
        {
            if (_position >= _text.Length || _text[_position] != expected)
                return false;
            _position++;
            return true;
        }
    }

    internal static string Format(double value) =>
        value.ToString("0.##########", CultureInfo.InvariantCulture);
}

internal sealed class ArithmeticParser
{
    private readonly string _expression;
    private int _position;

    public List<string> Steps { get; } = new();

    public ArithmeticParser(string expression) => _expression = expression;

    public double Parse()
    {
        if (_expression.Length == 0)
            throw new FormatException("На фото не найден арифметический пример.");

        var result = ParseExpression();
        if (_position != _expression.Length)
            throw new FormatException("Пример содержит неподдерживаемые символы.");

        Steps.Add("Соблюдаем порядок действий: скобки, умножение и деление, затем сложение и вычитание.");
        Steps.Add($"Ответ: {ArithmeticSolver.Format(result)}");
        return result;
    }

    private double ParseExpression()
    {
        var value = ParseTerm();
        while (TryRead('+') || TryRead('-'))
        {
            var operation = _expression[_position - 1];
            var right = ParseTerm();
            value = operation == '+' ? value + right : value - right;
            Steps.Add($"{operation}: получаем {ArithmeticSolver.Format(value)}");
        }
        return value;
    }

    private double ParseTerm()
    {
        var value = ParseFactor();
        while (TryRead('*') || TryRead('/'))
        {
            var operation = _expression[_position - 1];
            var right = ParseFactor();
            if (operation == '/' && right == 0)
                throw new DivideByZeroException("Деление на ноль невозможно.");

            value = operation == '*' ? value * right : value / right;
            Steps.Add($"{operation}: получаем {ArithmeticSolver.Format(value)}");
        }
        return value;
    }

    private double ParseFactor()
    {
        if (TryRead('+'))
            return ParseFactor();
        if (TryRead('-'))
            return -ParseFactor();

        if (TryRead('('))
        {
            var value = ParseExpression();
            if (!TryRead(')'))
                throw new FormatException("Проверь скобки в примере.");
            return value;
        }

        var start = _position;
        while (_position < _expression.Length &&
               (char.IsDigit(_expression[_position]) || _expression[_position] == '.'))
            _position++;

        if (start == _position ||
            !double.TryParse(_expression[start.._position], NumberStyles.Float,
                CultureInfo.InvariantCulture, out var number))
            throw new FormatException("Не удалось прочитать число.");

        return number;
    }

    private bool TryRead(char expected)
    {
        if (_position >= _expression.Length || _expression[_position] != expected)
            return false;
        _position++;
        return true;
    }
}
