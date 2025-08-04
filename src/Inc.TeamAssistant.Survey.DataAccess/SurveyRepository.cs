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

    public async Task<SurveyEntry?> Find(Guid surveyId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                s.id AS id,
                s.template_id AS templateid,
                s.room_id AS roomid,
                s.created AS created,
                s.state AS state,
                s.question_ids AS questionids
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

    public async Task<SurveyAnswer?> Find(Guid surveyId, long ownerId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                s.id AS id,
                s.survey_id AS surveyid,
                s.created AS created,
                s.owner_id AS ownerid,
                s.answers AS answers
            FROM survey.survey_answers AS s
            WHERE s.survey_id = @survey_id AND s.owner_id = @owner_id;
            """,
            new
            {
                survey_id = surveyId,
                owner_id = ownerId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var surveyEntry = await connection.QuerySingleOrDefaultAsync<SurveyAnswer>(command);
        
        return surveyEntry;
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

    public async Task Upsert(SurveyAnswer surveyAnswer, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(surveyAnswer);

        var answers = JsonSerializer.Serialize(surveyAnswer.Answers);
        var command = new CommandDefinition(
            """
            INSERT INTO survey.survey_answers (id, survey_id, created, owner_id, answers)
            VALUES (@id, @survey_id, @created, @owner_id, @answers::JSONB)
            ON CONFLICT (id) DO UPDATE SET
                survey_id = EXCLUDED.survey_id,
                created = EXCLUDED.created,
                owner_id = EXCLUDED.owner_id,
                answers = EXCLUDED.answers;
            """,
            new
            {
                id = surveyAnswer.Id,
                survey_id = surveyAnswer.SurveyId,
                created = surveyAnswer.Created,
                owner_id = surveyAnswer.OwnerId,
                answers = answers
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();
        
        await connection.ExecuteAsync(command);
    }
}