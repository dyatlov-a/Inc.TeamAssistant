using Dapper;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.DataAccess.Internal;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Npgsql;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

internal sealed class StoryReader : IStoryReader
{
    private readonly string _connectionString;

    public StoryReader(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        _connectionString = connectionString;
    }
    
    public async Task<IReadOnlyCollection<AssessmentHistoryDto>> GetAssessmentHistory(
        Guid teamId,
        int depth,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                DATE(s.created) AS assessmentdate,
                COUNT(*) AS storiescount
            FROM appraiser.stories AS s
            WHERE s.team_id = @team_id
            GROUP BY DATE(s.created)
            ORDER BY DATE(s.created) DESC
            OFFSET 0
            LIMIT @depth;",
            new
            {
                team_id = teamId,
                depth
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<(DateTime AssessmentDate, int StoriesCount)>(command);
        
        return results
            .Select(r => new AssessmentHistoryDto(DateOnly.FromDateTime(r.AssessmentDate), r.StoriesCount))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<StoryDto>> GetStories(
        Guid teamId,
        DateOnly assessmentDate,
        CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                s.id AS id,
                s.created AS created,
                s.title AS title
            FROM appraiser.stories AS s
            WHERE s.team_id = @team_id AND DATE(s.created) = @assessment_date
            ORDER BY s.created;",
            new
            {
                team_id = teamId,
                assessment_date = assessmentDate
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        var results = await connection.QueryAsync<StoryDto>(command);
        return results.ToArray();
    }

    public async Task<Story?> FindLast(Guid teamId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                s.id AS id
            FROM appraiser.stories AS s
            WHERE s.team_id = @team_id
            ORDER BY s.created DESC
            OFFSET 0
            LIMIT 1;",
            new { team_id = teamId },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);

        var storyId = await connection.QuerySingleOrDefaultAsync<Guid?>(command);
        if (storyId.HasValue)
            return await FindStoryByIdQuery.Find(connection, storyId.Value, token);

        return null;
    }

    public async Task<Story?> Find(Guid storyId, CancellationToken token)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        return await FindStoryByIdQuery.Find(connection, storyId, token);
    }
}