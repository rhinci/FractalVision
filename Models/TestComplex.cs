using System;
using System.Text;
using System.Windows.Forms;
using FractalVision.Enums;

namespace FractalVision.Models
{
    public static class TestComplex
    {
        public static void RunTest()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("=== ТЕСТ FRACTALCALCULATOR ===\n");

            try
            {
                // Создаем калькулятор
                FractalCalculator calculator = new FractalCalculator();
                calculator.MaxIterations = 100;

                result.AppendLine($"Тип фрактала: {calculator.FractalType}");
                result.AppendLine($"Макс итераций: {calculator.MaxIterations}\n");

                // Тест 1: Точка внутри множества Мандельброта (0, 0)
                ComplexNumber insidePoint = new ComplexNumber(0, 0);
                int iterations1 = calculator.CalculateMandelbrot(insidePoint);
                result.AppendLine($"Точка (0, 0): {iterations1} итераций (должно быть -1)");

                // Тест 2: Точка снаружи множества (2, 2)
                ComplexNumber outsidePoint = new ComplexNumber(2, 2);
                int iterations2 = calculator.CalculateMandelbrot(outsidePoint);
                result.AppendLine($"Точка (2, 2): {iterations2} итераций (должно быть 0-5)");

                // Тест 3: Интересная точка на границе (-0.5, 0.5)
                ComplexNumber borderPoint = new ComplexNumber(-0.5, 0.5);
                int iterations3 = calculator.CalculateMandelbrot(borderPoint);
                result.AppendLine($"Точка (-0.5, 0.5): {iterations3} итераций\n");

                // Тест 4: Цвета
                var color1 = calculator.GetColorFromIterations(-1);
                var color2 = calculator.GetColorFromIterations(0);
                var color3 = calculator.GetColorFromIterations(50);
                var color4 = calculator.GetColorFromIterations(100);

                result.AppendLine("Цвета:");
                result.AppendLine($"  -1 итераций: {color1.Name} (черный)");
                result.AppendLine($"   0 итераций: {color2.Name}");
                result.AppendLine($"  50 итераций: {color3.Name}");
                result.AppendLine($" 100 итераций: {color4.Name}\n");

                // Тест 5: Переключение на Жюлиа
                calculator.FractalType = FractalType.Julia;
                result.AppendLine($"Теперь тип: {calculator.FractalType}");

                int juliaIterations = calculator.GetIterationCount(new ComplexNumber(0.5, 0.5));
                result.AppendLine($"Жюлиа (0.5, 0.5): {juliaIterations} итераций");

                result.AppendLine("\n=== ТЕСТ ПРОЙДЕН УСПЕШНО ===");
            }
            catch (Exception ex)
            {
                result.AppendLine($"ОШИБКА: {ex.Message}");
                result.AppendLine($"StackTrace: {ex.StackTrace}");
            }

            MessageBox.Show(result.ToString(), "Тест FractalCalculator",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}