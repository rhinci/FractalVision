using System.Windows.Forms;
using FractalVision.Views;
using System.Drawing;

namespace FractalVision.Services
{
    public class ToolbarManager
    {
        private readonly ToolStrip _toolStrip;
        private readonly FractalViewer _fractalViewer;

        private ToolStripButton _zoomInButton = null!;
        private ToolStripButton _zoomOutButton = null!;
        private ToolStripButton _resetButton = null!;
        private ToolStripButton _renderButton = null!;
        private ToolStripSeparator _separator1 = null!;
        private ToolStripLabel _statusLabel = null!;

        public ToolStrip ToolStrip => _toolStrip;

        public ToolbarManager(FractalViewer fractalViewer)
        {
            _fractalViewer = fractalViewer;
            _toolStrip = new ToolStrip();
            InitializeToolbar();
            ConnectEvents();
        }

        private void InitializeToolbar()
        {
            // иконки из стандартных иконок Windows
            var zoomInIcon = CreateIcon("+");
            var zoomOutIcon = CreateIcon("-");
            var resetIcon = CreateIcon("⌂");
            var refreshIcon = CreateIcon("↻");

            _zoomInButton = new ToolStripButton
            {
                Text = "Увеличить",
                Image = zoomInIcon,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Увеличить область (ЛКМ по фракталу)"
            };

            _zoomOutButton = new ToolStripButton
            {
                Text = "Уменьшить",
                Image = zoomOutIcon,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Уменьшить область (ПКМ по фракталу)"
            };

            _resetButton = new ToolStripButton
            {
                Text = "Сброс",
                Image = resetIcon,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Вернуться к начальному виду"
            };

            _renderButton = new ToolStripButton
            {
                Text = "Перерисовать",
                Image = refreshIcon,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Перерисовать фрактал"
            };

            _separator1 = new ToolStripSeparator();

            _statusLabel = new ToolStripLabel
            {
                Text = "Готово",
                Alignment = ToolStripItemAlignment.Right
            };

            _toolStrip.Items.AddRange(new ToolStripItem[]
            {
                _zoomInButton,
                _zoomOutButton,
                _resetButton,
                _renderButton,
                _separator1,
                _statusLabel
            });
        }

        private Bitmap CreateIcon(string text)
        {
            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font("Arial", 10, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.Clear(Color.Transparent);
                var size = g.MeasureString(text, font);
                g.DrawString(text, font, brush,
                    (bmp.Width - size.Width) / 2,
                    (bmp.Height - size.Height) / 2);
            }
            return bmp;
        }

        private void ConnectEvents()
        {

            _zoomInButton.Click += async (sender, e) =>
            {
                if (_fractalViewer.IsRendering) return;
                await _fractalViewer.ZoomToPointAsync(
                    _fractalViewer.Width / 2,
                    _fractalViewer.Height / 2,
                    2.0
                );
            };

            _zoomOutButton.Click += async (sender, e) =>
            {
                if (_fractalViewer.IsRendering) return;
                await _fractalViewer.ZoomToPointAsync(
                    _fractalViewer.Width / 2,
                    _fractalViewer.Height / 2,
                    0.5
                );
            };

            _resetButton.Click += async (sender, e) =>
            {
                if (_fractalViewer.IsRendering) return;
                await _fractalViewer.ResetViewAsync();
            };

            _renderButton.Click += async (sender, e) =>
            {
                if (_fractalViewer.IsRendering) return;
                await _fractalViewer.GenerateFractalAsync();
            };

            // Обработчики состояния рендеринга
            _fractalViewer.RenderingStarted += () =>
            {
                _statusLabel.Text = "Рендеринг...";
                SetButtonsEnabled(false);
            };

            _fractalViewer.RenderingFinished += () =>
            {
                _statusLabel.Text = "Готово";
                SetButtonsEnabled(true);
            };
        }

        private void SetButtonsEnabled(bool enabled)
        {
            _zoomInButton.Enabled = enabled;
            _zoomOutButton.Enabled = enabled;
            _resetButton.Enabled = enabled;
            _renderButton.Enabled = enabled;
        }

        public void UpdateStatus(string text)
        {
            _statusLabel.Text = text;
        }
    }
}