using FluentValidation;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.StartRetro.Validators;

internal sealed class StartRetroCommandValidator : AbstractValidator<StartRetroCommand>
{
    private readonly IRetroReader _reader;
    
    public StartRetroCommandValidator(IRetroReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        
        RuleFor(x => x.TeamId)
            .NotEmpty()
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
        var items = await _reader.ReadRetroItems(teamId, [], token);
        
        return items.Any();
    }
}