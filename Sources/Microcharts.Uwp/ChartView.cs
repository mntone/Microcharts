// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microcharts.Uwp
{
    using SkiaSharp;
    using SkiaSharp.Views.UWP;
    using System;
    using Windows.Graphics.Display;
    using Windows.UI.Core;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;

    public class ChartView : SKXamlCanvas
    {
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
