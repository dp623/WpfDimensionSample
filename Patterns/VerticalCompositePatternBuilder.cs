using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfDimensionSample.Models;

namespace WpfDimensionSample.Patterns
{
    public sealed class VerticalCompositePatternBuilder
    {
        private readonly string _name;
        private readonly double _initialGap;
        private readonly List<PartDefinition> _parts = new List<PartDefinition>();
        private DimensionDisplay _gapDisplay = DimensionDisplay.Visible();

        public VerticalCompositePatternBuilder(string name, double initialGap)
        {
            _name = name;
            _initialGap = initialGap;
        }

        public VerticalCompositePatternBuilder AddRectangle(
            string key,
            string label,
            double initialWidth,
            double initialHeight)
        {
            return AddRectangle(
                key,
                label,
                initialWidth,
                initialHeight,
                DimensionDisplay.Visible(),
                DimensionDisplay.Visible());
        }

        public VerticalCompositePatternBuilder AddRectangle(
            string key,
            string label,
            double initialWidth,
            double initialHeight,
            DimensionDisplay widthDisplay,
            DimensionDisplay heightDisplay)
        {
            _parts.Add(new RectanglePartDefinition(
                key,
                label,
                initialWidth,
                initialHeight,
                widthDisplay,
                heightDisplay));
            return this;
        }

        public VerticalCompositePatternBuilder AddTrapezoid(
            string key,
            string label,
            double initialTopWidth,
            double initialBottomWidth,
            double initialHeight)
        {
            return AddTrapezoid(
                key,
                label,
                initialTopWidth,
                initialBottomWidth,
                initialHeight,
                DimensionDisplay.Visible(),
                DimensionDisplay.Visible(),
                DimensionDisplay.Visible());
        }

        public VerticalCompositePatternBuilder AddTrapezoid(
            string key,
            string label,
            double initialTopWidth,
            double initialBottomWidth,
            double initialHeight,
            DimensionDisplay topWidthDisplay,
            DimensionDisplay bottomWidthDisplay,
            DimensionDisplay heightDisplay)
        {
            _parts.Add(new TrapezoidPartDefinition(
                key,
                label,
                initialTopWidth,
                initialBottomWidth,
                initialHeight,
                topWidthDisplay,
                bottomWidthDisplay,
                heightDisplay));
            return this;
        }

        public VerticalCompositePatternBuilder AddRectangleWithCircle(
            string key,
            string label,
            double initialWidth,
            double initialHeight,
            double initialCircleRadius,
            double initialCircleCenterX,
            double initialCircleCenterY)
        {
            _parts.Add(new RectangleWithCirclePartDefinition(
                key,
                label,
                initialWidth,
                initialHeight,
                initialCircleRadius,
                initialCircleCenterX,
                initialCircleCenterY));
            return this;
        }

        public VerticalCompositePatternBuilder SetGapDisplay(DimensionDisplay display)
        {
            _gapDisplay = display;
            return this;
        }

        public ShapePattern Build()
        {
            if (_parts.Count == 0)
            {
                throw new InvalidOperationException("図形を1つ以上追加してください。");
            }

            var parameters = new List<DimensionParameter>();
            foreach (var part in _parts)
            {
                part.AddParameters(parameters);
            }

            if (_parts.Count > 1)
            {
                parameters.Add(new DimensionParameter(
                    "Gap",
                    "間隔",
                    _initialGap,
                    "mm",
                    _gapDisplay.IsInputVisible));
            }

            var parameterMap = parameters.ToDictionary(x => x.Key);
            var inputRows = new List<InputRow>();
            foreach (var part in _parts)
            {
                part.AddInputRows(inputRows, parameterMap);
            }

            if (_parts.Count > 1)
            {
                var gap = parameterMap["Gap"];
                inputRows.Add(new InputRow(
                    gap.Label,
                    new List<InputSlot>
                    {
                        new InputSlot(string.Empty, gap, gap.Unit)
                    },
                    gap.IsVisible));
            }

            return new ShapePattern(
                _name,
                parameters,
                inputRows,
                values =>
                {
                    foreach (var part in _parts)
                    {
                        part.UpdateComputedParameters(parameterMap, values);
                    }
                },
                BuildDrawing);
        }

