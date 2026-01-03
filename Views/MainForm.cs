using System.Windows.Forms;
using FractalVision.Services;
using FractalVision.Views;

namespace FractalVision
{
    public partial class MainForm : Form
    {
        private FractalViewer _fractalViewer = null!;
        private ToolbarManager _toolbarManager = null!;

        public MainForm()
        {
            InitializeComponent();  // Этот метод в Designer.cs
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Создаем FractalViewer
            _fractalViewer = new FractalViewer();
            _fractalViewer.Dock = DockStyle.Fill;

            // Создаем ToolbarManager
            _toolbarManager = new ToolbarManager(_fractalViewer);
            _toolbarManager.ToolStrip.Dock = DockStyle.Top;

            // Добавляем элементы на форму
            this.Controls.Add(_fractalViewer);
            this.Controls.Add(_toolbarManager.ToolStrip);

            // Генерируем первый фрактал
            _ = _fractalViewer.GenerateFractalAsync();
        }
    }
}