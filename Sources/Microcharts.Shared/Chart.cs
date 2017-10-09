// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microcharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SkiaSharp;
    using Xam.Animations;

    public abstract class Chart
    {
        #region Constructors
        
        public Chart()
        {
            this.AddLayer(DrawBackground, Animations.FadeIn(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0)), Animations.FadeOut());
            this.AddLayer(DrawForeground, Animations.FadeIn(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1)), Animations.FadeOut());
            this.AddLayer(DrawCaption, Animations.FadeIn(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)), Animations.FadeOut());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the global margin.
        /// </summary>
        /// <value>The margin.</value>
        public float Margin { get; set; } = 20;

        /// <summary>
        /// Gets or sets the text size of the labels.
        /// </summary>
        /// <value>The size of the label text.</value>
        public float LabelTextSize { get; set; } = 16;

        /// <summary>
        /// Gets or sets the color of the chart background.
        /// </summary>
        /// <value>The color of the background.</value>
        public SKColor BackgroundColor { get; set; } = SKColors.White;

        /// <summary>
        /// Gets or sets the data entries.
        /// </summary>
        /// <value>The entries.</value>
        public IEnumerable<Entry> Entries { get; set; }

        /// <summary>
        /// Gets or sets the minimum value from entries. If not defined, it will be the minimum between zero and the 
        /// minimal entry value.
        /// </summary>
        /// <value>The minimum value.</value>
        public float MinValue
        {
            get
            {
                if (!this.Entries.Any())
                {
                    return 0;
                } 

                if (this.InternalMinValue == null)
                {
                    return Math.Min(0, this.Entries.Min(x => x.Value));
                } 

                return Math.Min(this.InternalMinValue.Value, this.Entries.Min(x => x.Value));
            }

            set => this.InternalMinValue = value;
        }

        /// <summary>
        /// Gets or sets the maximum value from entries. If not defined, it will be the maximum between zero and the 
        /// maximum entry value.
        /// </summary>
        /// <value>The minimum value.</value>
        public float MaxValue
        {
            get
            {
                if (!this.Entries.Any()) 
                {
                    return 0;
                } 

                if (this.InternalMaxValue == null)
                {
                   return Math.Max(0, this.Entries.Max(x => x.Value)); 
                } 

                return Math.Max(this.InternalMaxValue.Value, this.Entries.Max(x => x.Value));
            }

            set => this.InternalMaxValue = value;
        }

        /// <summary>
        /// Gets or sets the internal minimum value (that can be null).
        /// </summary>
        /// <value>The internal minimum value.</value>
        protected float? InternalMinValue { get; set; }

        /// <summary>
        /// Gets or sets the internal max value (that can be null).
        /// </summary>
        /// <value>The internal max value.</value>
        protected float? InternalMaxValue { get; set; }

        public IEnumerable<ChartLayer> Layers => this.layers;

        #endregion

        #region Fields

        private List<ChartLayer> layers = new List<ChartLayer>();

        #endregion

        #region Methods

        /// <summary>
        /// Add a new layer.
        /// </summary>
        /// <param name="draw">The draw function for rendering this layer.</param>
        /// <returns>The index of the layer.</returns>
        protected int AddLayer(Action<SKCanvas, int, int> draw, IAnimation enter, IAnimation exit)
        {
            this.layers.Add(new ChartLayer(draw)
            {
                EnterAnimation = enter,
                ExitAnimation = exit,
            });
            return this.layers.Count - 1;
        }

        /// <summary>
        /// Draw the graph layers onto the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Draw(SKCanvas canvas, int width, int height)
        {
            canvas.Clear(this.BackgroundColor);

            foreach (var layer in layers)
            {
                layer.Draw(canvas, width, height);
            }
        }

        #region Layers

        /// <summary>
        /// The background content contains data that changes only when the number of entries, one of the entry labels/colors changed.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        protected virtual void DrawBackground(SKCanvas canvas, int width, int height) { }

        /// <summary>
        /// The foreground contains the graph content, it will change everytime entries are updated.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        protected virtual void DrawForeground(SKCanvas canvas, int width, int height) { }

        /// <summary>
        /// The labels should contains labels and descriptive data, it will change everytime entries are updated.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        protected virtual void DrawCaption(SKCanvas canvas, int width, int height) { }

        #endregion

        /// <summary>
        /// Draws caption elements on the right or left side of the chart.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="entries">The entries.</param>
        /// <param name="isLeft">If set to <c>true</c> is left.</param>
        protected void DrawCaptionElements(SKCanvas canvas, int width, int height, List<Entry> entries, bool isLeft)
        {
            var margin = 2 * this.Margin;
            var availableHeight = height - (2 * margin);
            var x = isLeft ? this.Margin : (width - this.Margin - this.LabelTextSize);
            var ySpace = (availableHeight - this.LabelTextSize) / ((entries.Count <= 1) ? 1 : entries.Count - 1);

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries.ElementAt(i);
                var y = margin + (i * ySpace);
                if (entries.Count <= 1)
                {
                    y += (availableHeight - this.LabelTextSize) / 2;
                }

                var hasLabel = !string.IsNullOrEmpty(entry.Label);
                var hasValueLabel = !string.IsNullOrEmpty(entry.ValueLabel);

                if (hasLabel || hasValueLabel)
                {
                    var hasOffset = hasLabel && hasValueLabel;
                    var captionMargin = this.LabelTextSize * 0.60f;
                    var space = hasOffset ? captionMargin : 0;
                    var captionX = isLeft ? this.Margin : width - this.Margin - this.LabelTextSize;

                    using (var paint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = entry.Color,
                    })
                    {
                        var rect = SKRect.Create(captionX, y, this.LabelTextSize, this.LabelTextSize);
                        canvas.DrawRect(rect, paint);
                    }

                    if (isLeft)
                    {
                        captionX += this.LabelTextSize + captionMargin;
                    }
                    else
                    {
                        captionX -= captionMargin;
                    }

                    canvas.DrawCaptionLabels(entry.Label, entry.TextColor, entry.ValueLabel, entry.Color, this.LabelTextSize, new SKPoint(captionX, y + (this.LabelTextSize / 2)), isLeft ? SKTextAlign.Left : SKTextAlign.Right);
                }
            }
        }

        #endregion
    }
}