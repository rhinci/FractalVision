using FractalVision.Models;
using FractalVision.Views;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace FractalVision.Services
{
    public class ToolbarManager
    {
        private readonly ToolStrip _toolStrip;
        private readonly FractalViewer _fractalViewer;
        private HistoryManager? _historyManager;

        private ToolStripButton _zoomInButton = null!;
        private ToolStripButton _zoomOutButton = null!;
        private ToolStripButton _resetButton = null!;
        private ToolStripButton _renderButton = null!;
        private ToolStripButton _undoButton = null!;
        private ToolStripButton _redoButton = null!;
        private ToolStripSeparator _separator1 = null!;
        private ToolStripLabel _statusLabel = null!;

        private ToolStripButton _saveButton = null!;
        private ToolStripButton _clipboardButton = null!;
        private ImageExporter _imageExporter = null!;

        public ToolStrip ToolStrip => _toolStrip;

        public ToolbarManager(FractalViewer fractalViewer, HistoryManager? historyManager = null)
        {
            _fractalViewer = fractalViewer;
            _historyManager = historyManager;
            _toolStrip = new ToolStrip();
            _imageExporter = new ImageExporter(fractalViewer);
            InitializeToolbar();
            ConnectEvents();
        }

        private void InitializeToolbar()
        {
            var zoomInIcon = CreateIcon("+");
            var zoomOutIcon = CreateIcon("-");
            var resetIcon = CreateIcon("⌂");
            var refreshIcon = CreateIcon("↻");
            var saveIcon = CreateIcon("💾");
            var clipboardIcon = CreateIcon("📋");

            var items = new System.Collections.Generic.List<ToolStripItem>();

            if (_historyManager != null)
            {
                var undoIcon = CreateIcon("←");
                var redoIcon = CreateIcon("→");
                

                _undoButton = new ToolStripButton
                {
                    Text = "Назад",
                    Image = undoIcon,
                    DisplayStyle = ToolStripItemDisplayStyle.Image,
                    ToolTipText = "Вернуться назад",
                    Enabled = false
                };

                _redoButton = new ToolStripButton
                {
                    Text = "Вперед",
                    Image = redoIcon,
                    DisplayStyle = ToolStripItemDisplayStyle.Image,
                    ToolTipText = "Вернуться вперед",
                    Enabled = false
                };

                items.Add(_undoButton);
                items.Add(_redoButton);
                items.Add(new ToolStripSeparator());
            }

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

            _saveButton = new ToolStripButton
            {
                Text = "Сохранить",
                Image = saveIcon,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Сохранить фрактал в файл"
            };

            _clipboardButton = new ToolStripButton
            {
                Text = "В буфер",
                Image = clipboardIcon,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Копировать в буфер обмена"
            };


            _separator1 = new ToolStripSeparator();
            _statusLabel = new ToolStripLabel
            {
                Text = "Готово",
                Alignment = ToolStripItemAlignment.Right
            };

            items.Add(_zoomInButton);
            items.Add(_zoomOutButton);
            items.Add(_resetButton);
            items.Add(_renderButton);
            items.Add(_saveButton);
            items.Add(_clipboardButton);
            items.Add(_separator1);
            items.Add(_statusLabel);

            _toolStrip.Items.AddRange(items.ToArray());
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

            if (_historyManager != null)
            {
                _undoButton.Click += async (sender, e) =>
                {
                    if (_fractalViewer.IsRendering) return;
                    var prevState = _historyManager.Undo();
                    await ApplyHistoryState(prevState);
                };

                _redoButton.Click += async (sender, e) =>
                {
                    if (_fractalViewer.IsRendering) return;
                    var nextState = _historyManager.Redo();
                    await ApplyHistoryState(nextState);
                };

                _historyManager.HistoryChanged += () =>
                {
                    _undoButton.Enabled = _historyManager.CanUndo;
                    _redoButton.Enabled = _historyManager.CanRedo;
                    UpdateStatus(_historyManager.GetHistoryInfo());
                };
            }

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

            _saveButton.Click += (sender, e) => ShowSaveDialog();
            _clipboardButton.Click += (sender, e) => CopyToClipboard();

        }

        private async Task ApplyHistoryState(FractalParameters parameters)
        {
            _fractalViewer.Parameters = parameters;
            await _fractalViewer.GenerateFractalAsync();
        }

        private void SetButtonsEnabled(bool enabled)
        {
            _zoomInButton.Enabled = enabled;
            _zoomOutButton.Enabled = enabled;
            _resetButton.Enabled = enabled;
            _renderButton.Enabled = enabled;

            if (_historyManager != null)
            {
                _undoButton.Enabled = enabled && _historyManager.CanUndo;
                _redoButton.Enabled = enabled && _historyManager.CanRedo;
            }
        }

        public void UpdateStatus(string text)
        {
            _statusLabel.Text = text;
        }

        private void ShowSaveDialog()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "PNG изображение (*.png)|*.png|JPEG изображение (*.jpg;*.jpeg)|*.jpg;*.jpeg";
                dialog.FileName = _imageExporter.GetDefaultFileName();
                dialog.DefaultExt = "png";
                dialog.AddExtension = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string result;

                    switch (dialog.FilterIndex)
                    {
                        case 1:
                            result = _imageExporter.ExportToPng(dialog.FileName);
                            break;
                        case 2:
                            result = _imageExporter.ExportToJpeg(dialog.FileName, 90);
                            break;
                        default:
                            result = "Неизвестный формат";
                            break;
                    }

                    UpdateStatus(result);
                }
            }
        }

        private void CopyToClipboard()
        {
            _imageExporter.CopyToClipboard();
            UpdateStatus("Изображение скопировано в буфер обмена");
        }
    }
}