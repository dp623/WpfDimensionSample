using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfDimensionSample.Models
{
    // 図形上に表示する寸法線とラベルの情報です。
    // OffsetDirectionで、寸法線を図形からどちら側へ逃がすかを指定します。
    public sealed class DimensionAnnotation
    {
        public DimensionAnnotation(
            Point from,
            Point to,
            Vector offsetDirection,
            string label,
            bool isLineVisible = true,
            bool isLabelVisible = true)
        {
            From = from;
            To = to;
            OffsetDirection = offsetDirection;
            Label = label;
            IsLineVisible = isLineVisible;
            IsLabelVisible = isLabelVisible;
        }

        public Point From { get; private set; }
        public Point To { get; private set; }
        public Vector OffsetDirection { get; private set; }
        public string Label { get; private set; }
        public bool IsLineVisible { get; private set; }
        public bool IsLabelVisible { get; private set; }
    }

    public sealed class DrawingModel
    {
        public DrawingModel(
            IReadOnlyList<Point> shapePoints,
            IReadOnlyList<DimensionAnnotation> dimensions)
            : this(
                new List<IReadOnlyList<Point>> { shapePoints },
                new List<CircleShape>(),
                dimensions)
        {
        }

        public DrawingModel(
            IReadOnlyList<IReadOnlyList<Point>> shapes,
            IReadOnlyList<DimensionAnnotation> dimensions)
            : this(shapes, new List<CircleShape>(), dimensions)
        {
        }

        public DrawingModel(
            IReadOnlyList<IReadOnlyList<Point>> shapes,
            IReadOnlyList<CircleShape> circles,
            IReadOnlyList<DimensionAnnotation> dimensions)
        {
            Shapes = shapes;
            Circles = circles;
            Dimensions = dimensions;
        }

        public IReadOnlyList<IReadOnlyList<Point>> Shapes { get; private set; }
        public IReadOnlyList<CircleShape> Circles { get; private set; }
        public IReadOnlyList<DimensionAnnotation> Dimensions { get; private set; }
    }

    // 真円はポリゴンではなく中心点と半径で保持します。
    // 表示時はDimensionDrawingView側で同じスケールを掛けて描画します。
    public sealed class CircleShape
    {
        public CircleShape(Point center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public Point Center { get; private set; }
        public double Radius { get; private set; }
    }
}
