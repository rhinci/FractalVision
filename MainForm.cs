using FractalVision.Enums;
using FractalVision.Models;
using FractalVision.Services;
using System.Text;

namespace FractalVision
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            int y = 10;
            int buttonHeight = 30;
            int spacing = 5;

            Button[] buttons = new Button[]
            {
        CreateTestButton("Тест ComplexNumber", y),
        CreateTestButton("Тест FractalCalculator", y += buttonHeight + spacing),
        CreateTestButton("Тест ColorPalette", y += buttonHeight + spacing),
        CreateTestButton("Calculator+Palette", y += buttonHeight + spacing)
            };

            buttons[0].Click += (s, e) => TestComplex.RunTest();
            buttons[1].Click += (s, e) => TestFractalCalculator();
            buttons[2].Click += (s, e) => TestPalette.RunTest();
            buttons[3].Click += (s, e) => TestCalculatorWithPalette();

            foreach (var button in buttons)
                this.Controls.Add(button);

            this.ClientSize = new System.Drawing.Size(400, 180);
            this.Text = "FractalVision - Тесты";
        }

        private Button CreateTestButton(string text, int y)
        {
            return new Button
            {
                Text = text,
                Location = new System.Drawing.Point(10, y),
                Size = new System.Drawing.Size(180, 30)
            };
        }


        private void TestFractalCalculator()
        {
            var calculator = new FractalCalculator();
            calculator.MaxIterations = 50;

            string result = "Быстрый тест FractalCalculator:\n\n";

            var points = new[]
            {
        new { Name = "Центр (0,0)", Point = new ComplexNumber(0, 0) },
        new { Name = "Справа (1,0)", Point = new ComplexNumber(1, 0) },
        new { Name = "Спираль (-0.5,0.5)", Point = new ComplexNumber(-0.5, 0.5) },
        new { Name = "Далеко (2,2)", Point = new ComplexNumber(2, 2) }
    };

            foreach (var p in points)
            {
                int iterations = calculator.CalculateMandelbrot(p.Point);
                string status = iterations == -1 ? "ВНУТРИ множества" : $"убежала за {iterations} итераций";
                result += $"{p.Name}: {status}\n";
            }

            MessageBox.Show(result, "Быстрый тест",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TestCalculatorWithPalette()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("=== ИНТЕГРАЦИЯ CALCULATOR + PALETTE ===\n");

            var calculator = new FractalCalculator();
            var palette = new ColorPalette(ColorScheme.Fire);

            calculator.MaxIterations = 100;

            var testPoints = new[]
            {
        new ComplexNumber(0, 0),
        new ComplexNumber(1, 0),
        new ComplexNumber(2, 2),
        new ComplexNumber(-0.5, 0.5) 
    };

            result.AppendLine($"Палитра: {palette.PaletteType}");
            result.AppendLine($"Макс итераций: {calculator.MaxIterations}\n");

            for (int i = 0; i < testPoints.Length; i++)
            {
                int iterations = calculator.CalculateMandelbrot(testPoints[i]);
                Color color = palette.GetColor(iterations, calculator.MaxIterations);

                result.AppendLine($"Точка {i + 1} ({testPoints[i]}):");
                result.AppendLine($"  Итерации: {iterations}");
                result.AppendLine($"  Цвет: {color.Name} (R={color.R}, G={color.G}, B={color.B})");
                result.AppendLine();
            }

            palette.PaletteType = ColorScheme.Ocean;
            result.AppendLine($"Сменили на Ocean: {palette.GetPaletteInfo()}");

            MessageBox.Show(result.ToString(), "Интеграционный тест",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
