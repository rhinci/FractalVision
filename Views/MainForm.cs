using FractalVision.Controls;
using FractalVision.Services;
using FractalVision.Views;
using System.Windows.Forms;

namespace FractalVision
{
    public partial class MainForm : Form
    {
        private FractalViewer _fractalViewer = null!;
        private ToolbarManager _toolbarManager = null!;
        private ParametersPanel _parametersPanel = null!;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            _fractalViewer = new FractalViewer();
            _fractalViewer.Dock = DockStyle.Fill;

            _parametersPanel = new ParametersPanel(_fractalViewer);

            _toolbarManager = new ToolbarManager(_fractalViewer);
            _toolbarManager.ToolStrip.Dock = DockStyle.Top;

            var viewerContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };
            viewerContainer.Controls.Add(_fractalViewer);

            this.Controls.Add(_parametersPanel);
            this.Controls.Add(viewerContainer);
            this.Controls.Add(_toolbarManager.ToolStrip);

            _ = _fractalViewer.GenerateFractalAsync();
        }
    }
}