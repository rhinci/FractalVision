using System;
using System.Drawing;
using System.Windows.Forms;
using FractalVision.Enums;
using FractalVision.Models;
using FractalVision.Views;

namespace FractalVision.Controls
{
    public class ParametersPanel : Panel
    {
        private GroupBox _mainGroup;
        private ComboBox _fractalTypeCombo;
        private NumericUpDown _iterationsNumeric;
        private NumericUpDown _widthNumeric;
        private NumericUpDown _heightNumeric;
        private ComboBox _paletteCombo;
        private ComboBox _qualityCombo;
        private Label _coordinatesLabel;
        private Button _applyButton;
        private Button _resetParamsButton;

        private FractalViewer _fractalViewer;

        public ParametersPanel(FractalViewer fractalViewer)
        {
            _fractalViewer = fractalViewer;
            InitializeComponents();
            SetupEvents();
            UpdateCoordinates(0, 0);
        }

        private void InitializeComponents()
        {
            this.BackColor = SystemColors.Control;
            this.Dock = DockStyle.Right;
            this.Width = 300;
            this.BorderStyle = BorderStyle.FixedSingle;

            _mainGroup = new GroupBox
            {
                Text = "Параметры фрактала",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var fractalTypeLabel = new Label
            {
                Text = "Тип фрактала:",
                Location = new Point(10, 30),
                Width = 120
            };

            _fractalTypeCombo = new ComboBox
            {
                Location = new Point(130, 27),
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _fractalTypeCombo.Items.AddRange(new[] { "Мандельброт", "Жюлиа" });
            _fractalTypeCombo.SelectedIndex = 0;

            var iterationsLabel = new Label
            {
                Text = "Итерации:",
                Location = new Point(10, 65),
                Width = 120
            };

            _iterationsNumeric = new NumericUpDown
            {
                Location = new Point(130, 62),
                Width = 140,
                Minimum = 10,
                Maximum = 5000,
                Value = 100,
                Increment = 50
            };

            var widthLabel = new Label
            {
                Text = "Ширина:",
                Location = new Point(10, 100),
                Width = 120
            };

            _widthNumeric = new NumericUpDown
            {
                Location = new Point(130, 97),
                Width = 140,
                Minimum = 100,
                Maximum = 3840,
                Value = 800,
                Increment = 100
            };

            var heightLabel = new Label
            {
                Text = "Высота:",
                Location = new Point(10, 135),
                Width = 120
            };

            _heightNumeric = new NumericUpDown
            {
                Location = new Point(130, 132),
                Width = 140,
                Minimum = 100,
                Maximum = 2160,
                Value = 600,
                Increment = 100
            };

            var paletteLabel = new Label
            {
                Text = "Цветовая палитра:",
                Location = new Point(10, 170),
                Width = 120
            };

            _paletteCombo = new ComboBox
            {
                Location = new Point(130, 167),
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _paletteCombo.Items.AddRange(new[] { "Радуга", "Огонь", "Океан", "Лес", "Черно-белая" });
            _paletteCombo.SelectedIndex = 0;

            var qualityLabel = new Label
            {
                Text = "Качество:",
                Location = new Point(10, 205),
                Width = 120
            };

            _qualityCombo = new ComboBox
            {
                Location = new Point(130, 202),
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _qualityCombo.Items.AddRange(new[] { "Предпросмотр", "Стандарт", "Высокое" });
            _qualityCombo.SelectedIndex = 1;

            var coordsGroup = new GroupBox
            {
                Text = "Координаты",
                Location = new Point(10, 240),
                Size = new Size(260, 60)
            };

            _coordinatesLabel = new Label
            {
                Location = new Point(10, 20),
                Size = new Size(240, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Consolas", 9)
            };
            coordsGroup.Controls.Add(_coordinatesLabel);

            _applyButton = new Button
            {
                Text = "Применить",
                Location = new Point(10, 310),
                Size = new Size(125, 35),
                BackColor = Color.LightGreen
            };

            _resetParamsButton = new Button
            {
                Text = "Сбросить",
                Location = new Point(145, 310),
                Size = new Size(125, 35),
                BackColor = Color.LightCoral
            };

            _mainGroup.Controls.AddRange(new Control[]
            {
                fractalTypeLabel, _fractalTypeCombo,
                iterationsLabel, _iterationsNumeric,
                widthLabel, _widthNumeric,
                heightLabel, _heightNumeric,
                paletteLabel, _paletteCombo,
                qualityLabel, _qualityCombo,
                coordsGroup,
                _applyButton, _resetParamsButton
            });

            this.Controls.Add(_mainGroup);
        }

        private void SetupEvents()
        {

            _applyButton.Click += async (sender, e) =>
            {
                await ApplyParameters();
            };

            _resetParamsButton.Click += (sender, e) =>
            {
                ResetToDefaults();
            };

            _fractalViewer.MouseCoordinatesChanged += coordinates =>
            {
                UpdateCoordinates(coordinates.Real, coordinates.Imaginary);
            };
        }

        private async Task ApplyParameters()
        {
            _fractalViewer.FractalType = _fractalTypeCombo.SelectedIndex == 0
                ? FractalType.Mandelbrot
                : FractalType.Julia;

            _fractalViewer.MaxIterations = (int)_iterationsNumeric.Value;

            _fractalViewer.Parameters.Width = (int)_widthNumeric.Value;
            _fractalViewer.Parameters.Height = (int)_heightNumeric.Value;

            var palette = new Services.ColorPalette((ColorScheme)_paletteCombo.SelectedIndex);
            _fractalViewer.Palette = palette;

            _fractalViewer.RenderQuality = (RenderQuality)_qualityCombo.SelectedIndex;

            await _fractalViewer.GenerateFractalAsync();
        }

        private void ResetToDefaults()
        {
            _fractalTypeCombo.SelectedIndex = 0;
            _iterationsNumeric.Value = 100;
            _widthNumeric.Value = 800;
            _heightNumeric.Value = 600;
            _paletteCombo.SelectedIndex = 0;
            _qualityCombo.SelectedIndex = 1;

            _ = _fractalViewer.ResetViewAsync();
        }

        public void UpdateCoordinates(double real, double imaginary)
        {
            _coordinatesLabel.Text = $"Re: {real:F6}\nIm: {imaginary:F6}";
        }

        public void UpdateFromViewer()
        {
            _fractalTypeCombo.SelectedIndex = _fractalViewer.FractalType == FractalType.Mandelbrot ? 0 : 1;
            _iterationsNumeric.Value = _fractalViewer.MaxIterations;
            _widthNumeric.Value = _fractalViewer.Parameters.Width;
            _heightNumeric.Value = _fractalViewer.Parameters.Height;
            _paletteCombo.SelectedIndex = (int)_fractalViewer.Palette.PaletteType;
            _qualityCombo.SelectedIndex = (int)_fractalViewer.RenderQuality;
        }
    }
}