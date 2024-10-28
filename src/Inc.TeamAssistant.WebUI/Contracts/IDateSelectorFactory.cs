namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IDateSelectorFactory
{
    IReadOnlyDictionary<string, DateOnly> CreateWeeks();

    IReadOnlyDictionary<string, DateOnly> CreateMonths();
}