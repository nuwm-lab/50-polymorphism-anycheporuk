using System;
using System.Globalization;
using System.Text;

namespace LinearInequalitiesApp
{
    /// <summary>
    /// Допоміжний клас для загальних статичних методів
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Зчитує дійсне число з консолі з перевіркою.
        /// </summary>
        public static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                    && double.IsFinite(value))
                    return value;
                Console.WriteLine("❌ Некоректне число. Використовуйте крапку для дробових значень (наприклад: 1.5).");
            }
        }
    }

    /// <summary>
    /// Базовий клас для систем нерівностей
    /// </summary>
    public class InequalitiesSystem
    {
        private readonly double[,] _coefficients;
        private readonly double[] _constants;

        public int InequalitiesCount { get; }
        public int VariablesCount { get; }

        /// <summary>
        /// Ініціалізує систему нерівностей.
        /// </summary>
        public InequalitiesSystem(int inequalitiesCount, int variablesCount)
        {
            if (inequalitiesCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(inequalitiesCount), "Кількість нерівностей має бути більшою за 0.");
            if (variablesCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(variablesCount), "Кількість змінних має бути більшою за 0.");

            InequalitiesCount = inequalitiesCount;
            VariablesCount = variablesCount;
            _coefficients = new double[inequalitiesCount, variablesCount];
            _constants = new double[inequalitiesCount];
        }

        /// <summary>
        /// Зчитує коефіцієнти системи.
        /// </summary>
        public virtual void InputCoefficients()
        {
            Console.WriteLine($"\nВведіть коефіцієнти для системи з {InequalitiesCount} нерівностей " +
                              $"та {VariablesCount} змінних:");

            for (int i = 0; i < InequalitiesCount; i++)
            {
                Console.WriteLine($"\nНерівність {i + 1}:");
                for (int j = 0; j < VariablesCount; j++)
                    _coefficients[i, j] = Helper.ReadDouble($"  Введіть a{i + 1}{j + 1}: ");

                _constants[i] = Helper.ReadDouble($"  Введіть b{i + 1}: ");
            }
        }

        /// <summary>
        /// Виводить систему нерівностей.
        /// </summary>
        public virtual void PrintSystem()
        {
            Console.WriteLine("\nСистема лінійних нерівностей має вигляд:");
            for (int i = 0; i < InequalitiesCount; i++)
            {
                var sb = new StringBuilder();
                for (int j = 0; j < VariablesCount; j++)
                {
                    double coeff = _coefficients[i, j];
                    string sign = coeff >= 0 && j > 0 ? " + " : (j > 0 ? " - " : coeff < 0 ? "-" : "");
                    sb.Append($"{sign}{Math.Abs(coeff).ToString(CultureInfo.InvariantCulture)}*x{j + 1}");
                }
                sb.Append($" ≤ {_constants[i].ToString(CultureInfo.InvariantCulture)}");
                Console.WriteLine(sb);
            }
        }

        /// <summary>
        /// Перевіряє, чи задовольняє вектор систему.
        /// </summary>
        public virtual bool CheckVector(params double[] variables)
        {
            if (variables.Length != VariablesCount)
                throw new ArgumentException("Кількість змінних не збігається з кількістю змінних у системі.");

            for (int i = 0; i < InequalitiesCount; i++)
            {
                double sum = 0;
                for (int j = 0; j < VariablesCount; j++)
                    sum += _coefficients[i, j] * variables[j];

                if (sum > _constants[i])
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Похідний клас для демонстрації поліморфізму
    /// </summary>
    public class SpecialInequalitiesSystem : InequalitiesSystem
    {
        public SpecialInequalitiesSystem(int inequalitiesCount, int variablesCount)
            : base(inequalitiesCount, variablesCount) { }

        public override void PrintSystem()
        {
            Console.WriteLine("\n--- Спеціальна система нерівностей ---");
            base.PrintSystem();
        }

        public override bool CheckVector(params double[] variables)
        {
            Console.WriteLine("Перевірка вектора у спеціальній системі...");
            return base.CheckVector(variables);
        }

        /// <summary>
        /// Демонстрація різниці override vs new
        /// </summary>
        public new void DemoNewMethod()
        {
            Console.WriteLine("Цей метод приховано (new), викликається лише через SpecialInequalitiesSystem");
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Демонстрація віртуальних методів та поліморфізму ===");
            Console.WriteLine("Оберіть режим роботи:");
            Console.WriteLine("1 — звичайна система");
            Console.WriteLine("2 — спеціальна система");

            string? userChoose = Console.ReadLine();

            // Поліморфізм: змінна типу базового класу
            InequalitiesSystem system = userChoose == "1"
                ? new InequalitiesSystem(2, 2)
                : new SpecialInequalitiesSystem(2, 2);

            system.InputCoefficients();
            system.PrintSystem();

            Console.WriteLine("\nВведіть 2 змінні для перевірки:");
            double x1 = Helper.ReadDouble("x1 = ");
            double x2 = Helper.ReadDouble("x2 = ");

            bool result = system.CheckVector(x1, x2);
            Console.WriteLine(result ? "✅ Вектор задовольняє систему" : "❌ Вектор не задовольняє систему");

            // Тест нового vs override
            if (system is SpecialInequalitiesSystem special)
            {
                special.DemoNewMethod(); // викликається через змінну похідного класу
                system.PrintSystem();    // override викликається через базовий клас
            }
        }
    }
}
