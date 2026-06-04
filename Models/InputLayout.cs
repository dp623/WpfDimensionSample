using System.Collections.Generic;
using System.Linq;
using WpfDimensionSample.Infrastructure;

namespace WpfDimensionSample.Models
{
    public sealed class InputRow : ObservableObject
    {
        private bool _isChecked;

        public InputRow(string label, IReadOnlyList<InputSlot> slots)
            : this(label, slots, slots.Any(x => x.IsVisible), false, true)
        {
        }

        public InputRow(string label, IReadOnlyList<InputSlot> slots, bool isVisible)
            : this(label, slots, isVisible, false, true)
        {
        }

        public InputRow(
            string label,
            IReadOnlyList<InputSlot> slots,
            bool isVisible,
            bool isCheckBoxVisible,
            bool isChecked)
        {
            Label = label;
            Slots = slots;
            IsVisible = isVisible;
            IsCheckBoxVisible = isCheckBoxVisible;
            _isChecked = isChecked;

            foreach (var slot in Slots)
            {
                slot.SetOwner(this);
            }
        }

        public string Label { get; private set; }

        public IReadOnlyList<InputSlot> Slots { get; private set; }

        public bool IsVisible { get; private set; }

        public bool IsCheckBoxVisible { get; private set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (SetProperty(ref _isChecked, value))
                {
                    OnPropertyChanged(nameof(IsSlotsEnabled));
                    foreach (var slot in Slots)
                    {
                        slot.NotifyEnabledChanged();
                    }
                }
            }
        }

        public bool IsSlotsEnabled
        {
            get { return !IsCheckBoxVisible || IsChecked; }
        }
    }

    public sealed class InputSlot : ObservableObject
    {
        private bool _isChecked;
        private InputRow _owner;

        public InputSlot(string prefix, DimensionParameter parameter, string suffix)
            : this(prefix, parameter, suffix, false, true)
        {
        }

        public InputSlot(
            string prefix,
            DimensionParameter parameter,
            string suffix,
            bool isCheckBoxVisible,
            bool isChecked)
        {
            Prefix = prefix;
            Parameter = parameter;
            Suffix = suffix;
            IsCheckBoxVisible = isCheckBoxVisible;
            _isChecked = isChecked;
        }

        public string Prefix { get; private set; }

        public DimensionParameter Parameter { get; private set; }

        public string Suffix { get; private set; }

        public bool IsVisible { get { return Parameter.IsVisible; } }

        public bool IsCheckBoxVisible { get; private set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (SetProperty(ref _isChecked, value))
                {
                    OnPropertyChanged(nameof(IsTextBoxEnabled));
                }
            }
        }

        public bool IsTextBoxEnabled
        {
            get
            {
                return (!IsCheckBoxVisible || IsChecked)
                    && !Parameter.IsReadOnly
                    && (_owner == null || _owner.IsSlotsEnabled);
            }
        }

        internal void SetOwner(InputRow owner)
        {
            _owner = owner;
        }

        internal void NotifyEnabledChanged()
        {
            OnPropertyChanged(nameof(IsTextBoxEnabled));
        }
    }
}
