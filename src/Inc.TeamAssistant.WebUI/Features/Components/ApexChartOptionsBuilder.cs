using ApexCharts;

namespace Inc.TeamAssistant.WebUI.Features.Components;

public static class ApexChartOptionsBuilder
{
    public static ApexChartOptions<T> Build<T>(Action<ApexChartOptions<T>>? configure = null)
        where T : class
    {
        var options = new ApexChartOptions<T>
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
                }
            }
        };
        
        configure?.Invoke(options);

        return options;
    }

    public static ApexChartOptions<T> DisableToolbar<T>(this ApexChartOptions<T> options)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(options);
        
        options.Chart.Toolbar = new Toolbar
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
        };

        return options;
    }
}