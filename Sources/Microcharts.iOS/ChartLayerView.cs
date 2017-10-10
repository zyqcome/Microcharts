using System;
using SkiaSharp.Views.iOS;
using UIKit;
using Xam.Animations;
using SkiaSharp;

namespace Microcharts.iOS
{
    public class ChartLayerView : SKCanvasView
    {
        public ChartLayerView()
        {
#if __IOS__
            this.BackgroundColor = UIColor.Clear;
#endif
            this.PaintSurface += OnPaintCanvas;
        }

        private ChartLayer layer;

        public ChartLayer ChartLayer
        {
            get => this.layer;
            set
            {
                if (this.layer != value)
                {
                    var oldLayer = this.layer;

                    if (oldLayer != null)
                    {
                        oldLayer.Invalidated -= OnLayerInvalidate; //TODO WEAK
                    }

                    this.layer = value;
                    this.OnLayerChanged(oldLayer, value);

                    if(this.layer != null)
                    {
                        this.layer.Invalidated += OnLayerInvalidate; //TODO WEAK
                    }
                }
            }
        }

        private bool isExiting;

        private void OnLayerInvalidate(object sender, EventArgs e)
        {
            this.OnLayerChanged(this.layer, this.layer);
        }

        private async void OnLayerChanged(ChartLayer oldLayer, ChartLayer newLayer)
        {
            if (oldLayer?.ExitAnimation != null)
            {
                this.isExiting = true;
                await this.AnimateAsync(oldLayer.ExitAnimation);
                this.isExiting = false;
            }

            this.SetNeedsDisplayInRect(this.Bounds);

            if (newLayer?.ExitAnimation != null)
                await this.AnimateAsync(newLayer.EnterAnimation);
        }

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.Transparent);

            if (!this.isExiting)
            {
                this.ChartLayer?.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
        }
    }
}
