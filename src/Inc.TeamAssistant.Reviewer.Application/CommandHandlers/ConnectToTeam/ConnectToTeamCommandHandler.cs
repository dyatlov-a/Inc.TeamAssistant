using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.ConnectToTeam;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ConnectToTeam;

internal sealed class ConnectToTeamCommandHandler : IRequestHandler<ConnectToTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ITelegramBotClient _client;
    private readonly ITranslateProvider _translateProvider;

    public ConnectToTeamCommandHandler(
        ITeamRepository teamRepository,
        IPersonRepository personRepository,
        ITelegramBotClient client,
        ITranslateProvider translateProvider)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task Handle(ConnectToTeamCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var person = new Person(
            command.Person.Id,
            command.Person.LanguageId,
            command.Person.FirstName,
            command.Person.LastName,
            command.Person.Username);
        
        if (command.TeamId.HasValue)
        {
            var team = await _teamRepository.Find(command.TeamId.Value, cancellationToken);
            
            if (team is not null)
            {
                if (team.Players.All(p => p.Id != command.Person.Id))
                {
                    team.AddPlayer(person);
                    await _teamRepository.Upsert(team, cancellationToken);
                }
                
                await _client.SendTextMessageAsync(
                    command.Person.Id,
                    await _translateProvider.Get(
                        Messages.Reviewer_JoinToTeamSuccess,
                        command.Person.LanguageId,
                        team.Name),
                    cancellationToken: cancellationToken);
            }

            await _client.SendTextMessageAsync(
                command.Person.Id,
                await _translateProvider.Get(Messages.Reviewer_TeamNotFoundError, command.Person.LanguageId),
                cancellationToken: cancellationToken);
        }
        
        await _personRepository.Upsert(person, cancellationToken);
        
        await _client.SendTextMessageAsync(
            command.Person.Id,
            await _translateProvider.Get(Messages.Reviewer_JoinSuccess, command.Person.LanguageId),
            cancellationToken: cancellationToken);
    }
}