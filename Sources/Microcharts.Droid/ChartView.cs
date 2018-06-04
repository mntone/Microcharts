namespace Microcharts.Droid
{
    using Android.Content;
    using SkiaSharp.Views.Android;
    using Android.Util;
    using System;
    using Android.Runtime;

    public class ChartView : SKCanvasView
    {
        #region Constructors

        public ChartView(Context context) : base(context)
        {
            this.PaintSurface += OnPaintCanvas;
        }

        public ChartView(Context context, IAttributeSet attributes) : base(context, attributes)
        {
            this.PaintSurface += OnPaintCanvas;
        }

        public ChartView(Context context, IAttributeSet attributes, int defStyleAtt) : base(context, attributes, defStyleAtt)
        {
            this.PaintSurface += OnPaintCanvas;
        }

        public ChartView(IntPtr ptr, JniHandleOwnership jni) : base(ptr, jni)
        {
            this.PaintSurface += OnPaintCanvas;
        }

        #endregion

        #region Fields

        private InvalidatedWeakEventHandler<ChartView> handler;

        private Chart chart;

        #endregion

        #region Properties

        public Chart Chart
        {
            get => this.chart;
            set
            {
                if (this.chart != value)
                {
                    if (this.chart != null)
                    {
                        handler.Dispose();
                        this.handler = null;
                    }

                    this.chart = value;
                    this.Invalidate();

                    if (this.chart != null)
                    {
                        this.handler = this.chart.ObserveInvalidate(this, (view) => view.Invalidate());
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            if (this.chart != null)
            {
                var scale = Resources.DisplayMetrics.Density;
                var textScale = Resources.DisplayMetrics.ScaledDensity / scale;
                e.Surface.Canvas.Scale(scale);
                this.chart.Draw(e.Surface.Canvas, (int)(e.Info.Width / scale), (int)(e.Info.Height / scale), textScale);
            }
        }

        #endregion
    }
}
