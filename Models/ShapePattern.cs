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
            : this(name, parameters, CreateDefaultInputRows(parameters), buildDrawing)
        {
        }

        public ShapePattern(
            string name,
            IReadOnlyList<DimensionParameter> parameters,
            IReadOnlyList<InputRow> inputRows,
            Func<IReadOnlyDictionary<string, double>, DrawingModel> buildDrawing)
        {
            Name = name;
            Parameters = parameters;
            InputRows = inputRows;
            BuildDrawing = buildDrawing;
        }

        public string Name { get; private set; }

        public IReadOnlyList<DimensionParameter> Parameters { get; private set; }

        public IReadOnlyList<InputRow> InputRows { get; private set; }

        public Func<IReadOnlyDictionary<string, double>, DrawingModel> BuildDrawing { get; private set; }

        private static IReadOnlyList<InputRow> CreateDefaultInputRows(
            IReadOnlyList<DimensionParameter> parameters)
        {
            var rows = new List<InputRow>();
            foreach (var parameter in parameters)
            {
                rows.Add(new InputRow(
                    parameter.Label,
                    new List<InputSlot>
                    {
                        new InputSlot(string.Empty, parameter, parameter.Unit)
                    },
                    parameter.IsVisible));
            }

            return rows;
        }
    }
}
