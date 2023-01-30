using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.CommandHandlers.AddLocationToMap;

internal sealed class AddLocationToMapCommandHandler : IRequestHandler<AddLocationToMapCommand, AddLocationToMapResult?>
{
    private readonly ILocationsRepository _locationsRepository;

    public AddLocationToMapCommandHandler(ILocationsRepository locationsRepository)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
    }
    
    public async Task<AddLocationToMapResult?> Handle(AddLocationToMapCommand command, CancellationToken cancellationToken)
    {
        var existsMap = await _locationsRepository.Find(command.ChatId, cancellationToken);
        var map = existsMap ?? new(command.ChatId);
        
        var location = new LocationOnMap(
            command.UserId,
            command.DisplayUsername,
            command.Longitude,
            command.Latitude,
            command.Source,
            map);

        await _locationsRepository.Insert(location, cancellationToken);

        return new AddLocationToMapResult(map.Id, IsCreated: existsMap is null);
    }
}