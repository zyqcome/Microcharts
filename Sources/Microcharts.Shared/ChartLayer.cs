// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microcharts
{
    using System;
    using SkiaSharp;
    using Xam.Animations;

    /// <summary>
    /// A renderer layer.
    /// </summary>
    public class ChartLayer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microcharts.ChartLayer"/> class.
        /// </summary>
        /// <param name="draw">The drawing method.</param>
        public ChartLayer(Action<SKCanvas, int, int> draw)
        {
            this.draw = draw;
        }

        #endregion

        #region Fields

        private Action<SKCanvas, int, int> draw;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the layer is invalidated and needs to be redrawn.
        /// </summary>
        public event EventHandler Invalidated;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the entrance animation.
        /// </summary>
        /// <value>The enter animation.</value>
        public IAnimation EnterAnimation { get; set; }

        /// <summary>
        /// Gets or sets the exit animation.
        /// </summary>
        /// <value>The exit animation.</value>
        public IAnimation ExitAnimation { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Draw this layer onto the specified canvas with width and height.
        /// </summary>
        /// <returns>The draw.</returns>
        /// <param name="canvas">Canvas.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void Draw(SKCanvas canvas, int width, int height) => this.draw(canvas, width, height);

        /// <summary>
        /// Invalidate this layer and request a draw.
        /// </summary>
        public void Invalidate() => this.Invalidated?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
