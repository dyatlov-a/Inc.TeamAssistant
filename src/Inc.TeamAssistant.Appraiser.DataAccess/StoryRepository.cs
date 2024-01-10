using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Npgsql;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

internal sealed class StoryRepository : IStoryRepository
{
    private readonly string _connectionString;

    public StoryRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        _connectionString = connectionString;
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
            return await Find(storyId.Value, token);

        return null;
    }

    public async Task<Story?> Find(Guid storyId, CancellationToken token)
    {
        var command = new CommandDefinition(@"
            SELECT
                s.id AS id,
                s.created AS created,
                s.team_id AS teamid,
                s.chat_id AS chatid,
                s.moderator_id AS moderatorid,
                s.language_id AS languageid,
                s.title AS title,
                s.external_id AS externalid,
                s.links AS links
            FROM appraiser.stories AS s
            WHERE s.id = @story_id;

            SELECT
                sfe.id AS id,
                sfe.story_id AS storyid,
                sfe.participant_id AS participantid,
                sfe.participant_display_name AS participantdisplayname,
                sfe.value AS value
            FROM appraiser.story_for_estimates AS sfe
            WHERE sfe.story_id = @story_id;",
            new { story_id = storyId },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var query = await connection.QueryMultipleAsync(command);

        var story = await query.ReadSingleOrDefaultAsync<Story>();
        var storyForEstimates = await query.ReadAsync<StoryForEstimate>();

        if (story is not null)
            foreach (var storyForEstimate in storyForEstimates)
                story.AddStoryForEstimate(storyForEstimate);

        return story;
    }

    public async Task Upsert(Story story, CancellationToken token)
    {
        if (story is null)
            throw new ArgumentNullException(nameof(story));

        var ids = new List<Guid>(story.StoryForEstimates.Count);
        var storyIds = new List<Guid>(story.StoryForEstimates.Count);
        var participantIds = new List<long>(story.StoryForEstimates.Count);
        var participantDisplayNames = new List<string>(story.StoryForEstimates.Count);
        var values = new List<int>(story.StoryForEstimates.Count);

        foreach (var storyForEstimate in story.StoryForEstimates)
        {
            ids.Add(storyForEstimate.Id);
            storyIds.Add(storyForEstimate.StoryId);
            participantIds.Add(storyForEstimate.ParticipantId);
            participantDisplayNames.Add(storyForEstimate.ParticipantDisplayName);
            values.Add((int)storyForEstimate.Value);
        }
        
        var upsertStory = new CommandDefinition(@"
            INSERT INTO appraiser.stories (
                id, created, team_id, chat_id, moderator_id, language_id, title, external_id, links)
            VALUES (@id, @created, @team_id, @chat_id, @moderator_id, @language_id, @title, @external_id, @links::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                created = EXCLUDED.created,
                team_id = EXCLUDED.team_id,
                chat_id = EXCLUDED.chat_id,
                moderator_id = EXCLUDED.moderator_id,
                language_id = EXCLUDED.language_id,
                title = EXCLUDED.title,
                external_id = EXCLUDED.external_id,
                links = EXCLUDED.links;",
            new
            {
                id = story.Id,
                created = story.Created,
                team_id = story.TeamId,
                chat_id = story.ChatId,
                moderator_id = story.ModeratorId,
                language_id = story.LanguageId.Value,
                title = story.Title,
                external_id = story.ExternalId,
                links = JsonSerializer.Serialize(story.Links)
            },
            flags: CommandFlags.None,
            cancellationToken: token);

        var upsertStoryForEstimates = new CommandDefinition(@"
            INSERT INTO appraiser.story_for_estimates (id, story_id, participant_id, participant_display_name, value)
            SELECT id, story_id, participant_id, participant_display_name, value
            FROM UNNEST(@ids, @story_ids, @participant_ids, @participant_display_names, @values)
                AS i(id, story_id, participant_id, participant_display_name, value)
            ON CONFLICT (id) DO UPDATE SET
                story_id = EXCLUDED.story_id,
                participant_id = EXCLUDED.participant_id,
                participant_display_name = EXCLUDED.participant_display_name,
                value = EXCLUDED.value;",
            new
            {
                ids,
                story_ids = storyIds,
                participant_ids = participantIds,
                participant_display_names = participantDisplayNames,
                values
            },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        await connection.ExecuteAsync(upsertStory);
        await connection.ExecuteAsync(upsertStoryForEstimates);

        await transaction.CommitAsync(token);
    }
}