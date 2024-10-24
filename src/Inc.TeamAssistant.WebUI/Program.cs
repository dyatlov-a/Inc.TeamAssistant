using FluentValidation;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Inc.TeamAssistant.WebUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
       .AddScoped(_ => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress) })
       .AddClientServices()
       .AddIsomorphicServices();

var host = builder.Build();
var renderContext = host.Services.GetRequiredService<IRenderContext>();
var languageContext = renderContext.GetLanguageContext();

ValidatorOptions.Global.Configure(languageContext.CurrentLanguage);
       
await host.RunAsync();