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
            _parts.Add(new RectanglePartDefinition(key, label, initialWidth, initialHeight));
            return this;
        }

        public VerticalCompositePatternBuilder AddTrapezoid(
            string key,
            string label,
            double initialTopWidth,
            double initialBottomWidth,
            double initialHeight)
        {
            _parts.Add(new TrapezoidPartDefinition(
                key,
                label,
                initialTopWidth,
                initialBottomWidth,
                initialHeight));
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
                parameters.Add(new DimensionParameter("Gap", "間隔", _initialGap));
            }

            return new ShapePattern(_name, parameters, BuildDrawing);
        }

        private DrawingModel BuildDrawing(IReadOnlyDictionary<string, double> values)
        {
            var gap = _parts.Count > 1 ? values["Gap"] : 0;
            var totalWidth = _parts.Max(x => x.GetWidth(values));
            var shapes = new List<IReadOnlyList<Point>>();
            var dimensions = new List<DimensionAnnotation>();
            var top = 0.0;

            for (var index = 0; index < _parts.Count; index++)
            {
                var part = _parts[index];
                var width = part.GetWidth(values);
                var height = part.GetHeight(values);
                var left = (totalWidth - width) / 2;

                shapes.Add(part.CreatePoints(values, left, top));
                part.AddDimensions(values, dimensions, left, top, top + height, index);

                var bottom = top + height;
                if (index < _parts.Count - 1)
                {
                    dimensions.Add(new DimensionAnnotation(
                        new Point(totalWidth, bottom),
                        new Point(totalWidth, bottom + gap),
                        new Vector(1, 0),
                        string.Format("間隔 G = {0:g} mm", gap)));
                }

                top = bottom + gap;
            }

            return new DrawingModel(shapes, dimensions);
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

            protected static Vector HeightOffset(int index)
            {
                return index % 2 == 0 ? new Vector(-1, 0) : new Vector(1, 0);
            }
        }

        private sealed class RectanglePartDefinition : PartDefinition
        {
            private readonly double _initialWidth;
            private readonly double _initialHeight;

            public RectanglePartDefinition(
                string key,
                string label,
                double initialWidth,
                double initialHeight)
                : base(key, label)
            {
                _initialWidth = initialWidth;
                _initialHeight = initialHeight;
            }

            public override void AddParameters(ICollection<DimensionParameter> parameters)
            {
                parameters.Add(new DimensionParameter(ParameterKey("Width"), Label + " 幅", _initialWidth));
                parameters.Add(new DimensionParameter(ParameterKey("Height"), Label + " 高さ", _initialHeight));
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
                var heightX = index % 2 == 0 ? left : left + width;

                dimensions.Add(new DimensionAnnotation(
                    new Point(left, bottom),
                    new Point(left + width, bottom),
                    new Vector(0, 1),
                    string.Format("{0} 幅 = {1:g} mm", Label, width)));
                dimensions.Add(new DimensionAnnotation(
                    new Point(heightX, top),
                    new Point(heightX, bottom),
                    HeightOffset(index),
                    string.Format("{0} 高さ = {1:g} mm", Label, height)));
            }
        }

        private sealed class TrapezoidPartDefinition : PartDefinition
        {
            private readonly double _initialTopWidth;
            private readonly double _initialBottomWidth;
            private readonly double _initialHeight;

            public TrapezoidPartDefinition(
                string key,
                string label,
                double initialTopWidth,
                double initialBottomWidth,
                double initialHeight)
                : base(key, label)
            {
                _initialTopWidth = initialTopWidth;
                _initialBottomWidth = initialBottomWidth;
                _initialHeight = initialHeight;
            }

            public override void AddParameters(ICollection<DimensionParameter> parameters)
            {
                parameters.Add(new DimensionParameter(ParameterKey("TopWidth"), Label + " 上底", _initialTopWidth));
                parameters.Add(new DimensionParameter(ParameterKey("BottomWidth"), Label + " 下底", _initialBottomWidth));
                parameters.Add(new DimensionParameter(ParameterKey("Height"), Label + " 高さ", _initialHeight));
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
                var heightX = index % 2 == 0 ? left : left + width;

                dimensions.Add(new DimensionAnnotation(
                    new Point(topLeft, top),
                    new Point(topLeft + topWidth, top),
                    new Vector(0, -1),
                    string.Format("{0} 上底 = {1:g} mm", Label, topWidth)));
                dimensions.Add(new DimensionAnnotation(
                    new Point(bottomLeft, bottom),
                    new Point(bottomLeft + bottomWidth, bottom),
                    new Vector(0, 1),
                    string.Format("{0} 下底 = {1:g} mm", Label, bottomWidth)));
                dimensions.Add(new DimensionAnnotation(
                    new Point(heightX, top),
                    new Point(heightX, bottom),
                    HeightOffset(index),
                    string.Format("{0} 高さ = {1:g} mm", Label, height)));
            }
        }
    }
}
