using System;
using FractalVision.Enums;

namespace FractalVision.Models
{
    /// Вычисляет фракталы: считает итерации для каждой точки
    public class FractalCalculator
    {
        private int _maxIterations = 100; 
        private double _escapeRadius = 2.0;
        private FractalType _fractalType = FractalType.Mandelbrot;
        private ComplexNumber _juliaConstant;

        public FractalCalculator()
        {
            // c = -0.7 + 0.27015i
            _juliaConstant = new ComplexNumber(-0.7, 0.27015);
        }

        public int MaxIterations
        {
            get { return _maxIterations; }
            set
            {
                if (value > 0 && value <= 10000)
                    _maxIterations = value;
            }
        }

        /// Радиус побега - когда точка считается ушедшей в бесконечность

        public double EscapeRadius
        {
            get { return _escapeRadius; }
            set
            {
                if (value > 0)
                    _escapeRadius = value;
            }
        }

        public FractalType FractalType
        {
            get { return _fractalType; }
            set { _fractalType = value; }
        }

        public ComplexNumber JuliaConstant
        {
            get { return _juliaConstant; }
            set { _juliaConstant = value; }
        }



        /// z = z² + c где z начинается с 0

        public int CalculateMandelbrot(ComplexNumber c)
        {
            ComplexNumber z = new ComplexNumber(0, 0);
            int iterations = 0;

            while (iterations < _maxIterations)
            {
                z = z.Square().Add(c);

                if (z.Magnitude > _escapeRadius)
                {
                    return iterations;
                }

                iterations++;
            }

            return -1;
        }



        /// z = z² + c где c фиксировано, а z начинается с координаты точки

        public int CalculateJulia(ComplexNumber z)
        {
            ComplexNumber c = _juliaConstant;
            int iterations = 0;

            while (iterations < _maxIterations)
            {
                z = z.Square().Add(c);

                if (z.Magnitude > _escapeRadius)
                {
                    return iterations;
                }

                iterations++;
            }

            return -1;
        }

        public int GetIterationCount(ComplexNumber point)
        {
            switch (_fractalType)
            {
                case FractalType.Mandelbrot:
                    return CalculateMandelbrot(point);

                case FractalType.Julia:
                    return CalculateJulia(point);

                default:
                    return CalculateMandelbrot(point);
            }
        }

        public bool IsInSet(ComplexNumber point)
        {
            int iterations = GetIterationCount(point);
            return iterations == -1; // -1 значит внутри множества
        }

        public int GetFastIterationCount(ComplexNumber point, int maxFastIterations = 50)
        {
            int originalMax = _maxIterations;
            _maxIterations = maxFastIterations;

            int result = GetIterationCount(point);

            _maxIterations = originalMax; 
            return result;
        }

        public System.Drawing.Color GetColorFromIterations(int iterations)
        {
            if (iterations == -1)
            {
                return System.Drawing.Color.Black;
            }

            double ratio = (double)iterations / _maxIterations;

            int red = (int)(255 * ratio);
            int green = (int)(128 * ratio);
            int blue = (int)(255 * (1 - ratio));

            red = Math.Min(255, Math.Max(0, red));
            green = Math.Min(255, Math.Max(0, green));
            blue = Math.Min(255, Math.Max(0, blue));

            return System.Drawing.Color.FromArgb(red, green, blue);
        }
    }
}