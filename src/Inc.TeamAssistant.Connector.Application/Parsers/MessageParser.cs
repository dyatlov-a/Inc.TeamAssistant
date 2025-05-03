using System.Text.RegularExpressions;
using Inc.TeamAssistant.Connector.Application.Contracts;

namespace Inc.TeamAssistant.Connector.Application.Parsers;

internal sealed class MessageParser
{
    private const char UserNameMarker = '@';
    private static readonly Regex WhiteSpacePattern = new(@"\s{2,}", RegexOptions.Compiled);
    private static readonly Regex UsernamePattern = new(
        @"@[\w]{5,32}\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    private readonly IPersonRepository _personRepository;

    public MessageParser(IPersonRepository personRepository)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<(string Text, long? TargetPersonId)> Parse(IInputMessage message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(message);
        
        var text = message.Text.Replace(UserNameMarker + message.BotName, string.Empty).Trim();
        var attachedPerson = await TryGetPersonFromEntities(message, token)
            ?? (string.IsNullOrWhiteSpace(text) ? null : await TryGetPersonFromText(text, token));

        if (attachedPerson is null)
            return (text, null);
        
        var cleanText = WhiteSpacePattern.Replace(
            text.Replace(attachedPerson.Marker, string.Empty),
            " ").TrimEnd('\n');
            
        return (cleanText, attachedPerson.Id);
    }

    private async Task<ParsedPerson?> TryGetPersonFromEntities(IInputMessage message, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (!message.PersonIds.Any())
            return null;

        var personId = message.PersonIds.Last();
        var person = await _personRepository.Find(personId, token);
        var result = person is null
            ? null
            : new ParsedPerson(person.Id, person.Name);

        return result;
    }

    private async Task<ParsedPerson?> TryGetPersonFromText(string text, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        var userNames = UsernamePattern.Matches(text)
            .Select(i => i.Value.TrimStart(UserNameMarker))
            .Distinct()
            .ToArray();

        foreach (var userName in userNames)
        {
            var person = await _personRepository.Find(userName, token);
            if (person is not null)
                return new ParsedPerson(person.Id, $"{UserNameMarker}{userName}");
        }

        return null;
    }

    private sealed record ParsedPerson(long Id, string Marker);
}