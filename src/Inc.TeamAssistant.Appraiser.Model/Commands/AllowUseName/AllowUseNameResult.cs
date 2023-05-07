using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;

public sealed record AllowUseNameResult(bool InProgress, SummaryByStory? SummaryByStory);