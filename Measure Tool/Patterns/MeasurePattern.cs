using cAlgo.API;
using cAlgo.Helpers;
using System;
using System.Linq;

namespace cAlgo.Patterns
{
    public class MeasurePattern : PatternBase
    {
        private ChartRectangle _rectangle;

        private readonly MeasureSettings _settings;

        public MeasurePattern(PatternConfig config, MeasureSettings settings) : base("Measure", config)
        {
            _settings = settings;
        }

        protected override void OnDrawingStopped()
        {
            _rectangle = null;
        }

        protected override void OnPatternChartObjectsUpdated(long id, ChartObject updatedChartObject, ChartObject[] patternObjects)
        {
            if (updatedChartObject.ObjectType != ChartObjectType.Rectangle) return;

            var rectangle = updatedChartObject as ChartRectangle;

            if (rectangle.Y1 > rectangle.Y2)
            {
                rectangle.Color = _settings.DownColor;
            }
            else
            {
                rectangle.Color = _settings.UpColor;
            }
        }

        protected override void OnMouseUp(ChartMouseEventArgs obj)
        {
            if (MouseUpNumber == 2)
            {
                FinishDrawing();

                return;
            }

            if (_rectangle == null)
            {
                var name = GetObjectName("Rectangle");

                _rectangle = Chart.DrawRectangle(name, obj.TimeValue, obj.YValue, obj.TimeValue, obj.YValue, _settings.UpColor, _settings.Thickness, _settings.Style);

                _rectangle.IsInteractive = true;
                _rectangle.IsFilled = _settings.IsFilled;
            }
        }

        protected override void OnMouseMove(ChartMouseEventArgs obj)
        {
            if (MouseUpNumber > 1 || _rectangle == null) return;

            _rectangle.Time2 = obj.TimeValue;
            _rectangle.Y2 = obj.YValue;

            if (_rectangle.Y1 > _rectangle.Y2)
            {
                _rectangle.Color = _settings.DownColor;
            }
            else
            {
                _rectangle.Color = _settings.UpColor;
            }
        }

        protected override void DrawLabels()
        {
            if (_rectangle == null) return;

            DrawLabels(_rectangle, Id);
        }

        private void DrawLabels(ChartRectangle rectangle, long id)
        {
            var label = DrawLabelText(string.Empty, DateTime.UtcNow, 0, id, objectNameKey: "Measure", fontSize: _settings.FontSize, isBold: _settings.IsTextBold, color: _settings.TextColor);

            SetLabelText(label, rectangle);
        }

        private void SetLabelText(ChartText label, ChartRectangle rectangle)
        {
            var priceDelta = rectangle.GetPriceDelta();
            var pricePercent = priceDelta / rectangle.Y1 * 100;

            if (rectangle.Y1 > rectangle.Y2)
            {
                label.Y = rectangle.GetBottomPrice();

                priceDelta *= -1;
                pricePercent *= -1;
            }
            else
            {
                var distance = priceDelta * 0.04;

                label.Y = rectangle.GetTopPrice() + distance;
            }

            double volume = rectangle.GetVolume(Chart.Bars, Chart.Symbol);

            var barsDelta = rectangle.GetBarsNumber(Chart.Bars, Chart.Symbol);

            var barIndex = rectangle.GetStartBarIndex(Chart.Bars, Chart.Symbol) + barsDelta / 3;

            var time = Chart.Bars.GetOpenTime(barIndex, Chart.Symbol);

            label.Text = string.Format("Bars: {0} | Time: {1}\nPrice Delta: {2} ({3}%)\nVolume: {4}", (int)barsDelta, rectangle.GetTimeDelta(), Math.Round(priceDelta, Chart.Symbol.Digits), Math.Round(pricePercent, 2), volume);
            label.Time = time;
        }

        protected override void UpdateLabels(long id, ChartObject chartObject, ChartText[] labels, ChartObject[] patternObjects)
        {
            var rectangle = patternObjects.FirstOrDefault(iObject => iObject is ChartRectangle) as ChartRectangle;

            if (rectangle == null) return;

            var measureLabel = labels.FirstOrDefault(label => label.Name.EndsWith("Measure", StringComparison.OrdinalIgnoreCase));

            if (measureLabel == null)
            {
                DrawLabels(rectangle, id);

                return;
            }
            else
            {
                SetLabelText(measureLabel, rectangle);
            }
        }

        protected override ChartObject[] GetFrontObjects()
        {
            return new ChartObject[] { _rectangle };
        }
    }
}