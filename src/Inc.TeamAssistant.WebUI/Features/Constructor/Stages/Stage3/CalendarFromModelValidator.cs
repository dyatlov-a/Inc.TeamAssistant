using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

internal sealed class CalendarFromModelValidator : AbstractValidator<CalendarFormModel>
{
    public CalendarFromModelValidator(IStringLocalizer<ConstructorResources> localizer)
    {
        ArgumentNullException.ThrowIfNull(localizer);
        
        RuleFor(e => e.Workdays)
            .NotNull()
            .Must(e => e.Distinct().Count() == e.Count);
        
        RuleFor(e => e.Holidays)
            .NotNull()
            .Must(e => e.Select(i => i.Date).Distinct().Count() == e.Count)
            .WithMessage(localizer["DuplicateHolidays"]);
        
        RuleFor(e => e.Start)
            .LessThan(e => e.End);

        RuleFor(e => e.End)
            .GreaterThan(e => e.Start);
    }
}