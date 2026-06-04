using System.Collections.Generic;
using System.Linq;

namespace WpfDimensionSample.Models
{
    public sealed class InputRow
    {
        public InputRow(string label, IReadOnlyList<InputSlot> slots)
            : this(label, slots, slots.Any(x => x.IsVisible))
        {
        }

        public InputRow(string label, IReadOnlyList<InputSlot> slots, bool isVisible)
        {
            Label = label;
            Slots = slots;
            IsVisible = isVisible;
        }

        public string Label { get; private set; }

        public IReadOnlyList<InputSlot> Slots { get; private set; }

        public bool IsVisible { get; private set; }
    }

    public sealed class InputSlot
    {
        public InputSlot(string prefix, DimensionParameter parameter, string suffix)
        {
            Prefix = prefix;
            Parameter = parameter;
            Suffix = suffix;
        }

        public string Prefix { get; private set; }

        public DimensionParameter Parameter { get; private set; }

        public string Suffix { get; private set; }

        public bool IsVisible { get { return Parameter.IsVisible; } }
    }
}
