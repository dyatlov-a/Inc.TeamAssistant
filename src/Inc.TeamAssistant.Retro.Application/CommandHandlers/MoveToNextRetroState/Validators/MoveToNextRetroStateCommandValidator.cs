using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.MoveToNextRetroState.Validators;

internal sealed class MoveToNextRetroStateCommandValidator : AbstractValidator<MoveToNextRetroStateCommand>
{
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    
    public MoveToNextRetroStateCommandValidator(
        IRoomPropertiesProvider propertiesProvider,
        IPersonResolver personResolver)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        
        RuleFor(c => c.Id)
            .NotEmpty();
        
        RuleFor(e => e)
            .MustAsync(HasRights)
            .WithMessage("You do not have rights to change this action item.");
    }
    
    private async Task<bool> HasRights(MoveToNextRetroStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var properties = await _propertiesProvider.Get(command.RoomId, token);

        return currentPerson.Id == properties.FacilitatorId;
    }
}