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
    private readonly IRetroTemplateReader _retroTemplateReader;

    public ChangeRetroPropertiesCommandHandler(
        IRetroPropertiesProvider propertiesProvider,
        IPersonResolver personResolver,
        IRetroEventSender eventSender,
        IRetroTemplateReader retroTemplateReader)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _retroTemplateReader = retroTemplateReader ?? throw new ArgumentNullException(nameof(retroTemplateReader));
    }

    public async Task Handle(ChangeRetroPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var currentProperties = await _propertiesProvider.Get(command.RoomId, token);
        var retroProperties = new RetroProperties
        {
            FacilitatorId = command.IsFacilitator == true ? currentPerson.Id : null,
            TemplateId = command.TemplateId,
            TimerDuration = command.TimerDuration,
            VoteCount = command.VoteCount
        };
        var columns = retroProperties.TemplateId.HasValue && currentProperties.TemplateId != retroProperties.TemplateId
            ? await _retroTemplateReader.GetColumns(retroProperties.TemplateId.Value, token)
            : [];
        
        await _propertiesProvider.Set(command.RoomId, retroProperties, token);

        await _eventSender.RetroPropertiesChanged(
            command.RoomId,
            RetroPropertiesConverter.ConvertTo(retroProperties),
            columns.Select(RetroColumnConverter.ConvertTo).ToArray());
    }
}