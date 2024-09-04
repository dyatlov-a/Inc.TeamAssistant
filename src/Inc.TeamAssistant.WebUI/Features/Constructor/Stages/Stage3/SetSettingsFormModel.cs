using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;
using Inc.TeamAssistant.Constructor.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Features.Common;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModel
{
    public string Token { get; set; } = string.Empty;
    public List<string> SupportedLanguages { get; set; } = new();
    public IReadOnlyCollection<SelectListItem> Properties { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> AvailableProperties { get; set; } = new Dictionary<string, IReadOnlyCollection<SettingSection>>();
    public IReadOnlyCollection<BotDetailsFormModel> BotDetails { get; set; } = Array.Empty<BotDetailsFormModel>();
    public bool WorkAllDay { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public List<DayOfWeek> SelectedWeekends { get; } = new();
    public List<HolidayFromModel> Holidays { get; } = new();

    public SetSettingsFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        Token = stagesState.Token;
        SupportedLanguages = stagesState.SupportedLanguages.ToList();
        Properties = stagesState.SelectedFeatures
            .SelectMany(f => f.Properties.Select(v => new SelectListItem
            {
                Name = v,
                Value = stagesState.Properties.GetValueOrDefault(v, string.Empty)
            }))
            .ToArray();
        AvailableProperties = stagesState.AvailableProperties.ToDictionary();

        return this;
    }

    public SetSettingsFormModel SetBotDetails(GetBotDetailsResult botDetails)
    {
        ArgumentNullException.ThrowIfNull(botDetails);
        
        BotDetails = botDetails.Items.Select(b => new BotDetailsFormModel
        {
            LanguageId = b.LanguageId,
            Name = b.Name,
            ShortDescription = b.ShortDescription,
            Description = b.Description
        }).ToArray();

        return this;
    }
    
    public SetSettingsFormModel SetCalendar(GetCalendarByOwnerResult calendar)
    {
        ArgumentNullException.ThrowIfNull(calendar);
        
        WorkAllDay = calendar.Schedule is null;
        Start = calendar.Schedule?.Start ?? TimeOnly.MinValue;
        End = calendar.Schedule?.End ?? TimeOnly.MaxValue;
        
        SelectedWeekends.Clear();
        SelectedWeekends.AddRange(calendar.Weekends);
        
        Holidays.Clear();
        foreach (var holiday in calendar.Holidays)
            AddHoliday(holiday.Key, holiday.Value.Equals("Workday", StringComparison.InvariantCultureIgnoreCase));

        return this;
    }

    public SetSettingsFormModel SetDefaultCalendar(int clientTimezoneOffset)
    {
        const int startMinutesUtc = 10 * 60;
        const int endMinutesUtc = 19 * 60;
        
        WorkAllDay = false;
        Start = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(startMinutesUtc + clientTimezoneOffset));
        End = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(endMinutesUtc + clientTimezoneOffset));
        
        SelectedWeekends.Clear();
        SelectedWeekends.AddRange(new [] { DayOfWeek.Sunday, DayOfWeek.Saturday });
        
        Holidays.Clear();
        
        return this;
    }
    
    public void AddHoliday()
    {
        var now = DateTimeOffset.UtcNow;
        var date = Holidays.Any()
            ? Holidays.OrderByDescending(h => h.Date).First().Date
            : new DateOnly(now.Year, now.Month, now.Day);
        
        AddHoliday(date.AddDays(1), isWorkday: false);
    }
    
    public void ToggleLanguage(LanguageId languageId)
    {
        var languageCode = languageId.Value;
        
        if (SupportedLanguages.Contains(languageCode, StringComparer.InvariantCultureIgnoreCase))
            SupportedLanguages.Remove(languageCode);
        else
            SupportedLanguages.Add(languageCode);
    }
    
    public void ToggleWeekend(DayOfWeek item)
    {
        if (SelectedWeekends.Contains(item))
            SelectedWeekends.Remove(item);
        else
            SelectedWeekends.Add(item);
    }

    public SetBotDetailsCommand ToSetBotDetailsCommand()
    {
        var botDetails = BotDetails
            .Select(d => new BotDetails(d.LanguageId, d.Name, d.ShortDescription, d.Description))
            .ToArray();
        
        return new SetBotDetailsCommand(Token, botDetails);
    }

    public UpdateCalendarCommand ToUpdateCalendarCommand()
    {
        return new UpdateCalendarCommand(
            ToWorkScheduleUtcDto(),
            SelectedWeekends,
            ToHolidays());
    }

    private WorkScheduleUtcDto? ToWorkScheduleUtcDto() => WorkAllDay ? null : new WorkScheduleUtcDto(Start, End);

    private IReadOnlyDictionary<DateOnly, string> ToHolidays()
        => Holidays.ToDictionary(h => h.Date, h => h.IsWorkday ? "Workday" : "Holiday");

    private void AddHoliday(DateOnly date, bool isWorkday)
    {
        var holidaysCount = Holidays.Count;
        Holidays.Add(new HolidayFromModel
        {
            DateFieldId = $"date-{holidaysCount}",
            WorkdayFieldId = $"workday-{holidaysCount}",
            Date = date,
            IsWorkday = isWorkday
        });
    }
}