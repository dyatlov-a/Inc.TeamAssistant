using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.SetVotes;

public sealed record SetVotesCommand(
    Guid TeamId,
    IReadOnlyCollection<PersonVoteByItemDto> Votes)
    : IRequest;