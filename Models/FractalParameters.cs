using System;

namespace FractalVision.Models
{
    /// Параметры области просмотра фрактала

    public class FractalParameters
    {
        private double _centerX = 0;
        private double _centerY = 0;
        private double _zoom = 1.0; 
        private int _width = 800; 
        private int _height = 600; 
        private double _aspectRatio;

        public FractalParameters()
        {
            UpdateAspectRatio();
        }

        public FractalParameters(double centerX, double centerY, double zoom, int width, int height)
        {
            _centerX = centerX;
            _centerY = centerY;
            _zoom = zoom;
            _width = width;
            _height = height;
            UpdateAspectRatio();
        }

        public double CenterX
        {
            get { return _centerX; }
            set { _centerX = value; }
        }

        public double CenterY
        {
            get { return _centerY; }
            set { _centerY = value; }
        }

        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (value > 0)
                    _zoom = value;
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                if (value > 0)
                {
                    _width = value;
                    UpdateAspectRatio();
                }
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                if (value > 0)
                {
                    _height = value;
                    UpdateAspectRatio();
                }
            }
        }

        public double AspectRatio
        {
            get { return _aspectRatio; }
        }

        public double ZoomFactor
        {
            get { return _zoom; }
        }





        public double PixelXToComplex(int pixelX)
        {
            double rangeX = 4.0 / _zoom;

            double left = _centerX - (rangeX / 2);

            return left + (rangeX * pixelX / _width);
        }

        public double PixelYToComplex(int pixelY)
        {
            double rangeY = (4.0 / _zoom) / _aspectRatio;

            // Верхняя граница = центр + половина диапазона (y перевернут)
            double top = _centerY + (rangeY / 2);

            return top - (rangeY * pixelY / _height);
        }

        public ComplexNumber PixelToComplex(int pixelX, int pixelY)
        {
            return new ComplexNumber(
                PixelXToComplex(pixelX),
                PixelYToComplex(pixelY)
            );
        }

        public void Pan(double deltaX, double deltaY)
        {
            _centerX += deltaX / _zoom;
            _centerY += deltaY / _zoom;
        }

        public void ZoomToPoint(int pixelX, int pixelY, double zoomFactor)
        {
            ComplexNumber point = PixelToComplex(pixelX, pixelY);

            _centerX = point.Real;
            _centerY = point.Imaginary;

            _zoom *= zoomFactor;
        }

        public void Reset()
        {
            _centerX = 0;
            _centerY = 0;
            _zoom = 1.0;
        }

        public FractalParameters Clone()
        {
            return new FractalParameters(_centerX, _centerY, _zoom, _width, _height);
        }

        public override string ToString()
        {
            return $"Центр: ({_centerX:F4}, {_centerY:F4}), Зум: {_zoom:F2}, Размер: {_width}x{_height}";
        }

        public static FractalParameters FromString(string str)
        {
            try
            {
                // Простой парсинг: "0.0,0.0,1.0,800,600"
                string[] parts = str.Split(',');

                if (parts.Length == 5)
                {
                    return new FractalParameters(
                        double.Parse(parts[0]),
                        double.Parse(parts[1]),
                        double.Parse(parts[2]),
                        int.Parse(parts[3]),
                        int.Parse(parts[4])
                    );
                }
            }
            catch
            {
                // в случае ошибки возвращаем значения по умолчанию
            }

            return new FractalParameters();
        }

        private void UpdateAspectRatio()
        {
            if (_height > 0)
                _aspectRatio = (double)_width / _height;
            else
                _aspectRatio = 1.0;
        }
    }
}