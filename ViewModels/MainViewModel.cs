using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using WpfDimensionSample.Infrastructure;
using WpfDimensionSample.Models;

namespace WpfDimensionSample.ViewModels
{
    public sealed class MainViewModel : ObservableObject
    {
        private ShapePattern _selectedPattern;
        private DrawingModel _drawing;

        public MainViewModel()
        {
            Patterns = new ObservableCollection<ShapePattern>
            {
                CreateRectanglePattern(),
                CreateSteppedPattern(),
                CreateTrapezoidPattern()
            };

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

        private static ShapePattern CreateRectanglePattern()
        {
            return new ShapePattern(
                "長方形",
                new List<DimensionParameter>
                {
                    new DimensionParameter("Width", "幅", 160),
                    new DimensionParameter("Height", "高さ", 100)
                },
                values =>
                {
                    var width = values["Width"];
                    var height = values["Height"];

                    return new DrawingModel(
                        new List<Point>
                        {
                            new Point(0, 0),
                            new Point(width, 0),
                            new Point(width, height),
                            new Point(0, height)
                        },
                        new List<DimensionAnnotation>
                        {
                            new DimensionAnnotation(
                                new Point(0, height),
                                new Point(width, height),
                                new Vector(0, 1),
                                string.Format("幅 W = {0:g} mm", width)),
                            new DimensionAnnotation(
                                new Point(0, 0),
                                new Point(0, height),
                                new Vector(-1, 0),
                                string.Format("高さ H = {0:g} mm", height))
                        });
                });
        }

        private static ShapePattern CreateSteppedPattern()
        {
            return new ShapePattern(
                "段付き形状",
                new List<DimensionParameter>
                {
                    new DimensionParameter("Width", "幅", 180),
                    new DimensionParameter("Height", "高さ", 140),
                    new DimensionParameter("Height2", "高さ2", 75)
                },
                values =>
                {
                    var width = values["Width"];
                    var height = values["Height"];
                    var height2 = values["Height2"];
                    var stepX = width * 0.55;

                    return new DrawingModel(
                        new List<Point>
                        {
                            new Point(0, 0),
                            new Point(stepX, 0),
                            new Point(stepX, height - height2),
                            new Point(width, height - height2),
                            new Point(width, height),
                            new Point(0, height)
                        },
                        new List<DimensionAnnotation>
                        {
                            new DimensionAnnotation(
                                new Point(0, height),
                                new Point(width, height),
                                new Vector(0, 1),
                                string.Format("幅 W = {0:g} mm", width)),
                            new DimensionAnnotation(
                                new Point(0, 0),
                                new Point(0, height),
                                new Vector(-1, 0),
                                string.Format("高さ H = {0:g} mm", height)),
                            new DimensionAnnotation(
                                new Point(width, height - height2),
                                new Point(width, height),
                                new Vector(1, 0),
                                string.Format("高さ2 H2 = {0:g} mm", height2))
                        });
                });
        }

        private static ShapePattern CreateTrapezoidPattern()
        {
            return new ShapePattern(
                "台形",
                new List<DimensionParameter>
                {
                    new DimensionParameter("TopWidth", "上底", 100),
                    new DimensionParameter("BottomWidth", "下底", 180),
                    new DimensionParameter("Height", "高さ", 110)
                },
                values =>
                {
                    var topWidth = values["TopWidth"];
                    var bottomWidth = values["BottomWidth"];
                    var height = values["Height"];
                    var left = Math.Min(0, (bottomWidth - topWidth) / 2);
                    var topLeft = (bottomWidth - topWidth) / 2;
                    var topRight = topLeft + topWidth;

                    return new DrawingModel(
                        new List<Point>
                        {
                            new Point(topLeft, 0),
                            new Point(topRight, 0),
                            new Point(bottomWidth, height),
                            new Point(0, height)
                        },
                        new List<DimensionAnnotation>
                        {
                            new DimensionAnnotation(
                                new Point(topLeft, 0),
                                new Point(topRight, 0),
                                new Vector(0, -1),
                                string.Format("上底 W1 = {0:g} mm", topWidth)),
                            new DimensionAnnotation(
                                new Point(0, height),
                                new Point(bottomWidth, height),
                                new Vector(0, 1),
                                string.Format("下底 W2 = {0:g} mm", bottomWidth)),
                            new DimensionAnnotation(
                                new Point(left, 0),
                                new Point(left, height),
                                new Vector(-1, 0),
                                string.Format("高さ H = {0:g} mm", height))
                        });
                });
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

            var values = SelectedPattern.Parameters.ToDictionary(x => x.Key, x => x.NumericValue);
            Drawing = SelectedPattern.BuildDrawing(values);
        }
    }
}
