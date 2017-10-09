namespace Microcharts.Uwp
{
    using System.Linq;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public class ChartView : Grid
    {
        public ChartView()
        {
            this.layers =  new ChartLayerView[3];
            for (int i = 0; i < this.layers.Length; i++)
            {
                var layer = new ChartLayerView()
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                this.layers[i] = layer;
                this.Children.Add(layer);
            }
        }

        private ChartLayerView[] layers;

        public static readonly DependencyProperty ChartProperty = DependencyProperty.Register(nameof(Chart), typeof(ChartView), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnChartChanged)));

        public Chart Chart
        {
            get { return (Chart)GetValue(ChartProperty); }
            set { SetValue(ChartProperty, value); }
        }
        
        private static void OnChartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as ChartView;
     
            var chart = e.NewValue as Chart;

            for (int i = 0; i < view.layers.Length; i++)
            {
                var layer = view.layers[i];
                layer.Layer = chart?.Layers.ElementAt(i);
            }
        }
    }
}
