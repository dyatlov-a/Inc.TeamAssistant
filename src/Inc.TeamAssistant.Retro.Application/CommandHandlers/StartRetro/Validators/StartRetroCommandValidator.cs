using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.StartRetro.Validators;

internal sealed class StartRetroCommandValidator : AbstractValidator<StartRetroCommand>
{
    private readonly IRetroReader _reader;
    private readonly IFacilitatorProvider _facilitatorProvider;
    private readonly IPersonResolver _personResolver;
    
    public StartRetroCommandValidator(
        IRetroReader reader,
        IFacilitatorProvider facilitatorProvider,
        IPersonResolver personResolver)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _facilitatorProvider = facilitatorProvider ?? throw new ArgumentNullException(nameof(facilitatorProvider));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));

        RuleFor(x => x.TeamId)
            .NotEmpty()
            .Must(HasFacilitationRights)
            .WithMessage(c => $"You do not have facilitation rights for team {c.TeamId}.")
            .MustAsync(NotHaveActiveSession)
            .WithMessage(c => $"There is already an active retro session for this team {c.TeamId}.")
            .MustAsync(HasItems)
            .WithMessage(c => $"There are no items to create a retro from team {c.TeamId}.");
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
    
    private bool HasFacilitationRights(Guid teamId)
    {
        var facilitator = _facilitatorProvider.Get(teamId);
        var currentPerson = _personResolver.GetCurrentPerson();

        return facilitator == currentPerson.Id;
    }
}