        private DrawingModel BuildDrawing(IReadOnlyDictionary<string, double> values)
        {
            var gap = _parts.Count > 1 ? values["Gap"] : 0;
            var totalWidth = _parts.Max(x => x.GetWidth(values));
            var shapes = new List<IReadOnlyList<Point>>();
            var circles = new List<CircleShape>();
            var dimensions = new List<DimensionAnnotation>();
            var top = 0.0;

            for (var index = 0; index < _parts.Count; index++)
            {
                var part = _parts[index];
                var width = part.GetWidth(values);
                var height = part.GetHeight(values);
                var left = (totalWidth - width) / 2;

                shapes.Add(part.CreatePoints(values, left, top));
                circles.AddRange(part.CreateCircles(values, left, top));
                part.AddDimensions(values, dimensions, left, top, top + height, index);

                var bottom = top + height;
                if (index < _parts.Count - 1)
                {
                    AddAnnotation(
                        dimensions,
                        new Point(totalWidth, bottom),
                        new Point(totalWidth, bottom + gap),
                        new Vector(1, 0),
                        string.Format("間隔 G = {0:g} mm", gap),
                        _gapDisplay);
                }

                top = bottom + gap;
            }

            return new DrawingModel(shapes, circles, dimensions);
        }

        private static void AddAnnotation(
            ICollection<DimensionAnnotation> dimensions,
            Point from,
            Point to,
            Vector offsetDirection,
            string label,
            DimensionDisplay display)
        {
            if (!display.IsLineVisible && !display.IsLabelVisible)
            {
                return;
            }

            dimensions.Add(new DimensionAnnotation(
                from,
                to,
                offsetDirection,
                label,
                display.IsLineVisible,
                display.IsLabelVisible));
        }

        private abstract class PartDefinition
        {
            protected PartDefinition(string key, string label)
            {
                Key = key;
                Label = label;
            }

            protected string Key { get; private set; }
            protected string Label { get; private set; }

            public abstract void AddParameters(ICollection<DimensionParameter> parameters);
            public abstract double GetWidth(IReadOnlyDictionary<string, double> values);
            public abstract double GetHeight(IReadOnlyDictionary<string, double> values);
            public abstract IReadOnlyList<Point> CreatePoints(
                IReadOnlyDictionary<string, double> values,
                double left,
                double top);
            public virtual IReadOnlyList<CircleShape> CreateCircles(
                IReadOnlyDictionary<string, double> values,
                double left,
                double top)
            {
                return new List<CircleShape>();
            }

            public abstract void AddInputRows(
                ICollection<InputRow> rows,
                IReadOnlyDictionary<string, DimensionParameter> parameters);

            public virtual void UpdateComputedParameters(
                IReadOnlyDictionary<string, DimensionParameter> parameters,
                IReadOnlyDictionary<string, double> values)
            {
            }

            public abstract void AddDimensions(
                IReadOnlyDictionary<string, double> values,
                ICollection<DimensionAnnotation> dimensions,
                double left,
                double top,
                double bottom,
                int index);

            protected string ParameterKey(string suffix)
            {
                return Key + "." + suffix;
            }

        }

        private sealed class RectanglePartDefinition : PartDefinition
        {
            private readonly double _initialWidth;
            private readonly double _initialHeight;
            private readonly DimensionDisplay _widthDisplay;
            private readonly DimensionDisplay _heightDisplay;

            public RectanglePartDefinition(
                string key,
                string label,
                double initialWidth,
                double initialHeight,
                DimensionDisplay widthDisplay,
                DimensionDisplay heightDisplay)
                : base(key, label)
            {
                _initialWidth = initialWidth;
                _initialHeight = initialHeight;
                _widthDisplay = widthDisplay;
                _heightDisplay = heightDisplay;
            }

            public override void AddParameters(ICollection<DimensionParameter> parameters)
            {
                parameters.Add(new DimensionParameter(
                    ParameterKey("Width"),
                    Label + " 幅",
                    _initialWidth,
                    "mm",
                    _widthDisplay.IsInputVisible));
                parameters.Add(new DimensionParameter(
                    ParameterKey("Height"),
                    Label + " 高さ",
                    _initialHeight,
                    "mm",
                    _heightDisplay.IsInputVisible));
            }

            public override double GetWidth(IReadOnlyDictionary<string, double> values)
            {
                return values[ParameterKey("Width")];
            }

            public override double GetHeight(IReadOnlyDictionary<string, double> values)
            {
                return values[ParameterKey("Height")];
            }

