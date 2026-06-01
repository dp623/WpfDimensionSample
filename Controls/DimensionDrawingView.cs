using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WpfDimensionSample.Models;

namespace WpfDimensionSample.Controls
{
    public sealed class DimensionDrawingView : FrameworkElement
    {
        private const double OuterMargin = 92;
        private const double DimensionGap = 28;
        private static readonly Pen ShapePen = CreatePen(Brushes.SteelBlue, 3);
        private static readonly Pen DimensionPen = CreatePen(Brushes.DimGray, 1.2);
        private static readonly Brush ShapeFill =
            new SolidColorBrush(Color.FromArgb(42, 70, 130, 180));

        public static readonly DependencyProperty DrawingProperty =
            DependencyProperty.Register(
                nameof(Drawing),
                typeof(DrawingModel),
                typeof(DimensionDrawingView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public DrawingModel Drawing
        {
            get { return (DrawingModel)GetValue(DrawingProperty); }
            set { SetValue(DrawingProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(RenderSize));

            if (Drawing == null || Drawing.Shapes.Count == 0)
            {
                return;
            }

            var bounds = GetBounds(Drawing.Shapes.SelectMany(x => x).ToArray());
            var scale = Math.Min(
                Math.Max(1, ActualWidth - OuterMargin * 2) / Math.Max(1, bounds.Width),
                Math.Max(1, ActualHeight - OuterMargin * 2) / Math.Max(1, bounds.Height));

            Func<Point, Point> map = point => new Point(
                OuterMargin + (point.X - bounds.Left) * scale,
                OuterMargin + (point.Y - bounds.Top) * scale);

            foreach (var shape in Drawing.Shapes)
            {
                if (shape.Count >= 3)
                {
                    DrawShape(drawingContext, shape.Select(map).ToArray());
                }
            }

            foreach (var dimension in Drawing.Dimensions)
            {
                DrawDimension(drawingContext, map(dimension.From), map(dimension.To), dimension);
            }
        }

        private static void DrawShape(
            DrawingContext drawingContext,
            IReadOnlyList<Point> points)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(points[0], true, true);
                context.PolyLineTo(points.Skip(1).ToArray(), true, false);
            }

            geometry.Freeze();
            drawingContext.DrawGeometry(ShapeFill, ShapePen, geometry);
        }

        private static void DrawDimension(
            DrawingContext drawingContext,
            Point from,
            Point to,
            DimensionAnnotation annotation)
        {
            var direction = annotation.OffsetDirection;
            direction.Normalize();
            var offset = direction * DimensionGap;
            var extension = direction * 8;
            var dimensionFrom = from + offset;
            var dimensionTo = to + offset;

            drawingContext.DrawLine(DimensionPen, from, dimensionFrom + extension);
            drawingContext.DrawLine(DimensionPen, to, dimensionTo + extension);
            drawingContext.DrawLine(DimensionPen, dimensionFrom, dimensionTo);
            DrawArrow(drawingContext, dimensionFrom, dimensionTo);
            DrawArrow(drawingContext, dimensionTo, dimensionFrom);
            DrawLabel(drawingContext, dimensionFrom, dimensionTo, annotation.Label);
        }

        private static void DrawArrow(DrawingContext drawingContext, Point tip, Point opposite)
        {
            var lineDirection = opposite - tip;
            lineDirection.Normalize();
            var normal = new Vector(-lineDirection.Y, lineDirection.X);
            const double length = 8;
            const double halfWidth = 3.5;

            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(tip, true, true);
                context.LineTo(tip + lineDirection * length + normal * halfWidth, true, false);
                context.LineTo(tip + lineDirection * length - normal * halfWidth, true, false);
            }

            geometry.Freeze();
            drawingContext.DrawGeometry(DimensionPen.Brush, null, geometry);
        }

        private static void DrawLabel(DrawingContext drawingContext, Point from, Point to, string label)
        {
            var text = new FormattedText(
                label,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                13,
                Brushes.Black,
                1);

            var center = new Point((from.X + to.X) / 2, (from.Y + to.Y) / 2);
            var isVertical = Math.Abs(to.Y - from.Y) > Math.Abs(to.X - from.X);
            var origin = isVertical
                ? new Point(center.X + 7, center.Y - text.Height / 2)
                : new Point(center.X - text.Width / 2, center.Y + 6);

            drawingContext.DrawText(text, origin);
        }

        private static Rect GetBounds(IReadOnlyList<Point> points)
        {
            var minX = points.Min(x => x.X);
            var maxX = points.Max(x => x.X);
            var minY = points.Min(x => x.Y);
            var maxY = points.Max(x => x.Y);
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private static Pen CreatePen(Brush brush, double thickness)
        {
            var pen = new Pen(brush, thickness);
            pen.Freeze();
            return pen;
        }
    }
}
