using Dapper;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Primitives;
using Npgsql;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

internal sealed class UserSettingsProvider : IUserSettingsProvider
{
    private readonly string _connectionString;
    private readonly string _anonymousUser;

    public UserSettingsProvider(string connectionString, string anonymousUser)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        if (string.IsNullOrWhiteSpace(anonymousUser))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(anonymousUser));

        _connectionString = connectionString;
        _anonymousUser = anonymousUser;
    }

    public async Task<string> GetUserName(ParticipantId userId, CancellationToken cancellationToken)
    {
        if (userId is null)
            throw new ArgumentNullException(nameof(userId));

        var command = new CommandDefinition(@"
SELECT name
FROM users.users
WHERE id = @user_id;",
            new {user_id = userId.Value},
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        var username = await connection.QuerySingleOrDefaultAsync<string>(command);

        return string.IsNullOrWhiteSpace(username)
            ? _anonymousUser
            : username;
    }

    public async Task SetUserName(ParticipantId userId, string userName, CancellationToken cancellationToken)
    {
        if (userId is null)
            throw new ArgumentNullException(nameof(userId));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

        var command = new CommandDefinition(@"
INSERT INTO users.users (id, name)
VALUES (@user_id, @name)
ON CONFLICT (id) DO UPDATE SET
name = excluded.name;",
            new {user_id = userId.Value, name = userName},
            flags: CommandFlags.None,
            cancellationToken: cancellationToken);

        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(command);
    }
}