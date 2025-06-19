using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeActionItem.Validators;

internal sealed class ChangeActionItemCommandValidator : AbstractValidator<ChangeActionItemCommand>
{
    private readonly IRetroPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    
    public ChangeActionItemCommandValidator(IRetroPropertiesProvider provider, IPersonResolver personResolver)
    {
        _propertiesProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        
        RuleFor(e => e.Id)
            .NotEmpty();

        RuleFor(e => e.RetroItemId)
            .NotEmpty();
        
        RuleFor(e => e.RoomId)
            .NotEmpty();

        RuleFor(e => e.Text)
            .NotEmpty();

        RuleFor(e => e.State)
            .Must(e => Enum.TryParse<ActionItemState>(e, ignoreCase: true, out _))
            .WithMessage("Invalid action item state.");

        RuleFor(e => e)
            .MustAsync(HasRights)
            .WithMessage("You do not have rights to change this action item.");
    }

    private async Task<bool> HasRights(ChangeActionItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var properties = await _propertiesProvider.Get(command.RoomId, token);

        return currentPerson.Id == properties.FacilitatorId;
    }
}