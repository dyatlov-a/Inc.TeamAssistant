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

    public async Task<SurveyEntry?> Find(Guid surveyId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                s.id AS id,
                s.template_id AS templateid,
                s.room_id AS roomid,
                s.created AS created,
                s.state AS state
            FROM survey.surveys AS s
            WHERE s.id = @survey_id;
            """,
            new
            {
                survey_id = surveyId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var surveyEntry = await connection.QuerySingleOrDefaultAsync<SurveyEntry>(command);
        
        return surveyEntry;
    }

    public async Task Upsert(SurveyEntry survey, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(survey);
        
        var command = new CommandDefinition(
            """
            INSERT INTO survey.surveys (id, template_id, room_id, created, state)
            VALUES (@id, @template_id, @room_id, @created, @state)
            ON CONFLICT (id) DO UPDATE SET
                template_id = EXCLUDED.template_id,
                room_id = EXCLUDED.room_id,
                created = EXCLUDED.created,
                state = EXCLUDED.state;
            """,
            new
            {
                id = survey.Id,
                template_id = survey.TemplateId,
                room_id = survey.RoomId,
                created = survey.Created,
                state = (int)survey.State
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }

    public async Task Upsert(SurveyAnswer surveyAnswer, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(surveyAnswer);
        
        var command = new CommandDefinition(
            """
            INSERT INTO survey.survey_answers (survey_id, question_id, responder_id, responded, value, comment)
            VALUES (@survey_id, @question_id, @responder_id, @responded, @value, @comment)
            ON CONFLICT (survey_id, question_id, responder_id) DO UPDATE SET
                responded = EXCLUDED.responded,
                value = EXCLUDED.value,
                comment = EXCLUDED.comment;
            """,
            new
            {
                survey_id = surveyAnswer.SurveyId,
                question_id = surveyAnswer.QuestionId,
                responder_id = surveyAnswer.ResponderId,
                responded = surveyAnswer.Responded,
                value = surveyAnswer.Value,
                comment = surveyAnswer.Comment
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}