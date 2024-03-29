using Inc.TeamAssistant.Primitives.DataAccess.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Inc.TeamAssistant.Primitives.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IDataTypeBuilder AddDataAccess(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        if (services.All(s => s.ImplementationType != typeof(ConnectionFactory)))
            services
                .AddSingleton<IConnectionFactory, ConnectionFactory>()
                .AddNpgsqlDataSource(connectionString);

        return new DataTypeBuilder();
    }
}