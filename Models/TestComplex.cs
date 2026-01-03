using System;
using System.Windows.Forms;

namespace FractalVision.Models
{
    public static class TestComplex
    {
        public static void RunTest()
        {
            string result = "=== ТЕСТ КОМПЛЕКСНЫХ ЧИСЕЛ ===\n\n";

            // Тест 1: Создание чисел
            ComplexNumber z1 = new ComplexNumber(2, 3); // 2 + 3i
            ComplexNumber z2 = new ComplexNumber(1, -1); // 1 - 1i

            result += $"z1 = {z1}\n";
            result += $"z2 = {z2}\n";
            result += $"Модуль z1 = {z1.Magnitude:F2}\n\n";

            // Тест 2: Сложение
            ComplexNumber sum = z1.Add(z2); // (2+1) + (3-1)i = 3 + 2i
            result += $"z1 + z2 = {sum}\n";

            // Тест 3: Умножение
            ComplexNumber product = z1.Multiply(z2);
            result += $"z1 * z2 = {product}\n";

            // Тест 4: Квадрат (особенно важно для фракталов!)
            ComplexNumber square = z1.Square(); // (2+3i)² = -5 + 12i
            result += $"z1² = {square}\n\n";

            // Тест 5: Формула фрактала z = z² + c
            ComplexNumber z = new ComplexNumber(0, 0); // Начальное z = 0
            ComplexNumber c = new ComplexNumber(0.5, 0.5); // Константа c

            result += "Тест итерации фрактала:\n";
            result += $"Начальное z = {z}\n";
            result += $"Константа c = {c}\n";

            // Делаем одну итерацию: z = z² + c
            z = z.Square().Add(c);
            result += $"После z = z² + c: {z}\n";

            MessageBox.Show(result, "Тест ComplexNumber",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}