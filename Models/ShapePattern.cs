using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfDimensionSample.Models
{
    public sealed class ShapePattern
    {
        public ShapePattern(
            string name,
            IReadOnlyList<DimensionParameter> parameters,
            Func<IReadOnlyDictionary<string, double>, DrawingModel> buildDrawing)
            : this(name, parameters, CreateDefaultInputRows(parameters), null, buildDrawing)
        {
        }

        public ShapePattern(
            string name,
            IReadOnlyList<DimensionParameter> parameters,
            IReadOnlyList<InputRow> inputRows,
            Func<IReadOnlyDictionary<string, double>, DrawingModel> buildDrawing)
            : this(name, parameters, inputRows, null, buildDrawing)
        {
        }

        public ShapePattern(
            string name,
            IReadOnlyList<DimensionParameter> parameters,
            IReadOnlyList<InputRow> inputRows,
            Action<IReadOnlyDictionary<string, double>> updateComputedParameters,
            Func<IReadOnlyDictionary<string, double>, DrawingModel> buildDrawing)
        {
            Name = name;
            Parameters = parameters;
            InputRows = SortInputRows(inputRows);
            UpdateComputedParameters = updateComputedParameters;
            BuildDrawing = buildDrawing;
        }

        public string Name { get; private set; }

        public IReadOnlyList<DimensionParameter> Parameters { get; private set; }

        public IReadOnlyList<InputRow> InputRows { get; private set; }

        public Func<IReadOnlyDictionary<string, double>, DrawingModel> BuildDrawing { get; private set; }

        public Action<IReadOnlyDictionary<string, double>> UpdateComputedParameters { get; private set; }

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
                    parameter.IsVisible,
                    false,
                    true,
                    parameter.SortOrder));
            }

            return rows;
        }

        private static IReadOnlyList<InputRow> SortInputRows(IReadOnlyList<InputRow> inputRows)
        {
            return inputRows
                .Select((row, index) => new { Row = row, Index = index })
                .OrderBy(x => x.Row.SortOrder)
                .ThenBy(x => x.Index)
                .Select(x => x.Row)
                .ToList();
        }
    }
}
