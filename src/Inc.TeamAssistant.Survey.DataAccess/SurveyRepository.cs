using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;

namespace Inc.TeamAssistant.Survey.DataAccess;

internal sealed class SurveyRepository : ISurveyRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SurveyRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task Upsert(SurveyEntry survey, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(survey);

        var questionIds = JsonSerializer.Serialize(survey.QuestionIds);
        var command = new CommandDefinition(
            """
            INSERT INTO survey.surveys (id, template_id, room_id, created, state, question_ids)
            VALUES (@id, @template_id, @room_id, @created, @state, @question_ids::JSONB)
            ON CONFLICT (id) DO UPDATE SET
                template_id = EXCLUDED.template_id,
                room_id = EXCLUDED.room_id,
                created = EXCLUDED.created,
                state = EXCLUDED.state,
                question_ids = EXCLUDED.question_ids;
            """,
            new
            {
                id = survey.Id,
                template_id = survey.TemplateId,
                room_id = survey.RoomId,
                created = survey.Created,
                state = (int)survey.State,
                question_ids = questionIds
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}