            public override IReadOnlyList<Point> CreatePoints(
                IReadOnlyDictionary<string, double> values,
                double left,
                double top)
            {
                var width = GetWidth(values);
                var height = GetHeight(values);
                return ShapePoints.CreateRectangle(left, top, width, height);
            }

            public override void AddInputRows(
                ICollection<InputRow> rows,
                IReadOnlyDictionary<string, DimensionParameter> parameters)
            {
                var width = parameters[ParameterKey("Width")];
                var height = parameters[ParameterKey("Height")];
                rows.Add(new InputRow(
                    Label,
                    new List<InputSlot>
                    {
                        new InputSlot(string.Empty, width, "W x"),
                        new InputSlot(string.Empty, height, "H")
                    }));
            }

            public override void AddDimensions(
                IReadOnlyDictionary<string, double> values,
                ICollection<DimensionAnnotation> dimensions,
                double left,
                double top,
                double bottom,
                int index)
            {
                var width = GetWidth(values);
                var height = GetHeight(values);
                var heightX = left + width;

                AddAnnotation(
                    dimensions,
                    new Point(left, bottom),
                    new Point(left + width, bottom),
                    new Vector(0, 1),
                    string.Format("{0} 幅 = {1:g} mm", Label, width),
                    _widthDisplay);
                AddAnnotation(
                    dimensions,
                    new Point(heightX, top),
                    new Point(heightX, bottom),
                    new Vector(1, 0),
                    string.Format("{0} 高さ = {1:g} mm", Label, height),
                    _heightDisplay);
            }
        }

        private sealed class RectangleWithCirclePartDefinition : PartDefinition
        {
            private readonly double _initialWidth;
            private readonly double _initialHeight;
            private readonly double _initialCircleRadius;
            private readonly double _initialCircleCenterX;
            private readonly double _initialCircleCenterY;

            public RectangleWithCirclePartDefinition(
                string key,
                string label,
                double initialWidth,
                double initialHeight,
                double initialCircleRadius,
                double initialCircleCenterX,
                double initialCircleCenterY)
                : base(key, label)
            {
                _initialWidth = initialWidth;
                _initialHeight = initialHeight;
                _initialCircleRadius = initialCircleRadius;
                _initialCircleCenterX = initialCircleCenterX;
                _initialCircleCenterY = initialCircleCenterY;
            }

            public override void AddParameters(ICollection<DimensionParameter> parameters)
            {
                parameters.Add(new DimensionParameter(ParameterKey("Width"), Label + " 幅", _initialWidth));
                parameters.Add(new DimensionParameter(ParameterKey("Height"), Label + " 高さ", _initialHeight));
                parameters.Add(new DimensionParameter(ParameterKey("CircleRadius"), "真円 半径", _initialCircleRadius));
                parameters.Add(new DimensionParameter(ParameterKey("CircleCenterX"), "真円 中心X", _initialCircleCenterX));
                parameters.Add(new DimensionParameter(ParameterKey("CircleCenterY"), "真円 中心Y", _initialCircleCenterY));
                parameters.Add(new DimensionParameter(
                    ParameterKey("CircleLeftDistance"),
                    "真円 左距離",
                    _initialCircleCenterX - _initialCircleRadius,
                    "mm",
                    true,
                    true));
                parameters.Add(new DimensionParameter(
                    ParameterKey("CircleTopDistance"),
                    "真円 上距離",
                    _initialCircleCenterY - _initialCircleRadius,
                    "mm",
                    true,
                    true));
            }

            public override double GetWidth(IReadOnlyDictionary<string, double> values)
            {
                return values[ParameterKey("Width")];
            }

            public override double GetHeight(IReadOnlyDictionary<string, double> values)
            {
                return values[ParameterKey("Height")];
            }

            public override IReadOnlyList<Point> CreatePoints(
                IReadOnlyDictionary<string, double> values,
                double left,
                double top)
            {
                return ShapePoints.CreateRectangle(left, top, GetWidth(values), GetHeight(values));
            }

            public override IReadOnlyList<CircleShape> CreateCircles(
                IReadOnlyDictionary<string, double> values,
                double left,
                double top)
            {
                return new List<CircleShape>
                {
                    new CircleShape(
                        new Point(
                            left + values[ParameterKey("CircleCenterX")],
                            top + values[ParameterKey("CircleCenterY")]),
                        values[ParameterKey("CircleRadius")])
                };
            }

