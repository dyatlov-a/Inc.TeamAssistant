using Inc.TeamAssistant.CheckIn.Application;
using Inc.TeamAssistant.CheckIn.DataAccess.Postgres;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnectionString")!;

builder.Services
	.AddCheckInApplication()
	.AddCheckInDataAccess(connectionString)
    .AddMvc();

var app = builder.Build();

app
	.UseRouting()
	.UseEndpoints(e => e.MapDefaultControllerRoute());

app.Run();