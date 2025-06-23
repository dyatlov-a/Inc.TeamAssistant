using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeRetroProperties;

internal sealed class ChangeRetroPropertiesCommandHandler : IRequestHandler<ChangeRetroPropertiesCommand>
{
    private readonly IRetroPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;

    public ChangeRetroPropertiesCommandHandler(
        IRetroPropertiesProvider propertiesProvider,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(ChangeRetroPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var retroProperties = new RetroProperties
        {
            FacilitatorId = command.IsFacilitator == true ? currentPerson.Id : null,
            TemplateId = command.TemplateId,
            TimerDuration = command.TimerDuration,
            VoteCount = command.VoteCount,
            VoteByItemCount = command.VoteByItemCount,
            RetroType = string.IsNullOrWhiteSpace(command.RetroType)
                ? null
                : Enum.Parse<RetroTypes>(command.RetroType, ignoreCase: true)
        };
        
        await _propertiesProvider.Set(command.RoomId, retroProperties, token);

        await _eventSender.RetroPropertiesChanged(
            command.RoomId,
            RetroPropertiesConverter.ConvertTo(retroProperties));
    }
}