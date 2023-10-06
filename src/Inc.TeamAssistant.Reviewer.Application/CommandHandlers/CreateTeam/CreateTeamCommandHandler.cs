using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.CreateTeam;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.CreateTeam;

internal sealed class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ReviewerOptions _options;
    private readonly ITelegramBotClient _client;
    private readonly ITranslateProvider _translateProvider;

    public CreateTeamCommandHandler(
        ITeamRepository teamRepository,
        ReviewerOptions options,
        ITelegramBotClient client,
        ITranslateProvider translateProvider)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task Handle(CreateTeamCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var team = new Team(
            command.ChatId,
            command.ChatName,
            Enum.Parse<NextReviewerType>(command.NextReviewerType));
        
        await _teamRepository.Upsert(team, cancellationToken);

        var link = string.Format(_options.LinkForConnectTemplate, _options.BotLink, team.Id.ToString("N"));
        var message = await _client.SendTextMessageAsync(
            command.ChatId,
            await _translateProvider.Get(
                Messages.Reviewer_ConnectToTeam, command.PersonLanguageId, team.Name, link),
            cancellationToken: cancellationToken);
        await _client.PinChatMessageAsync(command.ChatId, message.MessageId, cancellationToken: cancellationToken);
    }
}