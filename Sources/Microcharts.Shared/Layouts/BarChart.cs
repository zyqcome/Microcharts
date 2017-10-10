// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microcharts
{
    using System;
    using System.Linq;

    using SkiaSharp;
    using Xam.Animations;

    /// <summary>
    /// ![chart](../images/Bar.png)
    /// 
    /// A bar chart.
    /// </summary>
    public class BarChart : PointChart
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microcharts.BarChart"/> class.
        /// </summary>
        public BarChart()
        {
            this.PointSize = 0;
        }

        #endregion

        #region Fields

        private byte barAreaAlpha = 32;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the bar background area alpha.
        /// </summary>
        /// <value>The bar area alpha.</value>
        public byte BarAreaAlpha
        {
            get => this.barAreaAlpha;
            set => this.Set(ref this.barAreaAlpha, value);
        }

        #endregion

        #region Methods

        /*protected override void OnEntriesChanged(System.Collections.Generic.IEnumerable<Entry> newEntries)
        {
            base.OnEntriesChanged(newEntries);

            this.Layers.ElementAt(1).EnterAnimation = new TransformAnimation(
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(2),
                new Transform(scaleY: 0, opacity: 0, translateX: 0),
                new Transform(scaleY: 1, opacity: 1, translateY: 0),
                Curve.EaseOut);
        }*/

        #region Layers

        protected override void DrawBackground(SKCanvas canvas, int width, int height)
        {
            var valueLabelSizes = MeasureValueLabels();
            var footerHeight = CalculateFooterHeight(valueLabelSizes);
            var headerHeight = CalculateHeaderHeight(valueLabelSizes);
            var itemSize = CalculateItemSize(width, height, footerHeight, headerHeight);
            var origin = CalculateYOrigin(itemSize.Height, headerHeight);
            var points = this.CalculatePoints(itemSize, origin, headerHeight);
            
            this.DrawFooter(canvas, points, itemSize, height, footerHeight);
            this.DrawBarAreas(canvas, points, origin, itemSize, headerHeight);
        }

        protected override void DrawForeground(SKCanvas canvas, int width, int height)
        {
            var valueLabelSizes = MeasureValueLabels();
            var footerHeight = CalculateFooterHeight(valueLabelSizes);
            var headerHeight = CalculateHeaderHeight(valueLabelSizes);
            var itemSize = CalculateItemSize(width, height, footerHeight, headerHeight);
            var origin = CalculateYOrigin(itemSize.Height, headerHeight);
            var points = this.CalculatePoints(itemSize, origin, headerHeight);

            this.DrawBars(canvas, points, itemSize, origin, headerHeight);
            this.DrawPoints(canvas, points);
        }

        protected override void DrawCaption(SKCanvas canvas, int width, int height)
        {
            var valueLabelSizes = MeasureValueLabels();
            var footerHeight = CalculateFooterHeight(valueLabelSizes);
            var headerHeight = CalculateHeaderHeight(valueLabelSizes);
            var itemSize = CalculateItemSize(width, height, footerHeight, headerHeight);
            var origin = CalculateYOrigin(itemSize.Height, headerHeight);
            var points = this.CalculatePoints(itemSize, origin, headerHeight);
            
            this.DrawValueLabel(canvas, points, itemSize, height, valueLabelSizes);
        }

        #endregion

        /// <summary>
        /// Draws the value bars.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="points">The points.</param>
        /// <param name="itemSize">The item size.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="headerHeight">The Header height.</param>
        protected void DrawBars(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin, float headerHeight)
        {
            const float MinBarHeight = 4;
            if (points.Length > 0)
            {
                for (int i = 0; i < this.Entries.Count(); i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];

                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color,
                    })
                    {
                        var x = point.X - (itemSize.Width / 2);
                        var y = Math.Min(origin, point.Y);
                        var height = Math.Max(MinBarHeight, Math.Abs(origin - point.Y));
                        if (height < MinBarHeight)
                        {
                            height = MinBarHeight;
                            if (y + height > this.Margin + itemSize.Height)
                            {
                                y = headerHeight + itemSize.Height - height;
                            }
                        }

                        var rect = SKRect.Create(x, y, itemSize.Width, height);
                        canvas.DrawRect(rect, paint);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the bar background areas.
        /// </summary>
        /// <param name="canvas">The output canvas.</param>
        /// <param name="points">The entry points.</param>
        /// <param name="itemSize">The item size.</param>
        /// <param name="headerHeight">The header height.</param>
        protected void DrawBarAreas(SKCanvas canvas, SKPoint[] points, float originY, SKSize itemSize, float headerHeight)
        {
            if (points.Length > 0 && this.PointAreaAlpha > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = this.Entries.ElementAt(i);
                    var point = points[i];

                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color.WithAlpha(this.BarAreaAlpha),
                    })
                    {
                        var x = point.X - (itemSize.Width / 2);
                        var y = entry.Value > 0 ? headerHeight : originY;
                        var height = entry.Value > 0 ? originY - headerHeight : itemSize.Height + headerHeight - originY;
                        canvas.DrawRect(SKRect.Create(x, y, itemSize.Width, height), paint);
                    }
                }
            }
        }

        #endregion
    }
}
