using System.Drawing;

namespace FractalVision.Models
{
    public class ZoomRectangle
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public bool IsSelecting { get; set; }

        public Rectangle GetRectangle()
        {
            int x = Math.Min(StartPoint.X, EndPoint.X);
            int y = Math.Min(StartPoint.Y, EndPoint.Y);
            int width = Math.Abs(EndPoint.X - StartPoint.X);
            int height = Math.Abs(EndPoint.Y - StartPoint.Y);

            return new Rectangle(x, y, width, height);
        }

        public bool IsValid()
        {
            var rect = GetRectangle();
            return rect.Width > 5 && rect.Height > 5;
        }

        public void Reset()
        {
            StartPoint = Point.Empty;
            EndPoint = Point.Empty;
            IsSelecting = false;
        }

        public override string ToString()
        {
            var rect = GetRectangle();
            return $"({rect.X}, {rect.Y}) - [{rect.Width}x{rect.Height}]";
        }
    }
}