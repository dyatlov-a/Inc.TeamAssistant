using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Commands.RemoveBot;

public sealed record RemoveBotCommand(Guid Id) : IRequest;