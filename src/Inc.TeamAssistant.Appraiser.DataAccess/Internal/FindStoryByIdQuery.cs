using Dapper;
using Inc.TeamAssistant.Appraiser.Domain;
using Npgsql;

namespace Inc.TeamAssistant.Appraiser.DataAccess.Internal;

internal static class FindStoryByIdQuery
{
    public static async Task<Story?> Find(NpgsqlConnection connection, Guid storyId, CancellationToken token)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));
        
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
        
        var query = await connection.QueryMultipleAsync(command);

        var story = await query.ReadSingleOrDefaultAsync<Story>();
        var storyForEstimates = await query.ReadAsync<StoryForEstimate>();

        if (story is not null)
            foreach (var storyForEstimate in storyForEstimates)
                story.AddStoryForEstimate(storyForEstimate);

        return story;
    }
}