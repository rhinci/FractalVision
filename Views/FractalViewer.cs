using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using FractalVision.Enums;
using FractalVision.Models;
using ColorPaletteService = FractalVision.Services.ColorPalette;

namespace FractalVision.Views
{
    public class FractalViewer : Panel
    {
        private Bitmap _fractalBitmap;
        private FractalCalculator _calculator;
        private ColorPaletteService _palette;
        private FractalParameters _parameters;
        private bool _isRendering;
        private RenderQuality _renderQuality;

        public event Action RenderingStarted = delegate { };
        public event Action RenderingFinished = delegate { };
        public event Action<ComplexNumber> MouseCoordinatesChanged = delegate { };

        public FractalViewer()
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            this.DoubleBuffered = true;

            _calculator = new FractalCalculator();
            _palette = new ColorPaletteService(ColorScheme.Rainbow);
            _parameters = new FractalParameters(0, 0, 1.0, 800, 600);
            _renderQuality = RenderQuality.Standard;
            _isRendering = false;

            _fractalBitmap = new Bitmap(_parameters.Width, _parameters.Height);

            this.MouseClick += FractalViewer_MouseClick;
            this.MouseMove += FractalViewer_MouseMove;
            this.Size = new Size(_parameters.Width, _parameters.Height);
        }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public FractalParameters Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                UpdateSize();
            }
        }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public FractalType FractalType
        {
            get { return _calculator.FractalType; }
            set { _calculator.FractalType = value; }
        }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int MaxIterations
        {
            get { return _calculator.MaxIterations; }
            set { _calculator.MaxIterations = value; }
        }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public ColorPaletteService Palette
        {
            get { return _palette; }
            set { _palette = value; }
        }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public RenderQuality RenderQuality
        {
            get { return _renderQuality; }
            set { _renderQuality = value; }
        }

        public bool IsRendering => _isRendering;
        public Bitmap FractalImage => _fractalBitmap;

        public async Task GenerateFractalAsync()
        {
            if (_isRendering) return;

            _isRendering = true;
            RenderingStarted();

            try
            {
                int renderWidth = _parameters.Width;
                int renderHeight = _parameters.Height;

                if (_renderQuality == RenderQuality.Preview)
                {
                    renderWidth = Math.Min(400, renderWidth);
                    renderHeight = Math.Min(300, renderHeight);
                }

                var newBitmap = new Bitmap(renderWidth, renderHeight);

                await Task.Run(() => GenerateFractalBitmap(newBitmap));

                _fractalBitmap?.Dispose();
                _fractalBitmap = newBitmap;

                if (_renderQuality != RenderQuality.Preview)
                {
                    this.Size = new Size(renderWidth, renderHeight);
                    _parameters.Width = renderWidth;
                    _parameters.Height = renderHeight;
                }

                this.Invalidate();
            }
            finally
            {
                _isRendering = false;
                RenderingFinished();
            }
        }

        private void GenerateFractalBitmap(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int maxIterations = _calculator.MaxIterations;

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    ComplexNumber c = _parameters.PixelToComplex(
                        (int)(x * ((double)_parameters.Width / width)),
                        (int)(y * ((double)_parameters.Height / height))
                    );

                    int iterations = _calculator.GetIterationCount(c);
                    Color color = _palette.GetColor(iterations, maxIterations);
                    bitmap.SetPixel(x, y, color);
                }
            });
        }

        public async Task ZoomToPointAsync(int pixelX, int pixelY, double zoomFactor = 2.0)
        {
            if (_isRendering) return;

            _parameters.ZoomToPoint(pixelX, pixelY, zoomFactor);
            await GenerateFractalAsync();
        }

        public async Task ResetViewAsync()
        {
            if (_isRendering) return;

            _parameters.Reset();
            await GenerateFractalAsync();
        }

        private void UpdateSize()
        {
            if (!_isRendering && _renderQuality != RenderQuality.Preview)
            {
                this.Size = new Size(_parameters.Width, _parameters.Height);
            }
        }

        private void FractalViewer_MouseClick(object? sender, MouseEventArgs e)
        {
            if (_isRendering) return;

            if (e.Button == MouseButtons.Left)
            {
                _ = ZoomToPointAsync(e.X, e.Y, 2.0);
            }
            else if (e.Button == MouseButtons.Right)
            {
                _ = ZoomToPointAsync(e.X, e.Y, 0.5);
            }
        }

        private void FractalViewer_MouseMove(object? sender, MouseEventArgs e)
        {
            ComplexNumber complexCoord = _parameters.PixelToComplex(e.X, e.Y);
            MouseCoordinatesChanged(complexCoord);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_fractalBitmap != null)
            {
                e.Graphics.DrawImage(_fractalBitmap, 0, 0, this.Width, this.Height);

                if (_isRendering)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                    {
                        e.Graphics.FillRectangle(brush, 0, 0, this.Width, this.Height);
                    }

                    using (var font = new Font("Arial", 12))
                    using (var brush = new SolidBrush(Color.White))
                    {
                        string text = "Рендеринг...";
                        var size = e.Graphics.MeasureString(text, font);
                        e.Graphics.DrawString(text, font, brush,
                            (this.Width - size.Width) / 2,
                            (this.Height - size.Height) / 2);
                    }
                }
            }
            else
            {
                using (var brush = new SolidBrush(Color.Black))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, this.Width, this.Height);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fractalBitmap?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}