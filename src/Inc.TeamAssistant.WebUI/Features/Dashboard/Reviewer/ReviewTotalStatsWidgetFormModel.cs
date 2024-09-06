using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Reviewer;

public sealed class ReviewTotalStatsWidgetFormModel
{
    public int IntervalInDays { get; set; }
    
    private readonly List<HistoryByTeamItemDto> _review = new();
    public IReadOnlyCollection<HistoryByTeamItemDto> Review => _review;
    
    private readonly List<HistoryByTeamItemDto> _requests = new();
    public IReadOnlyCollection<HistoryByTeamItemDto> Requests => _requests;

    public ReviewTotalStatsWidgetFormModel Apply(GetHistoryByTeamResult historyByTeam, DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(historyByTeam);
        
        var interval = DateTimeOffset.UtcNow - new DateTimeOffset(date, TimeOnly.MinValue, TimeSpan.Zero);
        
        IntervalInDays = (int)interval.TotalDays;
        
        _review.Clear();
        _review.AddRange(historyByTeam.Review);
        
        _requests.Clear();
        _requests.AddRange(historyByTeam.Requests);
        
        return this;
    }
}