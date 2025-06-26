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
    
    public async Task<IReadOnlyCollection<SurveyTemplate>> GetTemplates(CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                t.id AS id,
                t.name AS name,
                t.question_ids AS questionids
            FROM survey.templates AS t;
            """,
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();
        
        var templates = await connection.QueryAsync<SurveyTemplate>(command);
        
        return templates.ToArray();
    }
}