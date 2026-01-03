using System.Windows.Forms;
using FractalVision.Views;
using FractalVision.Services;

namespace FractalVision
{
    public partial class MainForm : Form
    {
        private FractalViewer _fractalViewer = null!;
        private HistoryManager _historyManager = null!;
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

            _historyManager = new HistoryManager(_fractalViewer.Parameters);

            _parametersPanel = new ParametersPanel(_fractalViewer);

            _toolbarManager = new ToolbarManager(_fractalViewer, _historyManager);
            _toolbarManager.ToolStrip.Dock = DockStyle.Top;

            _fractalViewer.RenderingFinished += () =>
            {
                _historyManager.AddState(_fractalViewer.Parameters);
            };

            var viewerContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 30, 5, 5)
            };
            viewerContainer.Controls.Add(_fractalViewer);

            this.Controls.Add(_parametersPanel); 
            this.Controls.Add(viewerContainer); 
            this.Controls.Add(_toolbarManager.ToolStrip);

            _toolbarManager.ToolStrip.BringToFront();

            _ = _fractalViewer.GenerateFractalAsync();
        }
    }
}