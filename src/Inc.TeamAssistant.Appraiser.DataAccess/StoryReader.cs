using Dapper;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

internal sealed class StoryReader : IStoryReader
{
    private readonly IConnectionFactory _connectionFactory;
    
    public StoryReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IReadOnlyCollection<Story>> GetStories(
        Guid teamId,
        DateTimeOffset before,
        DateTimeOffset? from,
        CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                s.id AS id,
                s.bot_id AS botid,
                s.story_type AS storytype,
                s.created AS created,
                s.team_id AS teamid,
                s.chat_id AS chatid,
                s.moderator_id AS moderatorid,
                s.language_id AS languageid,
                s.title AS title,
                s.external_id AS externalid,
                s.total_value AS totalvalue,
                s.rounds_count AS roundscount,
                s.url AS url
            FROM appraiser.stories AS s
            WHERE s.team_id = @team_id AND s.created <= @before AND (@from IS NULL OR s.created >= @from);
            """,
            new
            {
                team_id = teamId,
                before = before.UtcDateTime,
                from = from?.UtcDateTime
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<Story>(command);
        
        return results.ToArray();
    }

    public async Task<IReadOnlyCollection<Story>> GetStories(
        Guid teamId,
        DateOnly assessmentDate,
        CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT s.id AS id
            FROM appraiser.stories AS s
            WHERE s.team_id = @team_id AND DATE(s.created) = @assessment_date;
            """,
            new
            {
                team_id = teamId,
                assessment_date = assessmentDate
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var storyIds = await connection.QueryAsync<Guid>(command);
        var stories = await StoryProvider.Get(connection, storyIds.ToArray(), token);
        var results = stories.OrderBy(p => p.Created).ToArray();
        
        return results;
    }

    public async Task<Story?> FindLast(Guid teamId, CancellationToken token)
    {
        var command = new CommandDefinition(
            """
            SELECT
                s.id AS id
            FROM appraiser.stories AS s
            WHERE s.team_id = @team_id AND total_value IS NULL
            ORDER BY s.created
            OFFSET 0
            LIMIT 1;
            """,
            new
            {
                team_id = teamId
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var storyId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
        if (!storyId.HasValue)
            return null;
        
        var stories = await StoryProvider.Get(connection, [storyId.Value], token);
        return stories.SingleOrDefault();
    }
}