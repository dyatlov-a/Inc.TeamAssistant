using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Reviewer;

public sealed class ReviewTotalStatsWidgetFormModel
{
    public int IntervalInDays { get; set; }
    public IReadOnlyCollection<HistoryByTeamItemDto> Review { get; set; } = Array.Empty<HistoryByTeamItemDto>();
    public IReadOnlyCollection<HistoryByTeamItemDto> Requests { get; set; } = Array.Empty<HistoryByTeamItemDto>();
}