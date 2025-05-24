using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
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

    public CreateRetroItemCommandHandler(
        IRetroItemRepository repository,
        IPersonResolver personResolver,
        IRetroEventSender eventSender,
        IPositionGenerator positionGenerator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _positionGenerator = positionGenerator ?? throw new ArgumentNullException(nameof(positionGenerator));
    }

    public async Task<CreateRetroItemResult> Handle(CreateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var position = _positionGenerator.Generate();
        var item = new RetroItem(
            Guid.CreateVersion7(),
            command.TeamId,
            DateTimeOffset.UtcNow,
            command.ColumnId,
            position,
            command.Text,
            person.Id);

        await _repository.Upsert(item, token);

        await _eventSender.RetroItemChanged(RetroItemConverter.ConvertFromCreated(item));

        return new CreateRetroItemResult(item.Id);
    }
}