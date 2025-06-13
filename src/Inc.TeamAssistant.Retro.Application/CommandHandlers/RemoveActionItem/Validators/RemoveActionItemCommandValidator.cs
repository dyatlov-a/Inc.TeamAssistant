using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveActionItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.RemoveActionItem.Validators;

internal sealed class RemoveActionItemCommandValidator : AbstractValidator<RemoveActionItemCommand>
{
    private readonly IRetroPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    
    public RemoveActionItemCommandValidator(IRetroPropertiesProvider propertiesProvider, IPersonResolver personResolver)
    {
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        
        RuleFor(e => e.Id)
            .NotEmpty();
        
        RuleFor(e => e.RoomId)
            .NotEmpty();
        
        RuleFor(e => e.ConnectionId)
            .NotEmpty();
        
        RuleFor(e => e)
            .MustAsync(HasRights)
            .WithMessage("You do not have rights to change this action item.");
    }
    
    private async Task<bool> HasRights(RemoveActionItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var properties = await _propertiesProvider.Get(command.RoomId, token);

        return currentPerson.Id == properties.FacilitatorId;
    }
}