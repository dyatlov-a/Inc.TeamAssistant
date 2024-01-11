using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class LeaveTeamHandler : ILeaveTeamHandler
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly ITeamAccessor _teamAccessor;

    public LeaveTeamHandler(ITaskForReviewRepository taskForReviewRepository, ITeamAccessor teamAccessor)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task Handle(MessageContext messageContext, Guid teamId, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        var teammates = await _teamAccessor.GetTeammates(teamId, token);
        var targetTeam = messageContext.FindTeam(teamId);
        if (!targetTeam.HasValue)
            throw new ApplicationException($"Team {teamId} was not found.");
        
        var lastReviewerId = await _taskForReviewRepository.FindLastReviewer(teamId, token);
        var nextReviewerId = new RoundRobinReviewerStrategy(teammates.Select(t => t.PersonId).ToArray())
            .Next(messageContext.PersonId, lastReviewerId);

        await _taskForReviewRepository.RetargetAndLeave(
            teamId,
            messageContext.PersonId,
            nextReviewerId,
            DateTimeOffset.UtcNow,
            token);
    }
}