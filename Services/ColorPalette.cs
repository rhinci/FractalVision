using System;
using System.Drawing;
using FractalVision.Enums;

namespace FractalVision.Services
{
    public class ColorPalette
    {
        private Color[] _colors = Array.Empty<Color>();
        private ColorScheme _paletteType;
        private int _colorCount = 256;

        public ColorPalette()
        {
            _paletteType = ColorScheme.Rainbow;
            CreateRainbowPalette();
        }

        public ColorPalette(ColorScheme paletteType)
        {
            _paletteType = paletteType;
            CreatePalette(paletteType);
        }

        public ColorPalette(Color[] customColors)
        {
            _paletteType = ColorScheme.Custom;
            _colors = customColors;
            _colorCount = customColors.Length;
        }

        public ColorScheme PaletteType
        {
            get { return _paletteType; }
            set
            {
                _paletteType = value;
                CreatePalette(value);
            }
        }

        public int ColorCount
        {
            get { return _colorCount; }
            set
            {
                if (value > 0 && value <= 4096)
                {
                    _colorCount = value;
                    CreatePalette(_paletteType);
                }
            }
        }

        public Color[] Colors => _colors;

        public Color GetColor(int iteration, int maxIterations)
        {
            if (iteration == -1) return Color.Black;

            double ratio = (double)iteration / maxIterations;
            double smoothRatio = 1 - Math.Pow(1 - ratio, 0.5);
            int colorIndex = (int)(smoothRatio * (_colorCount - 1));
            colorIndex = Math.Max(0, Math.Min(_colorCount - 1, colorIndex));

            return _colors[colorIndex];
        }

        public Color GetFastColor(int iteration, int maxIterations)
        {
            if (iteration == -1) return Color.Black;

            int index = (iteration * _colorCount) / maxIterations;
            index = Math.Max(0, Math.Min(_colorCount - 1, index));

            return _colors[index];
        }

        private void CreatePalette(ColorScheme type)
        {
            switch (type)
            {
                case ColorScheme.Rainbow:
                    CreateRainbowPalette();
                    break;
                case ColorScheme.Fire:
                    CreateFirePalette();
                    break;
                case ColorScheme.Ocean:
                    CreateOceanPalette();
                    break;
                case ColorScheme.Forest:
                    CreateForestPalette();
                    break;
                case ColorScheme.Grayscale:
                    CreateGrayscalePalette();
                    break;
                default:
                    CreateRainbowPalette();
                    break;
            }
        }

        public void CreateRainbowPalette()
        {
            _colors = new Color[_colorCount];

            for (int i = 0; i < _colorCount; i++)
            {
                double hue = (240.0 * i) / (_colorCount - 1);
                _colors[i] = HsvToRgb(hue, 1.0, 1.0);
            }
        }

        public void CreateFirePalette()
        {
            _colors = new Color[_colorCount];

            for (int i = 0; i < _colorCount; i++)
            {
                double ratio = (double)i / (_colorCount - 1);
                int r, g, b;

                if (ratio < 0.33)
                {
                    double subRatio = ratio / 0.33;
                    r = (int)(255 * subRatio);
                    g = 0;
                    b = 0;
                }
                else if (ratio < 0.66)
                {
                    double subRatio = (ratio - 0.33) / 0.33;
                    r = 255;
                    g = (int)(255 * subRatio);
                    b = 0;
                }
                else
                {
                    double subRatio = (ratio - 0.66) / 0.34;
                    r = 255;
                    g = 255;
                    b = (int)(255 * subRatio);
                }

                _colors[i] = Color.FromArgb(r, g, b);
            }
        }

        public void CreateOceanPalette()
        {
            _colors = new Color[_colorCount];

            for (int i = 0; i < _colorCount; i++)
            {
                double ratio = (double)i / (_colorCount - 1);
                int r = (int)(50 * (1 - ratio));
                int g = (int)(100 + 155 * ratio);
                int b = (int)(200 + 55 * ratio);
                _colors[i] = Color.FromArgb(r, g, b);
            }
        }

        public void CreateForestPalette()
        {
            _colors = new Color[_colorCount];

            for (int i = 0; i < _colorCount; i++)
            {
                double ratio = (double)i / (_colorCount - 1);
                int r = (int)(50 * ratio);
                int g = (int)(100 + 155 * ratio);
                int b = (int)(50 * ratio);
                _colors[i] = Color.FromArgb(r, g, b);
            }
        }

        public void CreateGrayscalePalette()
        {
            _colors = new Color[_colorCount];

            for (int i = 0; i < _colorCount; i++)
            {
                int intensity = (int)(255.0 * i / (_colorCount - 1));
                _colors[i] = Color.FromArgb(intensity, intensity, intensity);
            }
        }

        public void CreateCustomPalette(Color startColor, Color endColor)
        {
            _colors = new Color[_colorCount];
            _paletteType = ColorScheme.Custom;

            for (int i = 0; i < _colorCount; i++)
            {
                double ratio = (double)i / (_colorCount - 1);
                int r = Lerp(startColor.R, endColor.R, ratio);
                int g = Lerp(startColor.G, endColor.G, ratio);
                int b = Lerp(startColor.B, endColor.B, ratio);
                _colors[i] = Color.FromArgb(r, g, b);
            }
        }

        private Color HsvToRgb(double h, double s, double v)
        {
            h = h % 360;
            if (h < 0) h += 360;

            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;

            double r1 = 0, g1 = 0, b1 = 0;

            if (h < 60) { r1 = c; g1 = x; }
            else if (h < 120) { r1 = x; g1 = c; }
            else if (h < 180) { g1 = c; b1 = x; }
            else if (h < 240) { g1 = x; b1 = c; }
            else if (h < 300) { r1 = x; b1 = c; }
            else { r1 = c; b1 = x; }

            int r = (int)((r1 + m) * 255);
            int g = (int)((g1 + m) * 255);
            int b = (int)((b1 + m) * 255);

            return Color.FromArgb(r, g, b);
        }

        private int Lerp(int start, int end, double ratio)
        {
            return (int)(start + (end - start) * ratio);
        }

        public string GetPaletteInfo()
        {
            return $"Тип: {_paletteType}, Цветов: {_colorCount}";
        }
    }
}