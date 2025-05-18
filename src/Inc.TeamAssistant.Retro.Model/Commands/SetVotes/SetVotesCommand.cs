using Inc.TeamAssistant.Retro.Model.Common;
using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.SetVotes;

public sealed record SetVotesCommand(
    Guid RetroSessionId,
    IReadOnlyCollection<PersonVoteByItemDto> Votes)
    : IRequest;