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

            this.Controls.Add(testButton);
        }
    }
}
