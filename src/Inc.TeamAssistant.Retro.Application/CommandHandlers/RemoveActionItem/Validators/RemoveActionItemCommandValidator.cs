using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveActionItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.RemoveActionItem.Validators;

internal sealed class RemoveActionItemCommandValidator : AbstractValidator<RemoveActionItemCommand>
{
    private readonly IFacilitatorProvider _facilitatorProvider;
    private readonly IPersonResolver _personResolver;
    
    public RemoveActionItemCommandValidator(IFacilitatorProvider facilitatorProvider, IPersonResolver personResolver)
    {
        _facilitatorProvider = facilitatorProvider ?? throw new ArgumentNullException(nameof(facilitatorProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        
        RuleFor(e => e.Id)
            .NotEmpty();
        
        RuleFor(e => e.TeamId)
            .NotEmpty();
        
        RuleFor(e => e.ConnectionId)
            .NotEmpty();
        
        RuleFor(e => e)
            .Must(HasRights)
            .WithMessage("You do not have rights to change this action item.");
    }
    
    private bool HasRights(RemoveActionItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var facilitator = _facilitatorProvider.Get(command.TeamId);

        return currentPerson.Id == facilitator;
    }
}