using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroCardPool;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.UpdateRetroCardPool;

internal sealed class UpdateRetroCardPoolCommandHandler : IRequestHandler<UpdateRetroCardPoolCommand>
{
    private readonly IRetroCardPoolRepository _repository;
    private readonly IPersonResolver _personResolver;

    public UpdateRetroCardPoolCommandHandler(IRetroCardPoolRepository repository, IPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task Handle(UpdateRetroCardPoolCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var pool = await command.Id.Required(_repository.Find, token);
        
        pool
            .CheckRights(person.Id)
            .ChangeName(command.Name);

        await _repository.Upsert(pool, token);
    }
}