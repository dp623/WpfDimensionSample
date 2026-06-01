using System.Collections.Generic;
using System.Windows;

namespace WpfDimensionSample.Models
{
    public sealed class DimensionAnnotation
    {
        public DimensionAnnotation(Point from, Point to, Vector offsetDirection, string label)
        {
            From = from;
            To = to;
            OffsetDirection = offsetDirection;
            Label = label;
        }

        public Point From { get; private set; }
        public Point To { get; private set; }
        public Vector OffsetDirection { get; private set; }
        public string Label { get; private set; }
    }

    public sealed class DrawingModel
    {
        public DrawingModel(
            IReadOnlyList<Point> shapePoints,
            IReadOnlyList<DimensionAnnotation> dimensions)
        {
            ShapePoints = shapePoints;
            Dimensions = dimensions;
        }

        public IReadOnlyList<Point> ShapePoints { get; private set; }
        public IReadOnlyList<DimensionAnnotation> Dimensions { get; private set; }
    }
}
