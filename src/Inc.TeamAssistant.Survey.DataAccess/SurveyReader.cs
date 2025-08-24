using Dapper;
using Inc.TeamAssistant.Primitives.DataAccess;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;

namespace Inc.TeamAssistant.Survey.DataAccess;

internal sealed class SurveyReader : ISurveyReader
{
    private readonly IConnectionFactory _connectionFactory;

    public SurveyReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyCollection<Question>> ReadQuestions(Guid templateId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                q.id AS id,
                q.title AS title,
                q.text AS text
            FROM survey.questions AS q
            WHERE q.id = ANY(
            	ARRAY(SELECT jsonb_array_elements_text(t.question_ids)::uuid
            	FROM survey.templates AS t
            	WHERE t.id = @template_id));
            """,
            new
            {
                template_id = templateId
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var questions = await connection.QueryAsync<Question>(command);
        
        return questions.ToArray();
    }
    
    public async Task<SurveyEntry?> ReadLastSurvey(
        Guid roomId,
        IReadOnlyCollection<SurveyState> states,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(states);

        var targetStates = states.Select(s => (int)s).ToArray();
        
        var command = new CommandDefinition(
            """
            SELECT
                s.id AS id,
                s.template_id AS templateid,
                s.room_id AS roomid,
                s.created AS created,
                s.state AS state
            FROM survey.surveys AS s
            WHERE s.room_id = @room_id AND s.state = ANY(@target_states)
            ORDER BY created DESC
            OFFSET @offset
            LIMIT @limit;
            """,
            new
            {
                room_id = roomId,
                target_states = targetStates,
                offset = 0,
                limit = 1
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var surveyEntry = await connection.QuerySingleOrDefaultAsync<SurveyEntry>(command);
        
        return surveyEntry;
    }

    public async Task<IReadOnlyCollection<SurveyEntry>> ReadSurveys(
        DateTimeOffset from,
        Guid templateId,
        SurveyState state,
        int offset,
        int limit,
        CancellationToken token)
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
            WHERE s.created <= @from AND s.template_id = @template_id AND s.state = @state
            ORDER BY created DESC
            OFFSET @offset
            LIMIT @limit;
            """,
            new
            {
                from = from.UtcDateTime,
                template_id = templateId,
                state = state,
                offset = offset,
                limit = limit
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var surveys = await connection.QueryAsync<SurveyEntry>(command);
        
        return surveys.ToArray();
    }

    public async Task<IReadOnlyCollection<SurveyAnswer>> ReadAnswers(
        IReadOnlyCollection<Guid> surveyIds,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(surveyIds);
        
        var command = new CommandDefinition(
            """
            SELECT
                sa.survey_id AS surveyid,
                sa.question_id AS questionid,
                sa.responder_id AS responderid,
                sa.responded AS responded,
                sa.value AS value,
                sa.comment AS comment
            FROM survey.survey_answers AS sa
            WHERE sa.survey_id = ANY(@survey_ids);
            """,
            new
            {
                survey_ids = surveyIds.ToArray()
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var surveyAnswers = await connection.QueryAsync<SurveyAnswer>(command);
        
        return surveyAnswers.ToArray();
    }
}