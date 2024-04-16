using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Reviewer.Application.Contracts;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewHistoryService
{
    private readonly IHolidayService _holidayService;
    private readonly ITaskForReviewReader _taskForReviewReader;

    public ReviewHistoryService(IHolidayService holidayService, ITaskForReviewReader taskForReviewReader)
    {
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
    }

    public async Task<IReadOnlyDictionary<long, int>> GetHistory(Guid teamId, CancellationToken token)
    {
        var lastMonday = _holidayService.GetLastDayOfWeek(DayOfWeek.Monday, DateTimeOffset.UtcNow);
        
        var history = await _taskForReviewReader.GetHistory(teamId, lastMonday, token);
        
        return history;
    }
}