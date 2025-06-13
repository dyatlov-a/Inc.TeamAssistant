using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.SetVotes;

public sealed record SetVotesCommand(
    Guid RoomId,
    Guid RetroSessionId,
    IReadOnlyCollection<PersonVoteByItemDto> Votes)
    : IRequest;