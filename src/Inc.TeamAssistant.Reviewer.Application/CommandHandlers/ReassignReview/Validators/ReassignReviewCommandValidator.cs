using FluentValidation;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.ReassignReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ReassignReview.Validators;

internal sealed class ReassignReviewCommandValidator : AbstractValidator<ReassignReviewCommand>
{
    private readonly ITaskForReviewReader _reader;
    
    public ReassignReviewCommandValidator(ITaskForReviewReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        
        RuleFor(e => e.TaskId)
            .NotEmpty();

        RuleFor(e => e)
            .MustAsync(CanReassign)
            .WithMessage(e => $"{e.MessageContext.Person.DisplayName} can't reassign reviews before the end of the week.");
    }

    private async Task<bool> CanReassign(ReassignReviewCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var fromDate = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(7));
        var hasReassign = await _reader.HasReassignFromDate(
            command.MessageContext.Person.Id,
            fromDate,
            token);

        return !hasReassign;
    }
}