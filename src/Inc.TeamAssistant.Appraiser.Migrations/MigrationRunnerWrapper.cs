using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Appraiser.Migrations;

public sealed class MigrationRunnerWrapper
{
    private readonly ServiceProvider _serviceProvider;

    public MigrationRunnerWrapper(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _serviceProvider = Build(connectionString);
    }

    public void Execute(Action<IMigrationRunner> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        using var scope = _serviceProvider.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        action.Invoke(runner);
    }

    private ServiceProvider Build(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        return new ServiceCollection()
               .AddFluentMigratorCore()
               .ConfigureRunner(c => c
                   .AddPostgres()
                   .WithGlobalConnectionString(connectionString)
                   .ScanIn(typeof(CreateUsersTable).Assembly)
                   .For.Migrations()
                   .WithVersionTable(new VersionTableSettings()))
               .AddLogging(c => c.AddFluentMigratorConsole())
               .BuildServiceProvider();
    }
}