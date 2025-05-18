using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.SetVotes;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetVotes;

internal sealed class SetVotesCommandHandler : IRequestHandler<SetVotesCommand>
{
    private readonly IPersonVoteRepository _repository;
    private readonly IPersonResolver _personResolver;

    public SetVotesCommandHandler(IPersonVoteRepository repository, IPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task Handle(SetVotesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var votes = command.Votes
            .Select(v => new PersonVoteByItem(v.ItemId, v.Vote))
            .ToArray();
        var personVote = new PersonVote(command.RetroSessionId, currentPerson.Id, votes);

        await _repository.Upsert(personVote, token);
    }
}