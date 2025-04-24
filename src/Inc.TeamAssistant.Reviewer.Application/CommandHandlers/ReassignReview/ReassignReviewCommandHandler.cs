using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Commands.ReassignReview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ReassignReview;

internal sealed class ReassignReviewCommandHandler : IRequestHandler<ReassignReviewCommand, CommandResult>
{
    private readonly ReassignReviewService _service;
    
    public ReassignReviewCommandHandler(ReassignReviewService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }
    
    public async Task<CommandResult> Handle(ReassignReviewCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var notifications = await _service.ReassignReview(command.TaskId, token);
        
        return CommandResult.Build(notifications.ToArray());
    }
}