using Dapper;
using Inc.TeamAssistant.Appraiser.Domain;
using Npgsql;

namespace Inc.TeamAssistant.Appraiser.DataAccess.Internal;

internal static class GetStoryQuery
{
    public static async Task<IReadOnlyCollection<Story>> Get(
        NpgsqlConnection connection,
        IReadOnlyCollection<Guid> storyIds,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(connection);

        var command = new CommandDefinition(@"
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
            WHERE s.id = ANY(@story_ids);

            SELECT
                sfe.id AS id,
                sfe.story_id AS storyid,
                sfe.participant_id AS participantid,
                sfe.participant_display_name AS participantdisplayname,
                sfe.value AS value
            FROM appraiser.story_for_estimates AS sfe
            WHERE sfe.story_id = ANY(@story_ids);",
            new { story_ids = storyIds },
            flags: CommandFlags.None,
            cancellationToken: token);
        
        await using var query = await connection.QueryMultipleAsync(command);

        var stories = await query.ReadAsync<Story>();
        var storiesLookup = stories.ToDictionary(s => s.Id);
        var storyForEstimates = await query.ReadAsync<StoryForEstimate>();
        
        foreach (var storyForEstimate in storyForEstimates)
            storiesLookup[storyForEstimate.StoryId].AddStoryForEstimate(storyForEstimate);

        return storiesLookup.Values;
    }
}