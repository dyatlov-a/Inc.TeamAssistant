using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.UpdateRetroItem;

internal sealed class UpdateRetroItemCommandHandler : IRequestHandler<UpdateRetroItemCommand>
{
    private readonly IRetroItemRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;

    public UpdateRetroItemCommandHandler(
        IRetroItemRepository repository,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }
    
    public async Task Handle(UpdateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var item = await command.Id.Required(_repository.Find, token);

        item
            .CheckCanChange(person.Id)
            .ChangeText(command.Text)
            .ChangeParent(command.ParentId)
            .ChangePosition(command.ColumnId, command.Position);
        
        await _repository.Upsert(item, token);

        var excludedOwner = item.RetroSession is null;

        foreach (var changed in item.Children.Append(item))
            await _eventSender.RetroItemChanged(
                RetroItemConverter.ConvertFromChanged(changed, item.RetroSession?.State),
                excludedOwner);
    }
}