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

            string[] buttonTexts = {
        "Тест ComplexNumber",
        "Тест FractalCalculator",
        "Тест ColorPalette",
        "Calculator+Palette",
        "Тест FractalParameters"
    };

            for (int i = 0; i < buttonTexts.Length; i++)
            {
                Button button = CreateTestButton(buttonTexts[i], y + i * (buttonHeight + spacing));

                switch (i)
                {
                    case 0: button.Click += (s, e) => TestComplex.RunTest(); break;
                    case 1: button.Click += (s, e) => TestFractalCalculator(); break;
                    case 2: button.Click += (s, e) => TestPalette.RunTest(); break;
                    case 3: button.Click += (s, e) => TestCalculatorWithPalette(); break;
                    case 4: button.Click += (s, e) => TestFractalParameters(); break;
                }

                this.Controls.Add(button);
            }

            this.ClientSize = new System.Drawing.Size(400, 220);
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
                result.AppendLine($"Итерации: {iterations}");
                result.AppendLine($"Цвет: {color.Name} (R={color.R}, G={color.G}, B={color.B})");
                result.AppendLine();
            }

            palette.PaletteType = ColorScheme.Ocean;
            result.AppendLine($"Сменили на Ocean: {palette.GetPaletteInfo()}");

            MessageBox.Show(result.ToString(), "Интеграционный тест",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void TestFractalParameters()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("=== ТЕСТ FRACTALPARAMETERS ===\n");

            // Создаем параметры
            var parameters = new FractalParameters(0, 0, 1.0, 800, 600);

            result.AppendLine($"1. Параметры по умолчанию:");
            result.AppendLine($"{parameters}");
            result.AppendLine($"Соотношение сторон: {parameters.AspectRatio:F2}\n");

            // Тест преобразования координат
            result.AppendLine($"2. Преобразование координат (центр экрана):");
            ComplexNumber center = parameters.PixelToComplex(400, 300);
            result.AppendLine($"Пиксель (400,300) - {center}");
            result.AppendLine($"Ожидается около (0,0)\n");

            result.AppendLine($"3. Преобразование координат (левый верхний угол):");
            ComplexNumber topLeft = parameters.PixelToComplex(0, 0);
            result.AppendLine($"Пиксель (0,0) - {topLeft}");
            result.AppendLine($"Ожидается около (-2, 1.5)\n");

            result.AppendLine($"4. Преобразование координат (правый нижний угол):");
            ComplexNumber bottomRight = parameters.PixelToComplex(799, 599);
            result.AppendLine($"Пиксель (799,599) - {bottomRight}");
            result.AppendLine($"Ожидается около (2, -1.5)\n");

            // Тест зума
            result.AppendLine($"5. Тест зума (приближаем в 2 раза):");
            var zoomedParams = parameters.Clone();
            zoomedParams.Zoom = 2.0;
            result.AppendLine($"До зума: {parameters.PixelToComplex(400, 300)}");
            result.AppendLine($"После зума: {zoomedParams.PixelToComplex(400, 300)}");
            result.AppendLine($"Должен быть тот же центр (0,0)\n");

            // Тест перемещения
            result.AppendLine($"6. Тест перемещения (сдвигаем вправо-вверх):");
            var movedParams = parameters.Clone();
            movedParams.Pan(0.5, 0.5);
            result.AppendLine($"После Pan(0.5, 0.5): {movedParams}");
            result.AppendLine($"Центр: ({movedParams.CenterX:F4}, {movedParams.CenterY:F4})");

            MessageBox.Show(result.ToString(), "Тест FractalParameters",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
