using FluentValidation;
using Inc.TeamAssistant.Holidays.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.ReassignReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ReassignReview.Validators;

internal sealed class ReassignReviewCommandValidator : AbstractValidator<ReassignReviewCommand>
{
    private readonly ITaskForReviewReader _taskForReviewReader;
    
    public ReassignReviewCommandValidator(ITaskForReviewReader taskForReviewReader)
    {
        ArgumentNullException.ThrowIfNull(taskForReviewReader);
        
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
        
        RuleFor(e => e.TaskId)
            .NotEmpty();

        RuleFor(e => e)
            .MustAsync(CanReassign)
            .WithMessage(e => $"{e.MessageContext.Person.DisplayName} can't reassign reviews before the end of the week.");
    }

    private async Task<bool> CanReassign(ReassignReviewCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var fromDate = DateTimeOffset.UtcNow.GetLastDayOfWeek(DayOfWeek.Monday);

        var hasReassign = await _taskForReviewReader.HasReassignFromDate(
            command.MessageContext.Person.Id,
            fromDate,
            token);

        return !hasReassign;
    }
}