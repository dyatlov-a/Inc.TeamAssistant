using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed class CalendarFormModelValidator : AbstractValidator<CalendarFormModel>
{
    public CalendarFormModelValidator()
    {
        RuleFor(e => e.SelectedWeekends)
            .Must(e => e.Distinct().Count() == e.Count);
        
        RuleFor(e => e.Holidays)
            .Must(e => e.Select(i => i.Date).Distinct().Count() == e.Count);
    }
}