using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Users;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.LeaveFromTeam;

internal sealed class LeaveFromTeamCommandHandler : IRequestHandler<LeaveFromTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ITelegramBotClient _client;
    private readonly ITranslateProvider _translateProvider;

    public LeaveFromTeamCommandHandler(
        ITeamRepository teamRepository,
        ITaskForReviewRepository taskForReviewRepository,
        IPersonRepository personRepository,
        ITelegramBotClient client,
        ITranslateProvider translateProvider)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task Handle(LeaveFromTeamCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var currentTeam = await _teamRepository.Find(command.TeamId, cancellationToken);
        if (currentTeam is null)
            throw new ApplicationException($"Team {command.TeamId} was not found.");

        var owner = currentTeam.Players.SingleOrDefault(p => p.Id == command.PersonId)
                    ?? await _personRepository.Find(UserIdentity.Create(command.PersonId), cancellationToken);
        if (owner is null)
            throw new ApplicationException($"User {command.PersonFirstName} was not found.");

        var lastReviewer = await _personRepository.FindLastReviewer(currentTeam.Id, cancellationToken);
        var reviewer = currentTeam.GetNextReviewer(owner, lastReviewer);

        await _taskForReviewRepository.RetargetAndLeave(
            currentTeam.Id,
            owner,
            reviewer,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _client.SendTextMessageAsync(
            command.PersonId,
            await _translateProvider.Get(
                Messages.Reviewer_LeaveTeamSuccess,
                command.PersonLanguageId,
                currentTeam.Name),
            cancellationToken: cancellationToken);
    }
}