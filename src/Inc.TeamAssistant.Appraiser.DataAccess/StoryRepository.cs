using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.DataAccess.Internal;
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

    public async Task<Story?> Find(Guid storyId, CancellationToken token)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        return await FindStoryByIdQuery.Find(connection, storyId, token);
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
                id, story_type, created, team_id, chat_id, moderator_id, language_id, title, external_id,
                links)
            VALUES (
                @id, @story_type, @created, @team_id, @chat_id, @moderator_id, @language_id, @title, @external_id,
                @links::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                story_type = EXCLUDED.story_type,
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
                story_type = story.StoryType,
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