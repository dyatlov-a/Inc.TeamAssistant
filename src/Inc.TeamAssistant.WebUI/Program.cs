using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Inc.TeamAssistant.WebUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
       .AddScoped(_ => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress) })
       .AddClientServices()
       .AddIsomorphicServices();

await builder.Build().RunAsync();