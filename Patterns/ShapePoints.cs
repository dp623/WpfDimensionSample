using System.Collections.Generic;
using System.Windows;

namespace WpfDimensionSample.Patterns
{
    public static class ShapePoints
    {
        public static IReadOnlyList<Point> CreateRectangle(
            double left,
            double top,
            double width,
            double height)
        {
            return new List<Point>
            {
                new Point(left, top),
                new Point(left + width, top),
                new Point(left + width, top + height),
                new Point(left, top + height)
            };
        }
    }
}
