using System.Collections.Generic;
using System.Linq;
using WpfDimensionSample.Infrastructure;

namespace WpfDimensionSample.Models
{
    // 左側パネルの1行分の入力レイアウトです。
    // 複数のInputSlotを持てるため、"幅 [ ] W x [ ] H"のような行を作れます。
    public sealed class InputRow : ObservableObject
    {
        private bool _isChecked;

        public InputRow(string label, IReadOnlyList<InputSlot> slots)
            : this(label, slots, slots.Any(x => x.IsVisible), false, true, 0)
        {
        }

        public InputRow(string label, IReadOnlyList<InputSlot> slots, int sortOrder)
            : this(label, slots, slots.Any(x => x.IsVisible), false, true, sortOrder)
        {
        }

        public InputRow(string label, IReadOnlyList<InputSlot> slots, bool isVisible)
            : this(label, slots, isVisible, false, true, 0)
        {
        }

        public InputRow(
            string label,
            IReadOnlyList<InputSlot> slots,
            bool isVisible,
            bool isCheckBoxVisible,
            bool isChecked)
            : this(label, slots, isVisible, isCheckBoxVisible, isChecked, 0)
        {
        }

        public InputRow(
            string label,
            IReadOnlyList<InputSlot> slots,
            bool isVisible,
            bool isCheckBoxVisible,
            bool isChecked,
            int sortOrder)
        {
            Label = label;
            Slots = slots;
            IsVisible = isVisible;
            IsCheckBoxVisible = isCheckBoxVisible;
            SortOrder = sortOrder;
            _isChecked = isChecked;

            // 行単位のチェックボックスで、行内の複数TextBoxをまとめて有効/無効にします。
            foreach (var slot in Slots)
            {
                slot.SetOwner(this);
            }
        }

        public string Label { get; private set; }

        public IReadOnlyList<InputSlot> Slots { get; private set; }

        public bool IsVisible { get; private set; }

        public bool IsCheckBoxVisible { get; private set; }

        public int SortOrder { get; private set; }

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
                // スロット自身、行チェック、表示専用フラグの3条件で編集可否を決めます。
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
