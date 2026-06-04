using System;
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
                CreateRectangleCircleRectangleTrapezoidPattern(),
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
            var parameters = new List<DimensionParameter>
            {
                new DimensionParameter("RectangleWidth", "四角形 幅", 180),
                new DimensionParameter("RectangleHeight", "四角形 高さ", 130),
                new DimensionParameter("CircleRadius", "真円 半径", 30),
                new DimensionParameter("CircleCenterX", "真円 中心X", 90),
                new DimensionParameter("CircleCenterY", "真円 中心Y", 65)
            };

            return new ShapePattern(
                "四角形 + 真円",
                parameters,
                new List<InputRow>
                {
                    new InputRow(
                        "四角形",
                        new List<InputSlot>
                        {
                            new InputSlot(string.Empty, parameters[0], "W x"),
                            new InputSlot(string.Empty, parameters[1], "H")
                        }),
                    new InputRow(
                        "真円　半径",
                        new List<InputSlot>
                        {
                            new InputSlot(string.Empty, parameters[2], string.Empty)
                        }),
                    new InputRow(
                        "真円　位置",
                        new List<InputSlot>
                        {
                            new InputSlot("X：", parameters[3], "x"),
                            new InputSlot("Y：", parameters[4], string.Empty)
                        },
                        true,
                        true,
                        true)
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
                                string.Format("真円 半径 R = {0:g} mm", radius), false, false),
                            new DimensionAnnotation(
                                new Point(0, centerY),
                                new Point(centerX, centerY),
                                new Vector(0, -1),
                                string.Format("中心X = {0:g} mm", centerX), false, false),
                            new DimensionAnnotation(
                                new Point(rectangleWidth, 0),
                                new Point(rectangleWidth, centerY),
                                new Vector(1, 0),
                                string.Format("中心Y = {0:g} mm", centerY), false, false)
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

        private static ShapePattern CreateRectangleCircleRectangleTrapezoidPattern()
        {
            var parameters = new List<DimensionParameter>
            {
                new DimensionParameter("TopRectangleWidth", "真円入り矩形 幅", 180),
                new DimensionParameter("TopRectangleHeight", "真円入り矩形 高さ", 130),
                new DimensionParameter("CircleRadius", "真円 半径", 30),
                new DimensionParameter("CircleCenterX", "真円 中心X", 90),
                new DimensionParameter("CircleCenterY", "真円 中心Y", 65),
                new DimensionParameter("MiddleRectangleWidth", "矩形 幅", 150),
                new DimensionParameter("MiddleRectangleHeight", "矩形 高さ", 80),
                new DimensionParameter("TrapezoidTopWidth", "台形 上底", 110),
                new DimensionParameter("TrapezoidBottomWidth", "台形 下底", 170),
                new DimensionParameter("TrapezoidHeight", "台形 高さ", 90),
                new DimensionParameter("Gap", "間隔", 25)
            };

            return new ShapePattern(
                "真円入り矩形 + 矩形 + 台形",
                parameters,
                new List<InputRow>
                {
                    new InputRow(
                        "真円入り矩形",
                        new List<InputSlot>
                        {
                            new InputSlot(string.Empty, parameters[0], "W x"),
                            new InputSlot(string.Empty, parameters[1], "H")
                        }),
                    new InputRow(
                        "真円　半径",
                        new List<InputSlot>
                        {
                            new InputSlot(string.Empty, parameters[2], string.Empty)
                        }),
                    new InputRow(
                        "真円　位置",
                        new List<InputSlot>
                        {
                            new InputSlot("X", parameters[3], "x"),
                            new InputSlot("Y", parameters[4], string.Empty)
                        },
                        true,
                        true,
                        true),
                    new InputRow(
                        "矩形",
                        new List<InputSlot>
                        {
                            new InputSlot(string.Empty, parameters[5], "W x"),
                            new InputSlot(string.Empty, parameters[6], "H")
                        }),
                    new InputRow(
                        "台形",
                        new List<InputSlot>
                        {
                            new InputSlot("上", parameters[7], "x"),
                            new InputSlot("下", parameters[8], "x"),
                            new InputSlot("H", parameters[9], string.Empty)
                        }),
                    new InputRow(
                        "間隔",
                        new List<InputSlot>
                        {
                            new InputSlot(string.Empty, parameters[10], string.Empty)
                        })
                },
                values =>
                {
                    var topRectangleWidth = values["TopRectangleWidth"];
                    var topRectangleHeight = values["TopRectangleHeight"];
                    var circleRadius = values["CircleRadius"];
                    var circleCenterX = values["CircleCenterX"];
                    var circleCenterY = values["CircleCenterY"];
                    var middleRectangleWidth = values["MiddleRectangleWidth"];
                    var middleRectangleHeight = values["MiddleRectangleHeight"];
                    var trapezoidTopWidth = values["TrapezoidTopWidth"];
                    var trapezoidBottomWidth = values["TrapezoidBottomWidth"];
                    var trapezoidHeight = values["TrapezoidHeight"];
                    var gap = values["Gap"];

                    var trapezoidWidth = Math.Max(trapezoidTopWidth, trapezoidBottomWidth);
                    var totalWidth = Math.Max(
                        topRectangleWidth,
                        Math.Max(middleRectangleWidth, trapezoidWidth));

                    var topRectangleLeft = (totalWidth - topRectangleWidth) / 2;
                    var topRectangleTop = 0.0;
                    var middleRectangleLeft = (totalWidth - middleRectangleWidth) / 2;
                    var middleRectangleTop = topRectangleHeight + gap;
                    var trapezoidLeft = (totalWidth - trapezoidWidth) / 2;
                    var trapezoidTop = middleRectangleTop + middleRectangleHeight + gap;
                    var trapezoidTopLeft = trapezoidLeft + (trapezoidWidth - trapezoidTopWidth) / 2;
                    var trapezoidBottomLeft = trapezoidLeft + (trapezoidWidth - trapezoidBottomWidth) / 2;
                    var bottom = trapezoidTop + trapezoidHeight;
                    var circleCenter = new Point(
                        topRectangleLeft + circleCenterX,
                        topRectangleTop + circleCenterY);

                    return new DrawingModel(
                        new List<IReadOnlyList<Point>>
                        {
                            ShapePoints.CreateRectangle(
                                topRectangleLeft,
                                topRectangleTop,
                                topRectangleWidth,
                                topRectangleHeight),
                            ShapePoints.CreateRectangle(
                                middleRectangleLeft,
                                middleRectangleTop,
                                middleRectangleWidth,
                                middleRectangleHeight),
                            new List<Point>
                            {
                                new Point(trapezoidTopLeft, trapezoidTop),
                                new Point(trapezoidTopLeft + trapezoidTopWidth, trapezoidTop),
                                new Point(trapezoidBottomLeft + trapezoidBottomWidth, bottom),
                                new Point(trapezoidBottomLeft, bottom)
                            }
                        },
                        new List<CircleShape>
                        {
                            new CircleShape(circleCenter, circleRadius)
                        },
                        new List<DimensionAnnotation>
                        {
                            new DimensionAnnotation(
                                new Point(topRectangleLeft, topRectangleHeight),
                                new Point(topRectangleLeft + topRectangleWidth, topRectangleHeight),
                                new Vector(0, 1),
                                string.Format("真円入り矩形 幅 = {0:g} mm", topRectangleWidth)),
                            new DimensionAnnotation(
                                new Point(topRectangleLeft + topRectangleWidth, topRectangleTop),
                                new Point(topRectangleLeft + topRectangleWidth, topRectangleHeight),
                                new Vector(1, 0),
                                string.Format("真円入り矩形 高さ = {0:g} mm", topRectangleHeight)),
                            new DimensionAnnotation(
                                circleCenter,
                                new Point(circleCenter.X + circleRadius, circleCenter.Y),
                                new Vector(0, 1),
                                string.Format("真円 半径 R = {0:g} mm", circleRadius)),
                            new DimensionAnnotation(
                                new Point(topRectangleLeft, circleCenter.Y),
                                circleCenter,
                                new Vector(0, -1),
                                string.Format("中心X = {0:g} mm", circleCenterX)),
                            new DimensionAnnotation(
                                new Point(topRectangleLeft + topRectangleWidth, topRectangleTop),
                                new Point(topRectangleLeft + topRectangleWidth, circleCenter.Y),
                                new Vector(1, 0),
                                string.Format("中心Y = {0:g} mm", circleCenterY)),
                            new DimensionAnnotation(
                                new Point(middleRectangleLeft, middleRectangleTop + middleRectangleHeight),
                                new Point(middleRectangleLeft + middleRectangleWidth, middleRectangleTop + middleRectangleHeight),
                                new Vector(0, 1),
                                string.Format("矩形 幅 = {0:g} mm", middleRectangleWidth)),
                            new DimensionAnnotation(
                                new Point(middleRectangleLeft + middleRectangleWidth, middleRectangleTop),
                                new Point(middleRectangleLeft + middleRectangleWidth, middleRectangleTop + middleRectangleHeight),
                                new Vector(1, 0),
                                string.Format("矩形 高さ = {0:g} mm", middleRectangleHeight)),
                            new DimensionAnnotation(
                                new Point(trapezoidTopLeft, trapezoidTop),
                                new Point(trapezoidTopLeft + trapezoidTopWidth, trapezoidTop),
                                new Vector(0, -1),
                                string.Format("台形 上底 = {0:g} mm", trapezoidTopWidth), false, false),
                            new DimensionAnnotation(
                                new Point(trapezoidBottomLeft, bottom),
                                new Point(trapezoidBottomLeft + trapezoidBottomWidth, bottom),
                                new Vector(0, 1),
                                string.Format("台形 下底 = {0:g} mm", trapezoidBottomWidth), false, false),
                            new DimensionAnnotation(
                                new Point(trapezoidLeft + trapezoidWidth, trapezoidTop),
                                new Point(trapezoidLeft + trapezoidWidth, bottom),
                                new Vector(1, 0),
                                string.Format("台形 高さ = {0:g} mm", trapezoidHeight)),
                            new DimensionAnnotation(
                                new Point(totalWidth, topRectangleHeight),
                                new Point(totalWidth, middleRectangleTop),
                                new Vector(1, 0),
                                string.Format("間隔 = {0:g} mm", gap)),
                            new DimensionAnnotation(
                                new Point(totalWidth, middleRectangleTop + middleRectangleHeight),
                                new Point(totalWidth, trapezoidTop),
                                new Vector(1, 0),
                                string.Format("間隔 = {0:g} mm", gap), false, false)
                        });
                });
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
