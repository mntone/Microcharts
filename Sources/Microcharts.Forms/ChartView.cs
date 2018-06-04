// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microcharts.Forms
{
	using Xamarin.Forms;
	using SkiaSharp.Views.Forms;
    using SkiaSharp;

    public class ChartView : SKCanvasView
    {
        #region Constructors

        public ChartView()
        {
            this.BackgroundColor = Color.Transparent;
            this.PaintSurface += OnPaintCanvas;
        }

        #endregion

        #region Static fields

        public static readonly BindableProperty ChartProperty = BindableProperty.Create(nameof(Chart), typeof(Chart), typeof(ChartView), null, propertyChanged: OnChartChanged);

        #endregion

        #region Fields

        private InvalidatedWeakEventHandler<ChartView> handler;

        private Chart chart;

        #endregion

        #region Properties

        public Chart Chart
        {
            get { return (Chart)GetValue(ChartProperty); }
            set { SetValue(ChartProperty, value); }
        }

        #endregion

        #region Methods

        private static void OnChartChanged(BindableObject d, object oldValue, object value)
        {
            var view = d as ChartView;

            if (view.chart != null)
            {
                view.handler.Dispose();
                view.handler = null;
            }

            view.chart = value as Chart;
            view.InvalidateSurface();

            if (view.chart != null)
            {
                view.handler = view.chart.ObserveInvalidate(view, (v) => v.InvalidateSurface());
            }
        }

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            if (this.chart != null)
            {
                var canvasView = (SKCanvasView)sender;
                var scale = e.Info.Height / canvasView.Height;
                e.Surface.Canvas.Scale((float)scale);
                this.chart.Draw(e.Surface.Canvas, (int)(e.Info.Width / scale), (int)(e.Info.Height / scale), 1F);
            }
            else
            {
                e.Surface.Canvas.Clear(SKColors.Transparent);
            }
        }

        #endregion
    }
}
