using Inc.TeamAssistant.Gateway.Configs;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Telegram.Bot.Extensions.LoginWidget;

namespace Inc.TeamAssistant.Gateway.Auth;

public sealed class TelegramAuthService
{
    private readonly AuthOptions _options;
    private readonly IBotAccessor _botAccessor;
    private readonly IPersonAccessor _personAccessor;
    private readonly ILogger<TelegramAuthService> _logger;

    public TelegramAuthService(
        AuthOptions options,
        IBotAccessor botAccessor,
        IPersonAccessor personAccessor,
        ILogger<TelegramAuthService> logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
        _personAccessor = personAccessor ?? throw new ArgumentNullException(nameof(personAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Person?> CanLoginAs(
        long id,
        string firstName,
        string? lastName,
        string? username,
        string? photoUrl,
        string authDate,
        string hash,
        CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(authDate);
        ArgumentException.ThrowIfNullOrWhiteSpace(hash);

        var authToken = await _botAccessor.GetToken(_options.BotId, token);
        var fields = TelegramAuthProperties.ToFieldSet(
            id,
            firstName,
            lastName,
            username,
            photoUrl,
            authDate,
            hash);
        var loginWidget = new LoginWidget(authToken);
        var authState = loginWidget.CheckAuthorization(fields);
        var person = new Person(id, firstName, username);

        if (authState == Authorization.Valid)
            return await _personAccessor.Ensure(person, token);

        _logger.LogError(
            "User {Id} {UserName} is not valid {AuthState}",
            person.Id,
            person.DisplayName,
            authState);

        return null;
    }
}