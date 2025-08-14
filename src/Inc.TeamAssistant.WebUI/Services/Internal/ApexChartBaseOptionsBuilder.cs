using ApexCharts;

namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal static class ApexChartBaseOptionsBuilder
{
    public static ApexChartBaseOptions BuildDefault()
    {
        return new ApexChartBaseOptions
        {
            Legend = new Legend
            {
                Position = LegendPosition.Bottom,
                HorizontalAlign = Align.Center
            },
            Title = new Title
            {
                Align = Align.Center,
                Style = new TitleStyle
                {
                    Color = "#000"
                }
            },
            Theme = new Theme
            {
                Mode = Mode.Dark,
                Palette = PaletteType.Palette1,
                Monochrome = new ThemeMonochrome
                {
                    Enabled = true,
                    Color = "#555555",
                    ShadeTo = Mode.Dark
                }
            },
            Chart = new Chart
            {
                FontFamily = "'Roboto', sans-serif",
                Background = "none",
                Width = "100%",
                RedrawOnParentResize = true,
                Animations = new Animations
                {
                    Enabled = false
                },
                Toolbar = new Toolbar
                {
                    Show = false,
                    Tools = new Tools
                    {
                        Download = false,
                        Selection = false,
                        Zoom = false,
                        Zoomin = false,
                        Zoomout = false,
                        Pan = false,
                        Reset = false
                    }
                }
            }
        };
    }

    public static ApexChartOptions<T> BuildRadar<T>(int maxValue = 5)
        where T : class
    {
        return new()
        {
            PlotOptions = new PlotOptions
            {
                Radar = new PlotOptionsRadar
                {
                    Polygons = new RadarPolygons
                    {
                        StrokeColors = new List<string> { "#CCCCCC" },
                        ConnectorColors = "#CCCCCC",
                        Fill = new RadarPolygonsFill
                        {
                            Colors = ["transparent"]
                        }
                    }
                }
            },
            Tooltip = new Tooltip
            {
                Enabled = false
            },
            Yaxis =
            [
                new YAxis
                {
                    Min = 0,
                    Max = maxValue,
                    TickAmount = maxValue
                }
            ],
            Theme = new Theme
            {
                Mode = Mode.Dark,
                Palette = PaletteType.Palette1,
                Monochrome = new ThemeMonochrome
                {
                    Enabled = false
                }
            },
            Legend = new Legend
            {
                Show = false
            },
            Grid = new Grid
            {
                Padding = new Padding
                {
                    Top = -50
                }
            }
        };
    }
}