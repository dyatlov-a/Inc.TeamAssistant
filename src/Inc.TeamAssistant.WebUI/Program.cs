using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Inc.TeamAssistant.WebUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
       .AddScoped(_ => new HttpClient {BaseAddress = new(builder.HostEnvironment.BaseAddress)})
       .AddServices()
       .AddIsomorphic();

var host = builder.Build();

await ValidatorOptions.Global.SetCulture(host.Services);

await host.RunAsync();