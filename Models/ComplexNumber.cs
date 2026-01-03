using System;

namespace FractalVision.Models
{
    public class ComplexNumber
    {
        private double _real;      
        private double _imaginary;

        public ComplexNumber(double real = 0, double imaginary = 0)
        {
            _real = real;
            _imaginary = imaginary;
        }

        public double Real
        {
            get { return _real; }
            set { _real = value; }
        }

        public double Imaginary
        {
            get { return _imaginary; }
            set { _imaginary = value; }
        }

        public double Magnitude
        {
            get
            {
                return Math.Sqrt(Math.Pow(_real, 2) + Math.Pow(_imaginary, 2));
            }
        }




        /// (a + bi) + (c + di) = (a+c) + (b+d)i
        public ComplexNumber Add(ComplexNumber other)
        {
            return new ComplexNumber(
                _real + other._real,
                _imaginary + other._imaginary
            );
        }



        /// (a + bi) * (c + di) = (ac - bd) + (ad + bc)i
        public ComplexNumber Multiply(ComplexNumber other)
        {
            double newReal = (_real * other._real) - (_imaginary * other._imaginary);
            double newImaginary = (_real * other._imaginary) + (_imaginary * other._real);

            return new ComplexNumber(newReal, newImaginary);
        }


        public ComplexNumber Square()
        {
            // (a + bi)² = (a² - b²) + (2ab)i
            double newReal = (_real * _real) - (_imaginary * _imaginary);
            double newImaginary = 2 * _real * _imaginary;

            return new ComplexNumber(newReal, newImaginary);
        }


        public override string ToString()
        {
            string sign = _imaginary >= 0 ? "+" : "-";
            return $"{_real:F2} {sign} {Math.Abs(_imaginary):F2}i";
        }
    }
}