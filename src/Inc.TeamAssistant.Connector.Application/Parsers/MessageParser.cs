using Inc.TeamAssistant.Connector.Application.Contracts;

namespace Inc.TeamAssistant.Connector.Application.Parsers;

internal sealed class MessageParser
{
    private readonly IPersonRepository _personRepository;
    
    private const char UserNameMarker = '@';

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

        return attachedPerson is null
            ? (text, null)
            : (text.Replace(attachedPerson.Marker, string.Empty), attachedPerson.Id);
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
        
        var userName = text
            .Split(' ')
            .LastOrDefault(i => i.StartsWith(UserNameMarker))
            ?.TrimStart(UserNameMarker);

        if (string.IsNullOrWhiteSpace(userName))
            return null;
        
        var person = await _personRepository.Find(userName, token);
        var result = person is not null
            ? new ParsedPerson(person.Id, $"{UserNameMarker}{userName}")
            : null;

        return result;
    }

    private sealed record ParsedPerson(long Id, string Marker);
}