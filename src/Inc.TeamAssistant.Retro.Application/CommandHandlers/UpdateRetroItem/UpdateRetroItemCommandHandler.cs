using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.UpdateRetroItem;

internal sealed class UpdateRetroItemCommandHandler : IRequestHandler<UpdateRetroItemCommand>
{
    private readonly IRetroItemRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;
    private readonly IRetroPropertiesProvider _provider;

    public UpdateRetroItemCommandHandler(
        IRetroItemRepository repository,
        IPersonResolver personResolver,
        IRetroEventSender eventSender,
        IRetroPropertiesProvider provider)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public async Task Handle(UpdateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var item = await command.Id.Required(_repository.Find, token);
        var properties = await _provider.Get(item.RoomId, token);
        var retroType = properties.RequiredRetroType();

        item
            .CheckCanChange(person.Id, properties.FacilitatorId)
            .ChangeText(command.Text)
            .ChangeParent(command.ParentId)
            .ChangePosition(command.ColumnId, command.Position);

        await _repository.Upsert(item, token);

        if (item.RetroSession is not null || retroType != RetroTypes.Closed)
        {
            var eventTarget = item.RetroSession is null
                ? EventTarget.Participants
                : EventTarget.All;
            
            foreach (var changed in item.Children.Append(item))
                await _eventSender.RetroItemChanged(
                    RetroItemConverter.ConvertFromChanged(changed, item.RetroSession?.State, retroType),
                    eventTarget);
        }
    }
}