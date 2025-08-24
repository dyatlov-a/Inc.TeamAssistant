using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.StartRetro.Validators;

internal sealed class StartRetroCommandValidator : AbstractValidator<StartRetroCommand>
{
    private readonly IRetroSessionReader _retroSessionReader;
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    
    public StartRetroCommandValidator(
        IRetroSessionReader retroSessionReader,
        IRoomPropertiesProvider propertiesProvider,
        IPersonResolver personResolver)
    {
        _retroSessionReader = retroSessionReader ?? throw new ArgumentNullException(nameof(retroSessionReader));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));

        RuleFor(c => c.RoomId)
            .NotEmpty()
            .MustAsync(HasFacilitationRights)
            .WithMessage(c => $"You do not have facilitation rights for room {c.RoomId}.")
            .MustAsync(NotHaveActiveSession)
            .WithMessage(c => $"There is already an active retro session for this room {c.RoomId}.")
            .MustAsync(HasItems)
            .WithMessage(c => $"There are no items to create a retro from room {c.RoomId}.");
    }

    private async Task<bool> NotHaveActiveSession(Guid roomId, CancellationToken token)
    {
        var retroSession = await _retroSessionReader.FindSession(roomId, RetroSessionStateRules.Active, token);
        
        return retroSession is null;
    }
    
    private async Task<bool> HasItems(Guid roomId, CancellationToken token)
    {
        var items = await _retroSessionReader.ReadRetroItems(roomId, states: [], token);
        
        return items.Any(i => !string.IsNullOrWhiteSpace(i.Text));
    }
    
    private async Task<bool> HasFacilitationRights(Guid roomId, CancellationToken token)
    {
        var properties = await _propertiesProvider.Get(roomId, token);
        var currentPerson = _personResolver.GetCurrentPerson();

        return properties.FacilitatorId == currentPerson.Id;
    }
}