            public override void AddInputRows(
                ICollection<InputRow> rows,
                IReadOnlyDictionary<string, DimensionParameter> parameters)
            {
                rows.Add(new InputRow(
                    Label,
                    new List<InputSlot>
                    {
                        new InputSlot(string.Empty, parameters[ParameterKey("Width")], "W x"),
                        new InputSlot(string.Empty, parameters[ParameterKey("Height")], "H")
                    }));
                rows.Add(new InputRow(
                    "真円　半径",
                    new List<InputSlot>
                    {
                        new InputSlot(string.Empty, parameters[ParameterKey("CircleRadius")], string.Empty)
                    }));
                rows.Add(new InputRow(
                    "真円　位置",
                    new List<InputSlot>
                    {
                        new InputSlot("X", parameters[ParameterKey("CircleCenterX")], "x"),
                        new InputSlot("Y", parameters[ParameterKey("CircleCenterY")], string.Empty)
                    },
                    true,
                    true,
                    true));
                rows.Add(new InputRow(
                    "真円　距離",
                    new List<InputSlot>
                    {
                        new InputSlot("左", parameters[ParameterKey("CircleLeftDistance")], "x"),
                        new InputSlot("上", parameters[ParameterKey("CircleTopDistance")], string.Empty)
                    }));
            }

            public override void UpdateComputedParameters(
                IReadOnlyDictionary<string, DimensionParameter> parameters,
                IReadOnlyDictionary<string, double> values)
            {
                var radius = values[ParameterKey("CircleRadius")];
                var centerX = values[ParameterKey("CircleCenterX")];
                var centerY = values[ParameterKey("CircleCenterY")];

                parameters[ParameterKey("CircleLeftDistance")].SetComputedValue(centerX - radius);
                parameters[ParameterKey("CircleTopDistance")].SetComputedValue(centerY - radius);
            }

            public override void AddDimensions(
                IReadOnlyDictionary<string, double> values,
                ICollection<DimensionAnnotation> dimensions,
                double left,
                double top,
                double bottom,
                int index)
            {
                var width = GetWidth(values);
                var height = GetHeight(values);
                var radius = values[ParameterKey("CircleRadius")];
                var centerX = values[ParameterKey("CircleCenterX")];
                var centerY = values[ParameterKey("CircleCenterY")];
                var leftDistance = values[ParameterKey("CircleLeftDistance")];
                var topDistance = values[ParameterKey("CircleTopDistance")];
                var center = new Point(left + centerX, top + centerY);
                var circleLeft = center.X - radius;
                var circleTop = center.Y - radius;

                AddAnnotation(
                    dimensions,
                    new Point(left, bottom),
                    new Point(left + width, bottom),
                    new Vector(0, 1),
                    string.Format("{0} 幅 = {1:g} mm", Label, width),
                    DimensionDisplay.Visible());
                AddAnnotation(
                    dimensions,
                    new Point(left + width, top),
                    new Point(left + width, bottom),
                    new Vector(1, 0),
                    string.Format("{0} 高さ = {1:g} mm", Label, height),
                    DimensionDisplay.Visible());
                AddAnnotation(
                    dimensions,
                    center,
                    new Point(center.X + radius, center.Y),
                    new Vector(0, 1),
                    string.Format("真円 半径 R = {0:g} mm", radius),
                    DimensionDisplay.Visible());
                AddAnnotation(
                    dimensions,
                    new Point(left, center.Y),
                    center,
                    new Vector(0, -1),
                    string.Format("中心X = {0:g} mm", centerX),
                    DimensionDisplay.Visible());
                AddAnnotation(
                    dimensions,
                    new Point(left + width, top),
                    new Point(left + width, center.Y),
                    new Vector(1, 0),
                    string.Format("中心Y = {0:g} mm", centerY),
                    DimensionDisplay.Visible());
                AddAnnotation(
                    dimensions,
                    new Point(left, circleTop),
                    new Point(circleLeft, circleTop),
                    new Vector(0, -1),
                    string.Format("左距離 = {0:g} mm", leftDistance),
                    DimensionDisplay.Visible());
                AddAnnotation(
                    dimensions,
                    new Point(left + width, top),
                    new Point(left + width, circleTop),
                    new Vector(1, 0),
                    string.Format("上距離 = {0:g} mm", topDistance),
                    DimensionDisplay.Visible());
            }
        }

        private sealed class TrapezoidPartDefinition : PartDefinition
        {
            private readonly double _initialTopWidth;
            private readonly double _initialBottomWidth;
            private readonly double _initialHeight;
            private readonly DimensionDisplay _topWidthDisplay;
            private readonly DimensionDisplay _bottomWidthDisplay;
            private readonly DimensionDisplay _heightDisplay;

