using FluentValidation;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Stories;
using Inc.TeamAssistant.Stories.Features.Dashboard;
using Inc.TeamAssistant.WebUI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddScoped<WidgetsDataFactory>()
    .AddClientServices()
    .AddIsomorphicServices();

var host = builder.Build();
var hostEnvironment = host.Services.GetRequiredService<IWebAssemblyHostEnvironment>();

ValidatorOptions.Global.Configure(new LanguageId(hostEnvironment.Environment));
       
await host.RunAsync();