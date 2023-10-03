using MediatR;

namespace Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;

public sealed record AddLocationToMapCommand(
    long ChatId,
    long UserId,
    string DisplayName,
    double Longitude,
    double Latitude,
    string Data) : IRequest<AddLocationToMapResult>;