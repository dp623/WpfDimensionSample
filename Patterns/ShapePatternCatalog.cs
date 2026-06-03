using System.Collections.Generic;
using System.Windows;
using WpfDimensionSample.Models;

namespace WpfDimensionSample.Patterns
{
    public static class ShapePatternCatalog
    {
        public static IReadOnlyList<ShapePattern> CreateAll()
        {
            return new List<ShapePattern>
            {
                CreateRectanglePattern(),
                CreateSteppedPattern(),
                CreateTrapezoidPattern(),
                CreateRectangleWithCirclePattern(),
                CreateHeightOnlyRectanglePattern(),
                CreateDoubleRectanglePattern(),
                CreateTrapezoidAndRectanglePattern(),
                CreateFourRectanglesAndTwoTrapezoidsPattern()
            };
        }

        private static ShapePattern CreateRectanglePattern()
        {
            return new VerticalCompositePatternBuilder("長方形", 0)
                .AddRectangle("R1", "長方形", 160, 100)
                .Build();
        }

        private static ShapePattern CreateTrapezoidPattern()
        {
            return new VerticalCompositePatternBuilder("台形", 0)
                .AddTrapezoid("T1", "台形", 100, 180, 110)
                .Build();
        }

        private static ShapePattern CreateRectangleWithCirclePattern()
        {
            return new ShapePattern(
                "四角形 + 真円",
                new List<DimensionParameter>
                {
                    new DimensionParameter("RectangleWidth", "四角形 幅", 180),
                    new DimensionParameter("RectangleHeight", "四角形 高さ", 130),
                    new DimensionParameter("CircleRadius", "真円 半径", 30),
                    new DimensionParameter("CircleCenterX", "真円 中心X", 90),
                    new DimensionParameter("CircleCenterY", "真円 中心Y", 65)
                },
                values =>
                {
                    var rectangleWidth = values["RectangleWidth"];
                    var rectangleHeight = values["RectangleHeight"];
                    var radius = values["CircleRadius"];
                    var centerX = values["CircleCenterX"];
                    var centerY = values["CircleCenterY"];
                    var center = new Point(centerX, centerY);

                    return new DrawingModel(
                        new List<IReadOnlyList<Point>>
                        {
                            ShapePoints.CreateRectangle(0, 0, rectangleWidth, rectangleHeight)
                        },
                        new List<CircleShape>
                        {
                            new CircleShape(center, radius)
                        },
                        new List<DimensionAnnotation>
                        {
                            new DimensionAnnotation(
                                new Point(0, rectangleHeight),
                                new Point(rectangleWidth, rectangleHeight),
                                new Vector(0, 1),
                                string.Format("四角形 幅 = {0:g} mm", rectangleWidth)),
                            new DimensionAnnotation(
                                new Point(rectangleWidth, 0),
                                new Point(rectangleWidth, rectangleHeight),
                                new Vector(1, 0),
                                string.Format("四角形 高さ = {0:g} mm", rectangleHeight)),
                            new DimensionAnnotation(
                                new Point(centerX, centerY),
                                new Point(centerX + radius, centerY),
                                new Vector(0, 1),
                                string.Format("真円 半径 R = {0:g} mm", radius)),
                            new DimensionAnnotation(
                                new Point(0, centerY),
                                new Point(centerX, centerY),
                                new Vector(0, -1),
                                string.Format("中心X = {0:g} mm", centerX)),
                            new DimensionAnnotation(
                                new Point(rectangleWidth, 0),
                                new Point(rectangleWidth, centerY),
                                new Vector(1, 0),
                                string.Format("中心Y = {0:g} mm", centerY))
                        });
                });
        }

        private static ShapePattern CreateDoubleRectanglePattern()
        {
            return new VerticalCompositePatternBuilder("四角形 x2", 35)
                .AddRectangle("R1", "四角形1", 100, 90)
                .AddRectangle("R2", "四角形2", 130, 120)
                .Build();
        }

        private static ShapePattern CreateHeightOnlyRectanglePattern()
        {
            return new VerticalCompositePatternBuilder("四角形（高さのみ変更）", 0)
                .AddRectangle(
                    "R1",
                    "四角形",
                    160,
                    100,
                    DimensionDisplay.Hidden(),
                    DimensionDisplay.Visible())
                .Build();
        }

        private static ShapePattern CreateTrapezoidAndRectanglePattern()
        {
            return new VerticalCompositePatternBuilder("台形 + 四角形", 35)
                .AddTrapezoid("T1", "台形", 90, 160, 120)
                .AddRectangle("R1", "四角形", 100, 85)
                .Build();
        }

        private static ShapePattern CreateFourRectanglesAndTwoTrapezoidsPattern()
        {
            return new VerticalCompositePatternBuilder("四角形 x4 + 台形 x2", 25)
                .AddRectangle("R1", "四角形1", 80, 70)
                .AddRectangle("R2", "四角形2", 90, 85)
                .AddRectangle("R3", "四角形3", 100, 95)
                .AddRectangle("R4", "四角形4", 110, 105)
                .AddTrapezoid("T1", "台形1", 70, 120, 100)
                .AddTrapezoid("T2", "台形2", 85, 135, 115)
                .Build();
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
                                new Point(width, 0),
                                new Point(width, height),
                                new Vector(1, 0),
                                string.Format("高さ H = {0:g} mm", height)),
                            new DimensionAnnotation(
                                new Point(width, height - height2),
                                new Point(width, height),
                                new Vector(1, 0),
                                string.Format("高さ2 H2 = {0:g} mm", height2))
                        });
                });
        }
    }
}
