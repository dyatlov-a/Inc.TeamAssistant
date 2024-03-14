using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;

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
        if (targetTeam is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, teamId);

        // TODO: Accept task for leave last person (Impl remove team case)
        var otherTeammates = teammates.Where(t => t.PersonId != messageContext.PersonId).ToArray();

        if (otherTeammates.Any())
        {
            var nextReviewer = otherTeammates.MinBy(t => t.PersonId);

            await _taskForReviewRepository.RetargetAndLeave(
                teamId,
                messageContext.PersonId,
                nextReviewer.PersonId,
                DateTimeOffset.UtcNow,
                token);
        }
    }
}