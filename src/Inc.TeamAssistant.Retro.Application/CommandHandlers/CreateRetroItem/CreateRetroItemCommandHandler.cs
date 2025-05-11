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

    public CreateRetroItemCommandHandler(
        IRetroRepository retroRepository,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _retroRepository = retroRepository ?? throw new ArgumentNullException(nameof(retroRepository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task<CreateRetroItemResult> Handle(CreateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var item = new RetroItem(
            Guid.NewGuid(),
            command.TeamId,
            DateTimeOffset.UtcNow,
            command.Type,
            command.Text,
            person.Id);

        await _retroRepository.Upsert(item, token);

        await _eventSender.RetroItemChanged(RetroItemConverter.ConvertTo(item));

        return new CreateRetroItemResult(item.Id);
    }
}