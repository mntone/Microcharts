using System;
using System.Globalization;
using Foundation;
using SkiaSharp;
#if __IOS__
namespace Microcharts.iOS
{
    using UIKit;
    using SkiaSharp.Views.iOS;
    using System.Diagnostics;
#else
namespace Microcharts.macOS
{
    using SkiaSharp.Views.Mac;
#endif

    [Register("ChartView")]
    public class ChartView : SKCanvasView
    {
        public static SKTypeface DEFAULT_TYPEFACE
        {
            get => _DEFAULT_TYPEFACE ?? (_DEFAULT_TYPEFACE = GetDefaultTypeface());
        }
        private static SKTypeface _DEFAULT_TYPEFACE;

        private static SKTypeface GetDefaultTypeface()
        {
            var cultureName = CultureInfo.CurrentCulture.Name;
            if (cultureName == "ja-JP")
                return SKFontManager.Default.MatchCharacter("HiraginoSans", '日');
            else if (cultureName == "zh-CN")
                return SKFontManager.Default.MatchCharacter("PingFangSC", '简');
            else if (cultureName == "zh-HK")
                return SKFontManager.Default.MatchCharacter("PingFangHK", '賣');
            else if (cultureName == "zh-TW")
                return SKFontManager.Default.MatchCharacter("PingFangTC", '賣');
            else
                return SKFontManager.Default.MatchCharacter(".SFUIText-Regular", 'A');
        }

        #region Constructors

        public ChartView()
        {
            Initialize();
        }

        [Preserve]
        public ChartView(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            Initialize();
        }

        private void Initialize()
        {
#if __IOS__
            UIApplication.Notifications.ObserveContentSizeCategoryChanged((s, e) => InvalidateChart());
#endif
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
                    this.InvalidateChart();

                    if (this.chart != null)
                    {
                        if (this.chart.Typeface == null)
                            this.chart.Typeface = DEFAULT_TYPEFACE;

                        this.handler = this.chart.ObserveInvalidate(this, (view) => view.InvalidateChart());
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void InvalidateChart() => this.SetNeedsDisplayInRect(this.Bounds);

#if __MACOS__
        public override void DidChangeBackingProperties() => this.SetNeedsDisplayInRect(this.Bounds);
#endif

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            if (this.chart != null)
            {
                float scale = 1F, textScale = 1F;
#if __IOS__
                scale = (float)UIScreen.MainScreen.Scale;
                textScale = (float)(UIFont.PreferredBody.PointSize / 17F);
#else
                if (Window != null)
                    scale = (float)Window.Screen.BackingScaleFactor;
#endif
                e.Surface.Canvas.Scale(scale);
                this.chart.Draw(e.Surface.Canvas, (int)(e.Info.Width / scale), (int)(e.Info.Height / scale), textScale);
            }
            else
            {
                e.Surface.Canvas.Clear(SKColors.Transparent);
            }
        }

        #endregion
    }
}
