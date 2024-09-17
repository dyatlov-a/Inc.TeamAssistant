using Inc.TeamAssistant.Primitives.DataAccess.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Inc.TeamAssistant.Primitives.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IDataTypeBuilder AddDataAccessPrimitives(
        this IServiceCollection services,
        string connectionString,
        bool logQueryParameters = false)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services
            .AddSingleton<NpgsqlDataSource>(sp => new NpgsqlDataSourceBuilder(connectionString)
                .UseLoggerFactory(sp.GetService<ILoggerFactory>())
                .EnableParameterLogging(logQueryParameters)
                .Build())
            .AddSingleton<IConnectionFactory, ConnectionFactory>();

        return new DataTypeBuilder(services);
    }
}