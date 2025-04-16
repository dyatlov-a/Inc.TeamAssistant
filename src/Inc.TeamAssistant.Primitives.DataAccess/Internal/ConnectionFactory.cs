using System.Data.Common;
using Npgsql;

namespace Inc.TeamAssistant.Primitives.DataAccess.Internal;

internal sealed class ConnectionFactory : IConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public ConnectionFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    public DbConnection Create() => _dataSource.CreateConnection();
}