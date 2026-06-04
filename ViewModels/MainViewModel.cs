using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WpfDimensionSample.Infrastructure;
using WpfDimensionSample.Models;
using WpfDimensionSample.Patterns;

namespace WpfDimensionSample.ViewModels
{
    public sealed class MainViewModel : ObservableObject
    {
        private ShapePattern _selectedPattern;
        private DrawingModel _drawing;
        private bool _isRebuilding;

        public MainViewModel()
        {
            Patterns = new ObservableCollection<ShapePattern>(ShapePatternCatalog.CreateAll());
            SelectedPattern = Patterns[0];
        }

        public ObservableCollection<ShapePattern> Patterns { get; private set; }

        public ShapePattern SelectedPattern
        {
            get { return _selectedPattern; }
            set
            {
                if (_selectedPattern == value)
                {
                    return;
                }

                Unsubscribe(_selectedPattern);
                _selectedPattern = value;
                Subscribe(_selectedPattern);
                OnPropertyChanged();
                RebuildDrawing();
            }
        }

        public DrawingModel Drawing
        {
            get { return _drawing; }
            private set { SetProperty(ref _drawing, value); }
        }

        private void Subscribe(ShapePattern pattern)
        {
            if (pattern == null)
            {
                return;
            }

            foreach (var parameter in pattern.Parameters)
            {
                parameter.PropertyChanged += ParameterOnPropertyChanged;
            }
        }

        private void Unsubscribe(ShapePattern pattern)
        {
            if (pattern == null)
            {
                return;
            }

            foreach (var parameter in pattern.Parameters)
            {
                parameter.PropertyChanged -= ParameterOnPropertyChanged;
            }
        }

        private void ParameterOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isRebuilding)
            {
                return;
            }

            if (e.PropertyName == nameof(DimensionParameter.NumericValue))
            {
                RebuildDrawing();
            }
        }

        private void RebuildDrawing()
        {
            if (SelectedPattern == null)
            {
                Drawing = null;
                return;
            }

            _isRebuilding = true;
            try
            {
                var values = SelectedPattern.Parameters.ToDictionary(x => x.Key, x => x.NumericValue);
                if (SelectedPattern.UpdateComputedParameters != null)
                {
                    SelectedPattern.UpdateComputedParameters(values);
                    values = SelectedPattern.Parameters.ToDictionary(x => x.Key, x => x.NumericValue);
                }

                Drawing = SelectedPattern.BuildDrawing(values);
            }
            finally
            {
                _isRebuilding = false;
            }
        }
    }
}
