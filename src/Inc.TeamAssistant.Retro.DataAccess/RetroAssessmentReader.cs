using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class RetroAssessmentReader : IRetroAssessmentReader
{
    private readonly IConnectionFactory _connectionFactory;

    public RetroAssessmentReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyCollection<int>> Read(Guid retroSessionId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT ra.value AS value
            FROM retro.retro_assessments AS ra
            WHERE ra.retro_session_id = @retro_session_id;
            """,
            new
            {
                retro_session_id = retroSessionId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var retroAssessments = await connection.QueryAsync<int>(command);
        
        return retroAssessments.ToArray();
    }

    public async Task<(Guid RoomId, int? Value)> Read(Guid retroSessionId, long personId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                rs.room_id AS roomid,
                ra.value AS value
            FROM retro.retro_sessions AS rs
            LEFT JOIN retro.retro_assessments AS ra ON ra.retro_session_id = rs.id AND ra.person_id = @person_id
            WHERE rs.id = @retro_session_id;
            """,
            new
            {
                retro_session_id = retroSessionId,
                person_id = personId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var retroAssessment = await connection.QuerySingleAsync<(Guid RoomId, int? Value)>(command);
        
        return retroAssessment;
    }
}