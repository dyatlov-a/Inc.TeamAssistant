using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeActionItem.Validators;

internal sealed class ChangeActionItemCommandValidator : AbstractValidator<ChangeActionItemCommand>
{
    private readonly IFacilitatorProvider _facilitatorProvider;
    private readonly IPersonResolver _personResolver;
    
    public ChangeActionItemCommandValidator(IFacilitatorProvider facilitatorProvider, IPersonResolver personResolver)
    {
        _facilitatorProvider = facilitatorProvider ?? throw new ArgumentNullException(nameof(facilitatorProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        
        RuleFor(e => e.Id)
            .NotEmpty();

        RuleFor(e => e.RetroItemId)
            .NotEmpty();
        
        RuleFor(e => e.TeamId)
            .NotEmpty();

        RuleFor(e => e.Text)
            .NotEmpty();

        RuleFor(e => e.State)
            .Must(e => Enum.TryParse<ActionItemState>(e, ignoreCase: true, out _))
            .WithMessage("Invalid action item state.");

        RuleFor(e => e)
            .Must(HasRights)
            .WithMessage("You do not have rights to change this action item.");
    }

    private bool HasRights(ChangeActionItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var facilitator = _facilitatorProvider.Get(command.TeamId);

        return currentPerson.Id == facilitator;
    }
}