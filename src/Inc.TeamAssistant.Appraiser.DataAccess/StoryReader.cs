using Dapper;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.DataAccess.Internal;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

internal sealed class StoryReader : IStoryReader
{
    private readonly IConnectionFactory _connectionFactory;
    
    public StoryReader(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
    
    public async Task<IReadOnlyCollection<AssessmentHistoryDto>> GetAssessmentHistory(
        Guid teamId,
        DateTimeOffset before,
        DateTimeOffset? from,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                DATE(s.created) AS assessmentdate,
                COUNT(*) AS storiescount
            FROM appraiser.stories AS s
            WHERE s.team_id = @team_id AND s.created <= @before
            AND (@from is null OR s.created >= @from)
            GROUP BY DATE(s.created)
            ORDER BY DATE(s.created) DESC;",
            new
            {
                team_id = teamId,
                before,
                from
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        await using var connection = _connectionFactory.Create();

        var results = await connection.QueryAsync<(DateTime AssessmentDate, int StoriesCount)>(command);
        
        return results
            .Select(r => new AssessmentHistoryDto(DateOnly.FromDateTime(r.AssessmentDate), r.StoriesCount))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<Story>> GetStories(
        Guid teamId,
        DateOnly assessmentDate,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT s.id AS id
            FROM appraiser.stories AS s
            WHERE s.team_id = @team_id AND DATE(s.created) = @assessment_date;",
            new
            {
                team_id = teamId,
                assessment_date = assessmentDate
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var storyIds = await connection.QueryAsync<Guid>(command);
        var stories = await GetStoryQuery.Get(connection, storyIds.ToArray(), token);
        var results = stories.OrderBy(p => p.Created).ToArray();
        
        return results;
    }

    public async Task<Story?> FindLast(Guid teamId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                s.id AS id
            FROM appraiser.stories AS s
            WHERE s.team_id = @team_id AND accepted = false
            ORDER BY s.created
            OFFSET 0
            LIMIT 1;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = _connectionFactory.Create();

        var storyId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
        if (!storyId.HasValue)
            return null;
        
        var stories = await GetStoryQuery.Get(connection, new[] { storyId.Value }, token);
        return stories.SingleOrDefault();
    }
}