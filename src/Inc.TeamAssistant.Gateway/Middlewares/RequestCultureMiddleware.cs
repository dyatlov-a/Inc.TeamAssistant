using FluentValidation;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Services;

namespace Inc.TeamAssistant.Gateway.Middlewares;

internal sealed class RequestCultureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRenderContext _renderContext;

    public RequestCultureMiddleware(RequestDelegate next, IRenderContext renderContext)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _renderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var languageContext = _renderContext.GetLanguageContext();
		
        ValidatorOptions.Global.Configure(languageContext.CurrentLanguage);
        
        await _next(context);
    }
}