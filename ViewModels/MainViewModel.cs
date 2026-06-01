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
                CreateTrapezoidPattern(),
                CreateDoubleRectanglePattern(),
                CreateTrapezoidAndRectanglePattern()
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

        private static ShapePattern CreateDoubleRectanglePattern()
        {
            return new ShapePattern(
                "四角形 x2",
                new List<DimensionParameter>
                {
                    new DimensionParameter("Width1", "幅1", 100),
                    new DimensionParameter("Height1", "高さ1", 90),
                    new DimensionParameter("Width2", "幅2", 130),
                    new DimensionParameter("Height2", "高さ2", 120),
                    new DimensionParameter("Gap", "間隔", 35)
                },
                values =>
                {
                    var width1 = values["Width1"];
                    var height1 = values["Height1"];
                    var width2 = values["Width2"];
                    var height2 = values["Height2"];
                    var gap = values["Gap"];
                    var totalHeight = Math.Max(height1, height2);
                    var firstTop = totalHeight - height1;
                    var secondLeft = width1 + gap;
                    var secondTop = totalHeight - height2;

                    return new DrawingModel(
                        new List<IReadOnlyList<Point>>
                        {
                            CreateRectanglePoints(0, firstTop, width1, height1),
                            CreateRectanglePoints(secondLeft, secondTop, width2, height2)
                        },
                        new List<DimensionAnnotation>
                        {
                            new DimensionAnnotation(
                                new Point(0, totalHeight),
                                new Point(width1, totalHeight),
                                new Vector(0, 1),
                                string.Format("幅1 W1 = {0:g} mm", width1)),
                            new DimensionAnnotation(
                                new Point(secondLeft, totalHeight),
                                new Point(secondLeft + width2, totalHeight),
                                new Vector(0, 1),
                                string.Format("幅2 W2 = {0:g} mm", width2)),
                            new DimensionAnnotation(
                                new Point(0, firstTop),
                                new Point(0, totalHeight),
                                new Vector(-1, 0),
                                string.Format("高さ1 H1 = {0:g} mm", height1)),
                            new DimensionAnnotation(
                                new Point(secondLeft + width2, secondTop),
                                new Point(secondLeft + width2, totalHeight),
                                new Vector(1, 0),
                                string.Format("高さ2 H2 = {0:g} mm", height2)),
                            new DimensionAnnotation(
                                new Point(width1, totalHeight),
                                new Point(secondLeft, totalHeight),
                                new Vector(0, -1),
                                string.Format("間隔 G = {0:g} mm", gap))
                        });
                });
        }

        private static ShapePattern CreateTrapezoidAndRectanglePattern()
        {
            return new ShapePattern(
                "台形 + 四角形",
                new List<DimensionParameter>
                {
                    new DimensionParameter("TopWidth", "上底", 90),
                    new DimensionParameter("BottomWidth", "下底", 160),
                    new DimensionParameter("TrapezoidHeight", "台形高さ", 120),
                    new DimensionParameter("RectangleWidth", "四角形幅", 100),
                    new DimensionParameter("RectangleHeight", "四角形高さ", 85),
                    new DimensionParameter("Gap", "間隔", 35)
                },
                values =>
                {
                    var topWidth = values["TopWidth"];
                    var bottomWidth = values["BottomWidth"];
                    var trapezoidHeight = values["TrapezoidHeight"];
                    var rectangleWidth = values["RectangleWidth"];
                    var rectangleHeight = values["RectangleHeight"];
                    var gap = values["Gap"];
                    var totalHeight = Math.Max(trapezoidHeight, rectangleHeight);
                    var trapezoidTop = totalHeight - trapezoidHeight;
                    var rectangleTop = totalHeight - rectangleHeight;
                    var topLeft = (bottomWidth - topWidth) / 2;
                    var topRight = topLeft + topWidth;
                    var rectangleLeft = bottomWidth + gap;

                    return new DrawingModel(
                        new List<IReadOnlyList<Point>>
                        {
                            new List<Point>
                            {
                                new Point(topLeft, trapezoidTop),
                                new Point(topRight, trapezoidTop),
                                new Point(bottomWidth, totalHeight),
                                new Point(0, totalHeight)
                            },
                            CreateRectanglePoints(
                                rectangleLeft,
                                rectangleTop,
                                rectangleWidth,
                                rectangleHeight)
                        },
                        new List<DimensionAnnotation>
                        {
                            new DimensionAnnotation(
                                new Point(topLeft, trapezoidTop),
                                new Point(topRight, trapezoidTop),
                                new Vector(0, -1),
                                string.Format("上底 W1 = {0:g} mm", topWidth)),
                            new DimensionAnnotation(
                                new Point(0, totalHeight),
                                new Point(bottomWidth, totalHeight),
                                new Vector(0, 1),
                                string.Format("下底 W2 = {0:g} mm", bottomWidth)),
                            new DimensionAnnotation(
                                new Point(0, trapezoidTop),
                                new Point(0, totalHeight),
                                new Vector(-1, 0),
                                string.Format("台形高さ H1 = {0:g} mm", trapezoidHeight)),
                            new DimensionAnnotation(
                                new Point(rectangleLeft, totalHeight),
                                new Point(rectangleLeft + rectangleWidth, totalHeight),
                                new Vector(0, 1),
                                string.Format("四角形幅 W3 = {0:g} mm", rectangleWidth)),
                            new DimensionAnnotation(
                                new Point(rectangleLeft + rectangleWidth, rectangleTop),
                                new Point(rectangleLeft + rectangleWidth, totalHeight),
                                new Vector(1, 0),
                                string.Format("四角形高さ H2 = {0:g} mm", rectangleHeight)),
                            new DimensionAnnotation(
                                new Point(bottomWidth, totalHeight),
                                new Point(rectangleLeft, totalHeight),
                                new Vector(0, -1),
                                string.Format("間隔 G = {0:g} mm", gap))
                        });
                });
        }

        private static IReadOnlyList<Point> CreateRectanglePoints(
            double left,
            double top,
            double width,
            double height)
        {
            return new List<Point>
            {
                new Point(left, top),
                new Point(left + width, top),
                new Point(left + width, top + height),
                new Point(left, top + height)
            };
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
