using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Inc.TeamAssistant.Appraiser.Dashboard.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
       .AddScoped(_ => new HttpClient {BaseAddress = new(builder.HostEnvironment.BaseAddress)})
       .AddServices()
       .AddIsomorphic();

await builder.Build().RunAsync();