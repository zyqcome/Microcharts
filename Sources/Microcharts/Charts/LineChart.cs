// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using SkiaSharp;

namespace Microcharts
{
    /// <summary>
    /// ![chart](../images/Line.png)
    ///
    /// Line chart.
    /// </summary>
    public class LineChart : PointChart
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microcharts.LineChart"/> class.
        /// </summary>
        public LineChart()
        {
            this.PointSize = 10;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the size of the line.
        /// </summary>
        /// <value>The size of the line.</value>
        public float LineSize { get; set; } = 3;

        /// <summary>
        /// Gets or sets the line mode.
        /// </summary>
        /// <value>The line mode.</value>
        public LineMode LineMode { get; set; } = LineMode.Spline;

        /// <summary>
        /// Gets or sets the alpha of the line area.
        /// </summary>
        /// <value>The line area alpha.</value>
        public byte LineAreaAlpha { get; set; } = 32;

        /// <summary>
        /// Enables or disables a fade out gradient for the line area in the Y direction
        /// </summary>
        /// <value>The state of the fadeout gradient.</value>
        public bool EnableYFadeOutGradient { get; set; } = false;

        #endregion

        #region Methods

        /// <summary>
        /// 绘制内容
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void DrawContent(SKCanvas canvas, int width, int height)
        {
            if (this.Entries != null)
            {
                var labels = this.Entries.Select(x => x.Label).ToArray();
                var labelSizes = this.MeasureLabels(labels);
                var footerHeight = this.CalculateFooterHeaderHeight(labelSizes, this.LabelOrientation);

                var valueLabels = this.Entries.Select(x => x.ValueLabel).ToArray();
                var valueLabelSizes = this.MeasureLabels(valueLabels);
                var headerHeight = this.CalculateFooterHeaderHeight(valueLabelSizes, this.ValueLabelOrientation);

                var itemSize = this.CalculateItemSize(width, height, footerHeight, headerHeight);
                var origin = this.CalculateYOrigin(itemSize.Height, headerHeight);
                var points = this.CalculatePoints(itemSize, origin, headerHeight);

                //画阴影
                this.DrawArea(canvas, points, itemSize, origin);
                //画线
                this.DrawLine(canvas, points, itemSize);
                //画x轴
                this.DrawXaxis(canvas, points, itemSize, origin);

                //画y轴
                this.DrawYaxis(canvas, points, itemSize, origin);

                //画点
                this.DrawPoints(canvas, points);
                //画上
                this.DrawHeader(canvas, valueLabels, valueLabelSizes, points, itemSize, height, headerHeight);
                //画脚
                this.DrawFooter(canvas, labels, labelSizes, points, itemSize, height, footerHeight);
            }
        }

        protected void DrawXaxis(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin)
        {
            if (points.Length > 1 && this.LineMode != LineMode.None)
            {
                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.White,
                    StrokeWidth = this.LineSize,
                    IsAntialias = true,
                })
                {
                    using (var shader = this.CreateXGradient(points))
                    {
                        paint.Shader = shader;

                        var path = new SKPath();

                        //path.MoveTo(points.First());
                        path.MoveTo(points.First().X, origin);

                        var last = (this.LineMode == LineMode.Spline) ? points.Length - 1 : points.Length - 1;
                        SKPoint sKPoint = new SKPoint()
                        {
                            X = points[last].X,
                            Y = origin
                        };
                        path.LineTo(sKPoint);

                        canvas.DrawPath(path, paint);

                        SKPoint[] sKPaints = new SKPoint[]
                        {
                            new SKPoint() { X = sKPoint.X , Y = sKPoint.Y - 12},
                            new SKPoint() { X = sKPoint.X , Y = sKPoint.Y + 12},
                            new SKPoint() { X = sKPoint.X + 30 , Y = sKPoint.Y},
                        };
                        //this.DrawPolygon(canvas, sKPaints);

                        var pathP = new SKPath();

                        pathP.MoveTo(sKPaints.First());                        

                        for (int i = 1; i < 3; i++)
                        {
                            pathP.LineTo(sKPaints[i]);
                        }

                        pathP.Close();
                        paint.Style = SKPaintStyle.Fill;
                        canvas.DrawPath(pathP, paint);
                    }
                }
            }
        }

        protected void DrawYaxis(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin)
        {
            if (points.Length > 1 && this.LineMode != LineMode.None)
            {
                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.White,
                    StrokeWidth = this.LineSize,
                    IsAntialias = true,
                })
                {
                    using (var shader = this.CreateXGradient(points))
                    {
                        paint.Shader = shader;

                        var path = new SKPath();
                        
                        var last = (this.LineMode == LineMode.Spline) ? points.Length - 1 : points.Length - 1;

                        float yaxis_start = origin;
                        float yaxis_end = 0;

                        foreach (var item in points)
                        {
                            if (yaxis_start > item.Y)
                            {
                                yaxis_start = item.Y;
                            }

                            if (yaxis_end < item.Y)
                            {
                                yaxis_end = item.Y;
                            }
                        }

                        //注意这里是反的
                        SKPoint sKPoint = new SKPoint()
                        {
                            X = points.First().X,
                            Y = origin,//yaxis_end,
                        };

                        path.MoveTo(points.First().X, yaxis_start); //
                        path.LineTo(sKPoint);

                        canvas.DrawPath(path, paint);

                        SKPoint[] sKPaints = new SKPoint[]
                        {
                            new SKPoint() { X = points.First().X - 12, Y = yaxis_start },
                            new SKPoint() { X = points.First().X + 12, Y = yaxis_start },
                            new SKPoint() { X = points.First().X  , Y = yaxis_start - 30},
                        };
                        //this.DrawPolygon(canvas, sKPaints);

                        var pathP = new SKPath();

                        pathP.MoveTo(sKPaints.First());

                        for (int i = 1; i < 3; i++)
                        {
                            pathP.LineTo(sKPaints[i]);
                        }

                        pathP.Close();
                        paint.Style = SKPaintStyle.Fill;
                        canvas.DrawPath(pathP, paint);
                    }
                }
            }
        }

        protected void DrawLine(SKCanvas canvas, SKPoint[] points, SKSize itemSize)
        {
            if (points.Length > 1 && this.LineMode != LineMode.None)
            {
                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.White,
                    StrokeWidth = this.LineSize,
                    IsAntialias = true,
                })
                {
                    using (var shader = this.CreateXGradient(points))
                    {
                        paint.Shader = shader;

                        var path = new SKPath();

                        path.MoveTo(points.First());

                        var last = (this.LineMode == LineMode.Spline) ? points.Length - 1 : points.Length;
                        for (int i = 0; i < last; i++)
                        {
                            if (this.LineMode == LineMode.Spline)
                            {
                                var entry = this.Entries.ElementAt(i);
                                var nextEntry = this.Entries.ElementAt(i + 1);
                                var cubicInfo = this.CalculateCubicInfo(points, i, itemSize);
                                path.CubicTo(cubicInfo.control, cubicInfo.nextControl, cubicInfo.nextPoint);
                            }
                            else if (this.LineMode == LineMode.Straight)
                            {
                                path.LineTo(points[i]);
                            }
                        }

                        canvas.DrawPath(path, paint);
                    }
                }
            }
        }

        /// <summary>
        /// 按点画多边行
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="points"></param>
        /// <param name="origin"></param>
        protected void DrawPolygon(SKCanvas canvas, SKPoint[] points)
        {
            if (this.LineAreaAlpha > 0 && points.Length > 1)
            {
                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    //Color = SKColors.White,
                    //IsAntialias = true,
                    //Style = SKPaintStyle.Stroke,
                    Color = SKColors.White,
                    StrokeWidth = this.LineSize,
                    IsAntialias = true,
                })
                {
                    using (var shaderX = this.CreateXGradient(points, (byte)(this.LineAreaAlpha * this.AnimationProgress)))
                    using (var shaderY = this.CreateYGradient(points, (byte)(this.LineAreaAlpha * this.AnimationProgress)))
                    {
                        paint.Shader = EnableYFadeOutGradient ? SKShader.CreateCompose(shaderY, shaderX, SKBlendMode.SrcOut) : shaderX;

                        var path = new SKPath();

                        path.MoveTo(points.First());

                        var last = points.Length;

                        for (int i = 1; i < last; i++)
                        {
                            path.LineTo(points[i]);
                        }

                        path.Close();
                        canvas.DrawPath(path, paint);
                    }
                }
            }
        }

        protected void DrawArea(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin)
        {
            if (this.LineAreaAlpha > 0 && points.Length > 1)
            {
                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.White,
                    IsAntialias = true,
                })
                {
                    using (var shaderX = this.CreateXGradient(points, (byte)(this.LineAreaAlpha * this.AnimationProgress)))
                    using (var shaderY = this.CreateYGradient(points, (byte)(this.LineAreaAlpha * this.AnimationProgress)))
                    {
                        paint.Shader = EnableYFadeOutGradient ? SKShader.CreateCompose(shaderY, shaderX, SKBlendMode.SrcOut) : shaderX;

                        var path = new SKPath();

                        path.MoveTo(points.First().X, origin);
                        path.LineTo(points.First());

                        var last = (this.LineMode == LineMode.Spline) ? points.Length - 1 : points.Length;
                        for (int i = 0; i < last; i++)
                        {
                            if (this.LineMode == LineMode.Spline)
                            {
                                var entry = this.Entries.ElementAt(i);
                                var nextEntry = this.Entries.ElementAt(i + 1);
                                var cubicInfo = this.CalculateCubicInfo(points, i, itemSize);
                                path.CubicTo(cubicInfo.control, cubicInfo.nextControl, cubicInfo.nextPoint);
                            }
                            else if (this.LineMode == LineMode.Straight)
                            {
                                path.LineTo(points[i]);
                            }
                        }

                        path.LineTo(points.Last().X, origin);

                        path.Close();

                        canvas.DrawPath(path, paint);
                    }
                }
            }
        }

        private (SKPoint point, SKPoint control, SKPoint nextPoint, SKPoint nextControl) CalculateCubicInfo(SKPoint[] points, int i, SKSize itemSize)
        {
            var point = points[i];
            var nextPoint = points[i + 1];
            var controlOffset = new SKPoint(itemSize.Width * 0.8f, 0);
            var currentControl = point + controlOffset;
            var nextControl = nextPoint - controlOffset;
            return (point, currentControl, nextPoint, nextControl);
        }

        private SKShader CreateXGradient(SKPoint[] points, byte alpha = 255)
        {
            var startX = points.First().X;
            var endX = points.Last().X;
            var rangeX = endX - startX;

            return SKShader.CreateLinearGradient(
                new SKPoint(startX, 0),
                new SKPoint(endX, 0),
                this.Entries.Select(x => x.Color.WithAlpha(alpha)).ToArray(),
                null,
                SKShaderTileMode.Clamp);
        }

        private SKShader CreateYGradient(SKPoint[] points, byte alpha = 255)
        {
            var startY = points.Max(i => i.Y);
            var endY = 0;

            return SKShader.CreateLinearGradient(
                new SKPoint(0, startY),
                new SKPoint(0, endY),
                new SKColor[] {SKColors.White.WithAlpha(alpha), SKColors.White.WithAlpha(0)},
                null,
                SKShaderTileMode.Clamp);
        }

        #endregion
    }
}
