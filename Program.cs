            using System;
using System.Globalization;
using System.Text;

namespace LinearInequalitiesApp
{
    /// <summary>
    /// Базовий клас для систем нерівностей
    /// </summary>
    public class InequalitiesSystem
    {
        protected readonly double[,] _coefficients;
        protected readonly double[] _constants;

        public int InequalitiesCount { get; }
        public int VariablesCount { get; }

        public InequalitiesSystem(int inequalitiesCount, int variablesCount)
        {
            if (inequalitiesCount <= 0 || variablesCount <= 0)
                throw new ArgumentException("Кількість нерівностей і змінних має бути більшою за 0.");

            InequalitiesCount = inequalitiesCount;
            VariablesCount = variablesCount;
            _coefficients = new double[inequalitiesCount, variablesCount];
            _constants = new double[inequalitiesCount];
        }

        protected static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) &&
                    double.IsFinite(value))
                    return value;
                Console.WriteLine("❌ Некоректне число, спробуйте ще раз.");
            }
        }

        public virtual void InputCoefficients()
        {
            Console.WriteLine($"\nВведіть коефіцієнти для системи з {InequalitiesCount} нерівностей " +
                              $"та {VariablesCount} змінних:");
            for (int i = 0; i < InequalitiesCount; i++)
            {
                Console.WriteLine($"\nНерівність {i + 1}:");
                for (int j = 0; j < VariablesCount; j++)
                    _coefficients[i, j] = ReadDouble($"  Введіть a{i + 1}{j + 1}: ");

                _constants[i] = ReadDouble($"  Введіть b{i + 1}: ");
            }
        }

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
    /// Похідний клас для спеціальної системи, наприклад, з додатковою перевіркою
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
            Console.WriteLine("Перевіряємо вектор у спеціальній системі...");
            return base.CheckVector(variables);
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Демо віртуальних методів ===");
            Console.WriteLine("Оберіть режим роботи:");
            Console.WriteLine("1 — звичайна система");
            Console.WriteLine("2 — спеціальна система");

            string? userChoose = Console.ReadLine();

            InequalitiesSystem system;

            // Динамічне створення об’єкта залежно від вибору
            if (userChoose == "1")
                system = new InequalitiesSystem(2, 2);
            else
                system = new SpecialInequalitiesSystem(2, 2);

            // Виклик віртуальних методів через базовий клас
            system.InputCoefficients();
            system.PrintSystem();

            Console.WriteLine("\nВведіть 2 змінні для перевірки:");
            double x1 = InequalitiesSystem.ReadDouble("x1 = ");
            double x2 = InequalitiesSystem.ReadDouble("x2 = ");

            bool result = system.CheckVector(x1, x2);
            Console.WriteLine(result ? "✅ Вектор задовольняє систему" : "❌ Вектор не задовольняє систему");

            // Перевірка заміни віртуальних методів на звичайні
            Console.WriteLine("\nЯкщо методи зробити НЕ віртуальними, то перевизначення у похідному класі НЕ спрацює!");
        }
    }
}
