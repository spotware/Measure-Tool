using cAlgo.API;

namespace cAlgo.Patterns
{
    public class MeasureSettings
    {
        public int Thickness { get; set; }

        public LineStyle Style { get; set; }

        public Color UpColor { get; set; }

        public Color DownColor { get; set; }

        public Color TextColor { get; set; }

        public bool IsFilled { get; set; }

        public int FontSize { get; set; }

        public bool IsTextBold { get; set; }
    }
}