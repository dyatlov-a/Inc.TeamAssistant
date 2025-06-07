using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class RetroAssessmentRepository : IRetroAssessmentRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public RetroAssessmentRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<RetroAssessment?> Find(Guid sessionId, long personId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                ra.retro_session_id AS retrosessionid,
                ra.person_id AS personid,
                ra.value AS value
            FROM retro.retro_assessments AS ra
            WHERE ra.retro_session_id = @retro_session_id AND ra.person_id = @person_id;
            """,
            new
            {
                retro_session_id = sessionId,
                person_id = personId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var retroAssessment = await connection.QuerySingleOrDefaultAsync<RetroAssessment>(command);
        
        return retroAssessment;
    }

    public async Task Upsert(RetroAssessment assessment, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(assessment);
        
        var command = new CommandDefinition(
            """
            INSERT INTO retro.retro_assessments (
                retro_session_id,
                person_id,
                value)
            VALUES (
                @retro_session_id,
                @person_id,
                @value)
            ON CONFLICT (retro_session_id, person_id)
            DO UPDATE SET 
                value = EXCLUDED.value;
            """,
            new
            {
                retro_session_id = assessment.RetroSessionId,
                person_id = assessment.PersonId,
                value = assessment.Value
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}