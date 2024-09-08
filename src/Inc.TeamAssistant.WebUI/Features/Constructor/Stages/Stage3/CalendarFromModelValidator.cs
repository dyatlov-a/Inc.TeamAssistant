using FluentValidation;
using Inc.TeamAssistant.WebUI.Services.ClientCore;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class CalendarFromModelValidator : AbstractValidator<CalendarFromModel>
{
    private readonly HashSet<DateOnly> _dates = new();
    
    public CalendarFromModelValidator(ResourcesManager resources)
    {
        ArgumentNullException.ThrowIfNull(resources);
        
        RuleFor(e => e.SelectedWeekends)
            .NotNull()
            .Must(e => e.Distinct().Count() == e.Count);
        
        RuleFor(e => e.Holidays)
            .NotNull()
            .Must(e => e.Select(i => i.Date).Distinct().Count() == e.Count)
            .WithMessage(resources[Messages.Constructor_DuplicateHolidays]);

        _dates.Clear();
        RuleForEach(e => e.Holidays)
            .ChildRules(c => c.RuleFor(i => i.Date)
                .Must(d => _dates.Add(d)));
        
        RuleFor(e => e.Start)
            .LessThan(e => e.End);

        RuleFor(e => e.End)
            .GreaterThan(e => e.Start);
    }
}