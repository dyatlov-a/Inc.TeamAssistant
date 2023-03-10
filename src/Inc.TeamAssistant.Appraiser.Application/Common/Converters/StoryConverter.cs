using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.Common.Converters;

internal static class StoryConverter
{
    public static StoryDetails ConvertTo(Story story)
    {
        if (story is null)
            throw new ArgumentNullException(nameof(story));

        return new(story.Title, story.Links);
    }
}