using System.Collections.Generic;
using System.Linq;
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
            : this(
                new List<IReadOnlyList<Point>> { shapePoints },
                dimensions)
        {
        }

        public DrawingModel(
            IReadOnlyList<IReadOnlyList<Point>> shapes,
            IReadOnlyList<DimensionAnnotation> dimensions)
        {
            Shapes = shapes;
            Dimensions = dimensions;
        }

        public IReadOnlyList<IReadOnlyList<Point>> Shapes { get; private set; }
        public IReadOnlyList<DimensionAnnotation> Dimensions { get; private set; }
    }
}
