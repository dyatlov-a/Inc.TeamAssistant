using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Commands.ReassignReview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ReassignReview;

internal sealed class ReassignReviewCommandHandler : IRequestHandler<ReassignReviewCommand, CommandResult>
{
    private readonly ReassignReviewService _reassignReviewService;
    
    public ReassignReviewCommandHandler(ReassignReviewService reassignReviewService)
    {
        _reassignReviewService =
            reassignReviewService ?? throw new ArgumentNullException(nameof(reassignReviewService));
    }
    
    public async Task<CommandResult> Handle(ReassignReviewCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var notifications = await _reassignReviewService.ReassignReview(command.TaskId, token);
        
        return CommandResult.Build(notifications.ToArray());
    }
}