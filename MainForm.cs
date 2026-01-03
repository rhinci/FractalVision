using FractalVision.Models;

namespace FractalVision
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            Button testButton = new Button();
            testButton.Text = "Тест ComplexNumber";
            testButton.Location = new System.Drawing.Point(10, 10);
            testButton.Size = new System.Drawing.Size(150, 30);
            testButton.Click += (sender, e) =>
            {
                TestComplex.RunTest();
            };

            Button testButton2 = new Button();
            testButton2.Text = "Тест FractalCalculator";
            testButton2.Location = new System.Drawing.Point(10, 50);
            testButton2.Size = new System.Drawing.Size(150, 30);
            testButton2.Click += (sender, e) =>
            {
                TestFractalCalculator();
            };

            this.Controls.Add(testButton);
            this.Controls.Add(testButton2);
        }


        private void TestFractalCalculator()
        {
            // Простой тест прямо в форме
            var calculator = new FractalCalculator();
            calculator.MaxIterations = 50;

            // Тестируем несколько точек
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
    }
}
