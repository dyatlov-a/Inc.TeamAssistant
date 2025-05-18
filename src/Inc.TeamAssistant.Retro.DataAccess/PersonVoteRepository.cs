using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class PersonVoteRepository : IPersonVoteRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public PersonVoteRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task Upsert(PersonVote personVote, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(personVote);

        var votes = JsonSerializer.Serialize(personVote.Votes);
        var command = new CommandDefinition(
            """
            INSERT INTO retro.person_votes (retro_session_id, person_id, votes)
            VALUES (@retro_session_id, @person_id, @votes::jsonb)
            ON CONFLICT (retro_session_id, person_id) DO UPDATE SET
                votes = EXCLUDED.votes;
            """,
            new
            {
                retro_session_id = personVote.RetroSessionId,
                person_id = personVote.PersonId,
                votes = votes
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        await connection.ExecuteAsync(command);
    }
}