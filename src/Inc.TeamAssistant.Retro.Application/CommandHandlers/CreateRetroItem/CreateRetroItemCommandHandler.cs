using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Rooms;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Application.Extensions;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.CreateRetroItem;

internal sealed class CreateRetroItemCommandHandler : IRequestHandler<CreateRetroItemCommand, CreateRetroItemResult>
{
    private readonly IRetroItemRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;
    private readonly IPositionGenerator _positionGenerator;
    private readonly IRoomPropertiesProvider _propertiesProvider;

    public CreateRetroItemCommandHandler(
        IRetroItemRepository repository,
        IPersonResolver personResolver,
        IRetroEventSender eventSender,
        IPositionGenerator positionGenerator,
        IRoomPropertiesProvider propertiesProvider)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _positionGenerator = positionGenerator ?? throw new ArgumentNullException(nameof(positionGenerator));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
    }

    public async Task<CreateRetroItemResult> Handle(CreateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var properties = await _propertiesProvider.Get(command.RoomId, token);
        var retroType = properties.RequiredRetroTyped();
        var person = _personResolver.GetCurrentPerson();
        var position = _positionGenerator.Generate();
        var item = new RetroItem(
            Guid.CreateVersion7(),
            command.RoomId,
            DateTimeOffset.UtcNow,
            command.ColumnId,
            position,
            command.Text,
            person.Id);

        await _repository.Upsert(item, token);

        await _eventSender.RetroItemChanged(
            RetroItemConverter.ConvertFromCreated(item, retroType),
            retroType == RetroTypes.Closed ? EventTarget.Owner : EventTarget.All);

        return new CreateRetroItemResult(item.Id);
    }
}