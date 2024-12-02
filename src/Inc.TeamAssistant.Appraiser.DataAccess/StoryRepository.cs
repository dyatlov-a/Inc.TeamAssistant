using System.Text.Json;
using Dapper;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.DataAccess.Internal;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.DataAccess;

namespace Inc.TeamAssistant.Appraiser.DataAccess;

internal sealed class StoryRepository : IStoryRepository
{
    private readonly IConnectionFactory _connectionFactory;
    
    public StoryRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<Story?> Find(Guid storyId, CancellationToken token)
    {
        await using var connection = _connectionFactory.Create();
        
        var stories = await GetStoryQuery.Get(connection, new[] { storyId }, token);

        return stories.SingleOrDefault();
    }

    public async Task Upsert(Story story, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(story);

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
            values.Add(storyForEstimate.Value);
        }
        
        var upsertStory = new CommandDefinition(@"
            INSERT INTO appraiser.stories (
                id, bot_id, story_type, created, team_id, chat_id, moderator_id, language_id, title,
                external_id, total_value, rounds_count, url)
            VALUES (
                @id, @bot_id, @story_type, @created, @team_id, @chat_id, @moderator_id, @language_id, @title,
                @external_id, @total_value, @rounds_count, @url)
            ON CONFLICT (id) DO UPDATE SET
                bot_id = EXCLUDED.bot_id,
                story_type = EXCLUDED.story_type,
                created = EXCLUDED.created,
                team_id = EXCLUDED.team_id,
                chat_id = EXCLUDED.chat_id,
                moderator_id = EXCLUDED.moderator_id,
                language_id = EXCLUDED.language_id,
                title = EXCLUDED.title,
                external_id = EXCLUDED.external_id,
                total_value = EXCLUDED.total_value,
                rounds_count = EXCLUDED.rounds_count,
                url = EXCLUDED.url;",
            new
            {
                id = story.Id,
                bot_id = story.BotId,
                story_type = story.StoryType,
                created = story.Created,
                team_id = story.TeamId,
                chat_id = story.ChatId,
                moderator_id = story.ModeratorId,
                language_id = story.LanguageId.Value,
                title = story.Title,
                external_id = story.ExternalId,
                total_value = story.TotalValue,
                rounds_count = story.RoundsCount,
                url = story.Url
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
        
        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        
        await connection.ExecuteAsync(upsertStory);
        await connection.ExecuteAsync(upsertStoryForEstimates);

        await transaction.CommitAsync(token);
    }
}