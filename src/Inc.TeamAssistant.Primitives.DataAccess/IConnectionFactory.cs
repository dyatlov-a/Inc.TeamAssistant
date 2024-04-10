using Npgsql;

namespace Inc.TeamAssistant.Primitives.DataAccess;

public interface IConnectionFactory
{
    NpgsqlConnection Create();
}