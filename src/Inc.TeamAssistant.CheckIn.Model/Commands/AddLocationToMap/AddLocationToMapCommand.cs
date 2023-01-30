using MediatR;

namespace Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;

public sealed record AddLocationToMapCommand(
    long ChatId,
    string Source,
    long UserId,
    string DisplayUsername,
    double Longitude,
    double Latitude) : IRequest<AddLocationToMapResult>;