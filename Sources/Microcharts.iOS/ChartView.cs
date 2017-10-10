#if __IOS__
namespace Microcharts.iOS
{
    using UIKit;
    using SkiaSharp.Views.iOS;
    using System.Linq;
#else
namespace Microcharts.macOS
{
    using SkiaSharp.Views.Mac;
#endif

    public class ChartView : UIView
    {
        public ChartView()
        {
#if __IOS__
                        this.BackgroundColor = UIColor.Clear;
#endif
            this.layers = new ChartLayerView[3];
            for (int i = 0; i < this.layers.Length; i++)
            {
                var layer = new ChartLayerView()
                {
                    AutoresizingMask = UIViewAutoresizing.FlexibleDimensions,
                    Frame = this.Bounds,
                };
                this.layers[i] = layer;
            }

            this.AddSubviews(this.layers);
        }

        private ChartLayerView[] layers;

        private Chart chart;

        public Chart Chart
        {
            get => this.chart;
            set
            {
                if (this.chart != value)
                {
                    var oldChart = this.chart;
                    this.chart = value;
                    this.OnChartChanged(oldChart, value);
                }
            }
        }

        private void OnChartChanged(Chart oldChar, Chart newChart)
        {
            for (int i = 0; i < this.layers.Length; i++)
            {
                var layer = this.layers[i];
                layer.ChartLayer = chart?.Layers.ElementAt(i);
            }
        }
    }
}
