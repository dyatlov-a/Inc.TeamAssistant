using ApexCharts;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Extensions;

public static class RenderContextExtensions
{
    public static void RenderChart<TItem>(this IRenderContext renderContext, ApexChart<TItem>? chart)
        where TItem : class
    {
        ArgumentNullException.ThrowIfNull(renderContext);
        
        if (renderContext.IsBrowser)
        {
            Task.Run(async () =>
            {
                await Task.Delay(100);
                if (chart is not null) 
                    await chart.RenderAsync();
            });
        }
        else
            chart?.RenderAsync();
    }
}