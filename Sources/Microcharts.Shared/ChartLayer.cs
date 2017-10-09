// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microcharts
{
    using SkiaSharp;
    using System;
    using Xam.Animations;

    /// <summary>
    /// A renderer layer.
    /// </summary>
    public class ChartLayer
    {
        public ChartLayer(Action<SKCanvas,int,int> draw)
        {
            this.draw = draw;
        }

        public IAnimation EnterAnimation { get; set; }

        public IAnimation ExitAnimation { get; set; }

        private Action<SKCanvas, int, int> draw;

        public void Draw(SKCanvas canvas, int width, int height) => this.draw(canvas, width, height);
    }
}
