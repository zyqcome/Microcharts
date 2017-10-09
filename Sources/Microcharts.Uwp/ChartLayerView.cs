namespace Microcharts.Uwp
{
    using SkiaSharp.Views.UWP;
    using Windows.UI.Xaml;
    using Xam.Animations;

    public class ChartLayerView : SKXamlCanvas
    {
        public ChartLayerView()
        {
            this.PaintSurface += OnPaintCanvas;
        }

        public static readonly DependencyProperty LayerProperty = DependencyProperty.Register(nameof(Layer), typeof(ChartView), typeof(ChartLayer), new PropertyMetadata(null, new PropertyChangedCallback(OnLayerChanged)));

        public ChartLayer Layer
        {
            get { return (ChartLayer)GetValue(LayerProperty); }
            set { SetValue(LayerProperty, value); }
        }

        private ChartLayer exitingLayer;

        private bool isExiting;

        private static async void OnLayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as ChartLayerView;

            var oldLayer = e.OldValue as ChartLayer;
            var newLayer = e.NewValue as ChartLayer;

            if (oldLayer?.ExitAnimation != null)
            {
                view.isExiting = true;
                await view.AnimateAsync(oldLayer.ExitAnimation);
                view.isExiting = false;
            }
            
            view.Invalidate();

            if (newLayer?.ExitAnimation != null)
                await view.AnimateAsync(newLayer.EnterAnimation);
        }
        
        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            if( !this.isExiting)
            {
                var layer = this.exitingLayer ?? this.Layer;
                layer?.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
        }
    }
}
