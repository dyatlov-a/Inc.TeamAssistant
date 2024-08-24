using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.Constructor.Model.Common;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.UpdateCalendar.Validators;

internal sealed class UpdateCalendarCommandValidator : AbstractValidator<UpdateCalendarCommand>
{
    public UpdateCalendarCommandValidator(IValidator<WorkScheduleUtcDto> workScheduleUtcDtoValidator)
    {
        ArgumentNullException.ThrowIfNull(workScheduleUtcDtoValidator);
        
        RuleFor(e => e.Weekends)
            .NotNull()
            .Must(e => e.Distinct().Count() == e.Count);
        
        RuleFor(e => e.Holidays)
            .NotNull();

        RuleForEach(e => e.Holidays)
            .ChildRules(p => p.RuleFor(i => i.Value)
                .NotEmpty());
        
        RuleFor(e => e.Schedule)
            .SetValidator(workScheduleUtcDtoValidator!)
            .When(e => e.Schedule is not null);
    }
}