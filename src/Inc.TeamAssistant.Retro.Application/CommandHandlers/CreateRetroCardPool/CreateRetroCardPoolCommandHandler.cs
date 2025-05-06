using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroCardPool;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.CreateRetroCardPool;

internal sealed class CreateRetroCardPoolCommandHandler
    : IRequestHandler<CreateRetroCardPoolCommand, CreateRetroCardPoolResult>
{
    private readonly IRetroCardPoolRepository _repository;
    private readonly IPersonResolver _personResolver;

    public CreateRetroCardPoolCommandHandler(IRetroCardPoolRepository repository, IPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }
    
    public async Task<CreateRetroCardPoolResult> Handle(CreateRetroCardPoolCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var pool = new RetroCardPool(Guid.NewGuid(), command.Name, person.Id);

        await _repository.Upsert(pool, token);

        return new(pool.Id);
    }
}