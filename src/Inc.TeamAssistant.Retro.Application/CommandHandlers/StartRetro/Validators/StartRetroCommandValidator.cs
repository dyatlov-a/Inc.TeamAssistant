using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.StartRetro.Validators;

internal sealed class StartRetroCommandValidator : AbstractValidator<StartRetroCommand>
{
    private readonly IRetroReader _reader;
    private readonly IRetroPropertiesProvider _propertiesProvider;
    private readonly IPersonResolver _personResolver;
    
    public StartRetroCommandValidator(
        IRetroReader reader,
        IRetroPropertiesProvider propertiesProvider,
        IPersonResolver personResolver)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));

        RuleFor(x => x.RoomId)
            .NotEmpty()
            .MustAsync(HasFacilitationRights)
            .WithMessage(c => $"You do not have facilitation rights for team {c.RoomId}.")
            .MustAsync(NotHaveActiveSession)
            .WithMessage(c => $"There is already an active retro session for this team {c.RoomId}.")
            .MustAsync(HasItems)
            .WithMessage(c => $"There are no items to create a retro from team {c.RoomId}.");
    }

    private async Task<bool> NotHaveActiveSession(Guid teamId, CancellationToken token)
    {
        var retroSession = await _reader.FindSession(teamId, RetroSessionStateRules.Active, token);
        
        return retroSession is null;
    }
    
    private async Task<bool> HasItems(Guid teamId, CancellationToken token)
    {
        var items = await _reader.ReadRetroItems(teamId, states: [], token);
        
        return items.Any();
    }
    
    private async Task<bool> HasFacilitationRights(Guid teamId, CancellationToken token)
    {
        var properties = await _propertiesProvider.Get(teamId, token);
        var currentPerson = _personResolver.GetCurrentPerson();

        return properties.FacilitatorId == currentPerson.Id;
    }
}