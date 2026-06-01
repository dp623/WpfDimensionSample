using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using WpfDimensionSample.Infrastructure;

namespace WpfDimensionSample.Models
{
    public sealed class DimensionParameter : ObservableObject, INotifyDataErrorInfo
    {
        private readonly List<string> _errors = new List<string>();
        private string _valueText;
        private double _numericValue;

        public DimensionParameter(string key, string label, double initialValue, string unit = "mm")
        {
            Key = key;
            Label = label;
            Unit = unit;
            _numericValue = initialValue;
            _valueText = initialValue.ToString(CultureInfo.CurrentCulture);
        }

        public string Key { get; private set; }

        public string Label { get; private set; }

        public string Unit { get; private set; }

        public double NumericValue
        {
            get { return _numericValue; }
            private set { SetProperty(ref _numericValue, value); }
        }

        public string ValueText
        {
            get { return _valueText; }
            set
            {
                if (!SetProperty(ref _valueText, value))
                {
                    return;
                }

                ValidateAndUpdate(value);
            }
        }

        public bool HasErrors { get { return _errors.Count > 0; } }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == null || propertyName == nameof(ValueText))
            {
                return _errors;
            }

            return new string[0];
        }

        private void ValidateAndUpdate(string text)
        {
            _errors.Clear();

            double value;
            var parsed = double.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.CurrentCulture,
                out value);

            if (!parsed || double.IsNaN(value) || double.IsInfinity(value))
            {
                _errors.Add("数値を入力してください。");
            }
            else if (value <= 0)
            {
                _errors.Add("0より大きい値を入力してください。");
            }
            else
            {
                NumericValue = value;
            }

            var handler = ErrorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(nameof(ValueText)));
            }
            OnPropertyChanged(nameof(HasErrors));
        }
    }
}
