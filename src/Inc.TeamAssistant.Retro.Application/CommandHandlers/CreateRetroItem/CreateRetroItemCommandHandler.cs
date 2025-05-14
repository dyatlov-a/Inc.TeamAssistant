using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.CreateRetroItem;

internal sealed class CreateRetroItemCommandHandler : IRequestHandler<CreateRetroItemCommand, CreateRetroItemResult>
{
    private readonly IRetroRepository _retroRepository;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;
    private readonly IRetroReader _retroReader;

    public CreateRetroItemCommandHandler(
        IRetroRepository retroRepository,
        IPersonResolver personResolver,
        IRetroEventSender eventSender,
        IRetroReader retroReader)
    {
        _retroRepository = retroRepository ?? throw new ArgumentNullException(nameof(retroRepository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _retroReader = retroReader ?? throw new ArgumentNullException(nameof(retroReader));
    }

    public async Task<CreateRetroItemResult> Handle(CreateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var activeRetro = await _retroReader.FindSession(command.TeamId, RetroSessionStateRules.Active, token);
        var item = new RetroItem(
            Guid.NewGuid(),
            command.TeamId,
            DateTimeOffset.UtcNow,
            command.Type,
            command.Text,
            person.Id);
        
        if (activeRetro is not null)
            item.AttachToSession(activeRetro.Id);

        await _retroRepository.Upsert(item, token);

        await _eventSender.RetroItemChanged(RetroItemConverter.ConvertTo(item));

        return new CreateRetroItemResult(item.Id);
    }
}