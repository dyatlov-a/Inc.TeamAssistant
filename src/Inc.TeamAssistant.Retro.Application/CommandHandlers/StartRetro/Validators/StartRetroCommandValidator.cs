using FluentValidation;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.StartRetro.Validators;

internal sealed class StartRetroCommandValidator : AbstractValidator<StartRetroCommand>
{
    private readonly IRetroReader _retroReader;
    
    public StartRetroCommandValidator(IRetroReader retroReader)
    {
        _retroReader = retroReader ?? throw new ArgumentNullException(nameof(retroReader));
        
        RuleFor(x => x.TeamId)
            .NotEmpty()
            .MustAsync(NotHaveActiveSession)
            .WithMessage(c => $"There is already an active retro session for this team {c.TeamId}.");
    }

    private async Task<bool> NotHaveActiveSession(Guid teamId, CancellationToken token)
    {
        var retroSession = await _retroReader.FindSession(teamId, RetroSessionStateRules.Active, token);
        
        return retroSession is null;
    }
}