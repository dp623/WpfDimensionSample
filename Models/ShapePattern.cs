using System;
using System.Collections.Generic;

namespace WpfDimensionSample.Models
{
    public sealed class ShapePattern
    {
        public ShapePattern(
            string name,
            IReadOnlyList<DimensionParameter> parameters,
            Func<IReadOnlyDictionary<string, double>, DrawingModel> buildDrawing)
        {
            Name = name;
            Parameters = parameters;
            BuildDrawing = buildDrawing;
        }

        public string Name { get; private set; }

        public IReadOnlyList<DimensionParameter> Parameters { get; private set; }

        public Func<IReadOnlyDictionary<string, double>, DrawingModel> BuildDrawing { get; private set; }
    }
}
