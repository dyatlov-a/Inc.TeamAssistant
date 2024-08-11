using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.UpdateCalendar.Validators;

internal sealed class UpdateCalendarCommandValidator : AbstractValidator<UpdateCalendarCommand>
{
    public UpdateCalendarCommandValidator()
    {
        RuleFor(e => e.Weekends)
            .NotEmpty();
        
        RuleFor(e => e.Holidays)
            .NotEmpty();

        RuleForEach(e => e.Holidays)
            .ChildRules(p => p.RuleFor(i => i.Value)
                .NotEmpty());
    }
}