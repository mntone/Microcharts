// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microcharts.Uwp
{
    using SkiaSharp;
    using SkiaSharp.Views.UWP;
    using System;
    using System.Globalization;
    using Windows.Graphics.Display;
    using Windows.UI.Core;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;

    public class ChartView : SKXamlCanvas
    {
        public static SKTypeface DEFAULT_TYPEFACE
        {
            get => _DEFAULT_TYPEFACE ?? (_DEFAULT_TYPEFACE = GetDefaultTypeface());
        }
        private static SKTypeface _DEFAULT_TYPEFACE;

        private static SKTypeface GetDefaultTypeface()
        {
            var cultureName = CultureInfo.CurrentUICulture.Name;
            if (cultureName == "ja-JP")
                return SKFontManager.Default.MatchCharacter("Yu Gothic UI", '日');
            else if (cultureName == "zh-CN")
                return SKFontManager.Default.MatchCharacter("MS YaHei UI", '简');
            else if (cultureName == "zh-HK")
                return SKFontManager.Default.MatchCharacter("MS JhengHei UI", '賣');
            else if (cultureName == "zh-TW")
                return SKFontManager.Default.MatchCharacter("MS JhengHei UI", '賣');
            else
                return SKFontManager.Default.MatchCharacter("Segoe UI", 'A');
        }

        #region Constructors

        public ChartView()
        {
            displayInformation = DisplayInformation.GetForCurrentView();
            displayInformation.DpiChanged += OnDpiChanged;
            uiSettings = new UISettings();
            uiSettings.TextScaleFactorChanged += OnTextScaleFactorChanged;
            this.PaintSurface += OnPaintCanvas;
        }

        #endregion

        #region Static fields

        public static readonly DependencyProperty ChartProperty = DependencyProperty.Register(nameof(Chart), typeof(ChartView), typeof(Chart), new PropertyMetadata(null, new PropertyChangedCallback(OnChartChanged)));

        #endregion

        #region Fields

        private InvalidatedWeakEventHandler<ChartView> handler;

        private DisplayInformation displayInformation;
        private UISettings uiSettings;
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

        private async void OnDpiChanged(DisplayInformation sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(Invalidate));
        }

        private async void OnTextScaleFactorChanged(UISettings sender, object args)
        {
            uiSettings = sender;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(Invalidate));
        }

        private static void OnChartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as ChartView;

            if (view.chart != null)
            {
                view.handler.Dispose();
                view.handler = null;
            }

            view.chart = e.NewValue as Chart;
            view.Invalidate();

            if (view.chart != null)
            {
                if (view.chart.Typeface == null)
                    view.chart.Typeface = DEFAULT_TYPEFACE;

                view.handler = view.chart.ObserveInvalidate(view, (v) => v.Invalidate());
            }
        }

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            if (this.chart != null)
            {
                var scale = displayInformation.RawPixelsPerViewPixel;
                var textScale = (float)uiSettings.TextScaleFactor;
                e.Surface.Canvas.Scale((float)scale);
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
