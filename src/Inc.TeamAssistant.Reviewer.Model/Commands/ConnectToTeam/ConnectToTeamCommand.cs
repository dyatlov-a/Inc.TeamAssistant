using Inc.TeamAssistant.Users;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.ConnectToTeam;

public sealed record ConnectToTeamCommand(Guid? TeamId, PersonDto Person) : IRequest;