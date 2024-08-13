using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateCalendar;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.CreateCalendar.Validators;

internal sealed class CreateCalendarCommandValidator : AbstractValidator<CreateCalendarCommand>
{
    public CreateCalendarCommandValidator()
    {
        RuleFor(e => e.Weekends)
            .NotNull();
        
        RuleFor(e => e.Holidays)
            .NotNull();

        RuleForEach(e => e.Holidays)
            .ChildRules(p => p.RuleFor(i => i.Value)
                .NotEmpty());
    }
}