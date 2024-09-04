using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class CalendarFromModelValidator : AbstractValidator<CalendarFromModel>
{
    public CalendarFromModelValidator()
    {
        RuleFor(e => e.SelectedWeekends)
            .NotNull()
            .Must(e => e.Distinct().Count() == e.Count);
        
        RuleFor(e => e.Holidays)
            .NotNull()
            .Must(e => e.Select(i => i.Date).Distinct().Count() == e.Count);
        
        RuleFor(e => e.Start)
            .LessThan(e => e.End);
    }
}