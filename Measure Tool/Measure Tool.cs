using cAlgo.API;
using cAlgo.Controls;
using cAlgo.Helpers;
using cAlgo.Patterns;
using System.Collections.Generic;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class MeasureTool : Indicator
    {
        private StackPanel _mainButtonsPanel;

        private StackPanel _mainPanel;

        private Color _buttonsBackgroundDisableColor;

        private Color _buttonsBackgroundEnableColor;

        private Style _buttonsStyle;

        private readonly List<Button> _buttons = new List<Button>();

        #region Container Panel parameters

        [Parameter("Orientation", DefaultValue = Orientation.Vertical, Group = "Container Panel")]
        public Orientation PanelOrientation { get; set; }

        [Parameter("Horizontal Alignment", DefaultValue = HorizontalAlignment.Left, Group = "Container Panel")]
        public HorizontalAlignment PanelHorizontalAlignment { get; set; }

        [Parameter("Vertical Alignment", DefaultValue = VerticalAlignment.Top, Group = "Container Panel")]
        public VerticalAlignment PanelVerticalAlignment { get; set; }

        [Parameter("Margin", DefaultValue = 3, Group = "Container Panel")]
        public double PanelMargin { get; set; }

        #endregion Container Panel parameters

        #region Buttons parameters

        [Parameter("Disable Color", DefaultValue = "#FFCCCCCC", Group = "Buttons")]
        public string ButtonsBackgroundDisableColor { get; set; }

        [Parameter("Enable Color", DefaultValue = "Red", Group = "Buttons")]
        public string ButtonsBackgroundEnableColor { get; set; }

        [Parameter("Text Color", DefaultValue = "Blue", Group = "Buttons")]
        public string ButtonsForegroundColor { get; set; }

        [Parameter("Margin", DefaultValue = 1, Group = "Buttons")]
        public double ButtonsMargin { get; set; }

        [Parameter("Transparency", DefaultValue = 0.5, MinValue = 0, MaxValue = 1, Group = "Buttons")]
        public double ButtonsTransparency { get; set; }

        #endregion Buttons parameters

        #region TimeFrame Visibility parameters

        [Parameter("Enable", DefaultValue = false, Group = "TimeFrame Visibility")]
        public bool IsTimeFrameVisibilityEnabled { get; set; }

        [Parameter("TimeFrame", Group = "TimeFrame Visibility")]
        public TimeFrame VisibilityTimeFrame { get; set; }

        [Parameter("Only Buttons", Group = "TimeFrame Visibility")]
        public bool VisibilityOnlyButtons { get; set; }

        #endregion TimeFrame Visibility parameters

        #region Measure parameters

        [Parameter("Up Color", DefaultValue = "Blue", Group = "Measure")]
        public string MeasureUpColor { get; set; }

        [Parameter("Down Color", DefaultValue = "Red", Group = "Measure")]
        public string MeasureDownColor { get; set; }

        [Parameter("Color Alpha", DefaultValue = 50, MinValue = 0, MaxValue = 255, Group = "Measure")]
        public int MeasureColorAlpha { get; set; }

        [Parameter("Thickness", DefaultValue = 1, Group = "Measure")]
        public int MeasureThickness { get; set; }

        [Parameter("Style", DefaultValue = LineStyle.Solid, Group = "Measure")]
        public LineStyle MeasureStyle { get; set; }

        [Parameter("Filled", DefaultValue = true, Group = "Measure")]
        public bool MeasureIsFilled { get; set; }

        [Parameter("Text Color", DefaultValue = "Yellow", Group = "Measure")]
        public string MeasureTextColor { get; set; }

        [Parameter("Font Size", DefaultValue = 10, Group = "Measure")]
        public int MeasureFontSize { get; set; }

        [Parameter("Text Bold", DefaultValue = true, Group = "Measure")]
        public bool MeasureIsTextBold { get; set; }

        #endregion Measure parameters

        #region Overridden methods

        protected override void Initialize()
        {
            _mainPanel = new StackPanel
            {
                HorizontalAlignment = PanelHorizontalAlignment,
                VerticalAlignment = PanelVerticalAlignment,
                Orientation = PanelOrientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal,
                BackgroundColor = Color.Transparent,
            };

            _mainButtonsPanel = new StackPanel
            {
                Orientation = PanelOrientation,
                Margin = PanelMargin
            };

            _mainPanel.AddChild(_mainButtonsPanel);

            _buttonsBackgroundDisableColor = ColorParser.Parse(ButtonsBackgroundDisableColor);
            _buttonsBackgroundEnableColor = ColorParser.Parse(ButtonsBackgroundEnableColor);

            _buttonsStyle = new Style();

            _buttonsStyle.Set(ControlProperty.Margin, ButtonsMargin);
            _buttonsStyle.Set(ControlProperty.BackgroundColor, _buttonsBackgroundDisableColor);
            _buttonsStyle.Set(ControlProperty.ForegroundColor, ColorParser.Parse(ButtonsForegroundColor));
            _buttonsStyle.Set(ControlProperty.HorizontalContentAlignment, HorizontalAlignment.Center);
            _buttonsStyle.Set(ControlProperty.VerticalContentAlignment, VerticalAlignment.Center);
            _buttonsStyle.Set(ControlProperty.Opacity, ButtonsTransparency);

            var patternConfig = new PatternConfig(Chart, Color.Transparent, true, Color.Transparent, true, true, new Logger(this.GetType().Name, Print));

            AddPatternButton(new MeasurePattern(patternConfig, new MeasureSettings
            {
                Thickness = MeasureThickness,
                Style = MeasureStyle,
                UpColor = ColorParser.Parse(MeasureUpColor, MeasureColorAlpha),
                DownColor = ColorParser.Parse(MeasureDownColor, MeasureColorAlpha),
                TextColor = ColorParser.Parse(MeasureTextColor),
                IsFilled = MeasureIsFilled,
                FontSize = MeasureFontSize,
                IsTextBold = MeasureIsTextBold
            }));

            Chart.AddControl(_mainPanel);

            CheckTimeFrameVisibility();
        }

        public override void Calculate(int index)
        {
        }

        #endregion Overridden methods

        private void AddPatternButton(IPattern pattern)
        {
            var button = new PatternButton(pattern)
            {
                Style = _buttonsStyle,
                OnColor = _buttonsBackgroundEnableColor,
                OffColor = _buttonsBackgroundDisableColor,
            };

            _buttons.Add(button);

            _mainButtonsPanel.AddChild(button);

            pattern.Initialize();
        }

        private void CheckTimeFrameVisibility()
        {
            if (IsTimeFrameVisibilityEnabled)
            {
                if (TimeFrame != VisibilityTimeFrame)
                {
                    _mainButtonsPanel.IsVisible = false;

                    if (!VisibilityOnlyButtons) Chart.ChangePatternsVisibility(true);
                }
                else if (!VisibilityOnlyButtons)
                {
                    Chart.ChangePatternsVisibility(false);
                }
            }
        }
    }
}