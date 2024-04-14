using FluentValidation;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.ReassignReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ReassignReview.Validators;

internal sealed class ReassignReviewCommandValidator : AbstractValidator<ReassignReviewCommand>
{
    private readonly IHolidayService _holidayService;
    private readonly ITaskForReviewReader _taskForReviewReader;
    
    public ReassignReviewCommandValidator(IHolidayService holidayService, ITaskForReviewReader taskForReviewReader)
    {
        ArgumentNullException.ThrowIfNull(holidayService);
        ArgumentNullException.ThrowIfNull(taskForReviewReader);

        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
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
        
        var fromDate = _holidayService.GetLastDayOfWeek(DayOfWeek.Monday, DateTimeOffset.UtcNow);

        var hasReassign = await _taskForReviewReader.HasReassignFromDate(
            command.MessageContext.Person.Id,
            fromDate,
            token);

        return !hasReassign;
    }
}