            public TrapezoidPartDefinition(
                string key,
                string label,
                double initialTopWidth,
                double initialBottomWidth,
                double initialHeight,
                DimensionDisplay topWidthDisplay,
                DimensionDisplay bottomWidthDisplay,
                DimensionDisplay heightDisplay)
                : base(key, label)
            {
                _initialTopWidth = initialTopWidth;
                _initialBottomWidth = initialBottomWidth;
                _initialHeight = initialHeight;
                _topWidthDisplay = topWidthDisplay;
                _bottomWidthDisplay = bottomWidthDisplay;
                _heightDisplay = heightDisplay;
            }

            public override void AddParameters(ICollection<DimensionParameter> parameters)
            {
                parameters.Add(new DimensionParameter(
                    ParameterKey("TopWidth"),
                    Label + " 上底",
                    _initialTopWidth,
                    "mm",
                    _topWidthDisplay.IsInputVisible));
                parameters.Add(new DimensionParameter(
                    ParameterKey("BottomWidth"),
                    Label + " 下底",
                    _initialBottomWidth,
                    "mm",
                    _bottomWidthDisplay.IsInputVisible));
                parameters.Add(new DimensionParameter(
                    ParameterKey("Height"),
                    Label + " 高さ",
                    _initialHeight,
                    "mm",
                    _heightDisplay.IsInputVisible));
            }

            public override double GetWidth(IReadOnlyDictionary<string, double> values)
            {
                return Math.Max(
                    values[ParameterKey("TopWidth")],
                    values[ParameterKey("BottomWidth")]);
            }

            public override double GetHeight(IReadOnlyDictionary<string, double> values)
            {
                return values[ParameterKey("Height")];
            }

            public override IReadOnlyList<Point> CreatePoints(
                IReadOnlyDictionary<string, double> values,
                double left,
                double top)
            {
                var topWidth = values[ParameterKey("TopWidth")];
                var bottomWidth = values[ParameterKey("BottomWidth")];
                var width = GetWidth(values);
                var height = GetHeight(values);
                var topLeft = left + (width - topWidth) / 2;
                var bottomLeft = left + (width - bottomWidth) / 2;

                return new List<Point>
                {
                    new Point(topLeft, top),
                    new Point(topLeft + topWidth, top),
                    new Point(bottomLeft + bottomWidth, top + height),
                    new Point(bottomLeft, top + height)
                };
            }

            public override void AddInputRows(
                ICollection<InputRow> rows,
                IReadOnlyDictionary<string, DimensionParameter> parameters)
            {
                rows.Add(new InputRow(
                    Label,
                    new List<InputSlot>
                    {
                        new InputSlot("上", parameters[ParameterKey("TopWidth")], "x"),
                        new InputSlot("下", parameters[ParameterKey("BottomWidth")], "x"),
                        new InputSlot("H", parameters[ParameterKey("Height")], string.Empty)
                    }));
            }

            public override void AddDimensions(
                IReadOnlyDictionary<string, double> values,
                ICollection<DimensionAnnotation> dimensions,
                double left,
                double top,
                double bottom,
                int index)
            {
                var topWidth = values[ParameterKey("TopWidth")];
                var bottomWidth = values[ParameterKey("BottomWidth")];
                var width = GetWidth(values);
                var height = GetHeight(values);
                var topLeft = left + (width - topWidth) / 2;
                var bottomLeft = left + (width - bottomWidth) / 2;
                var heightX = left + width;

                AddAnnotation(
                    dimensions,
                    new Point(topLeft, top),
                    new Point(topLeft + topWidth, top),
                    new Vector(0, -1),
                    string.Format("{0} 上底 = {1:g} mm", Label, topWidth),
                    _topWidthDisplay);
                AddAnnotation(
                    dimensions,
                    new Point(bottomLeft, bottom),
                    new Point(bottomLeft + bottomWidth, bottom),
                    new Vector(0, 1),
                    string.Format("{0} 下底 = {1:g} mm", Label, bottomWidth),
                    _bottomWidthDisplay);
                AddAnnotation(
                    dimensions,
                    new Point(heightX, top),
                    new Point(heightX, bottom),
                    new Vector(1, 0),
                    string.Format("{0} 高さ = {1:g} mm", Label, height),
                    _heightDisplay);
            }
        }
    }
}
