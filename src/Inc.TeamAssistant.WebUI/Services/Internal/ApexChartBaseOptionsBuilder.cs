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
                },
                Zoom = new Zoom
                {
                    Enabled = false
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
                            Colors = []
                        }
                    }
                }
            },
            Stroke = new Stroke
            {
                Width = new List<double> { 2 },
                Curve = Curve.Straight
            },
            Fill = new Fill
            {
                Opacity = 0
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
                    Top = -50,
                    Bottom = -50
                }
            },
            Markers = new Markers
            {
                Size = 5,
                StrokeWidth = 0,
                FillOpacity = 1
            }
        };
    }
    
    public static ApexChartOptions<T> BuildBar<T>(int xMaxValue, string xTitle, int yMaxValue, string yTitle)
        where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xTitle);
        ArgumentException.ThrowIfNullOrWhiteSpace(yTitle);
        
        return new()
        {
            Tooltip = new Tooltip
            {
                Enabled = false
            },
            Legend = new Legend
            {
                Show = false
            },
            Xaxis = new XAxis
            {
                Min = 0,
                Max = xMaxValue,
                TickAmount = xMaxValue,
                Title = new AxisTitle
                {
                    Text = xTitle
                }
            },
            Yaxis =
            [
                new YAxis
                {
                    Min = 0,
                    Max = yMaxValue,
                    TickAmount = yMaxValue,
                    Title = new AxisTitle
                    {
                        Text = yTitle
                    }
                }
            ]
        };
    }

    public static ApexChartOptions<T> CreateLineOptions<T>()
        where T : class
    {
        return new()
        {
            Stroke = new Stroke
            {
                Width = new List<double> { 4 }
            },
            Markers = new Markers
            {
                Size = 4,
                StrokeWidth = 0,
                FillOpacity = 1
            },
            Grid = new Grid
            {
                Show = false
            },
            Xaxis = new XAxis
            {
                Labels = new XAxisLabels
                {
                    Show = false
                },
                AxisTicks = new AxisTicks
                {
                    Show = false
                },
                AxisBorder = new AxisBorder
                {
                    Show = false
                }
            },
            Yaxis =
            [
                new YAxis
                {
                    Show = false
                }
            ],
            Legend = new Legend
            {
                Show = false
            },
            Tooltip = new Tooltip
            {
                Enabled = false
            }
        };
    }
}