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

    public async Task<IReadOnlyCollection<Question>> ReadQuestions(
        IReadOnlyCollection<Guid> questionIds,
        CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                q.id AS id,
                q.title AS title,
                q.text AS text
            FROM survey.questions AS q
            WHERE q.id = ANY(@question_ids);
            """,
            new
            {
                question_ids = questionIds.ToArray() 
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var questions = await connection.QueryAsync<Question>(command);
        
        return questions.ToArray();
    }

    public async Task<SurveyTemplate?> FindTemplate(Guid id, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS id,
                t.name AS name,
                t.question_ids AS questionids
            FROM survey.templates AS t
            WHERE t.id = @id;
            """,
            new
            {
                id = id
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        return await connection.QuerySingleOrDefaultAsync(command);
    }

    public async Task<SurveyEntry?> Find(
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
                s.state AS state,
                s.question_ids AS questionids
            FROM survey.surveys AS s
            WHERE s.room_id = @room_id AND s.state = ANY(@target_states);
            """,
            new
            {
                room_id = roomId,
                target_states = targetStates
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var surveyEntry = await connection.QuerySingleOrDefaultAsync<SurveyEntry>(command);
        
        return surveyEntry;
    }
}