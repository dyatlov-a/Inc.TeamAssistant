using FluentValidation;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.CreateRetroItem.Validators;

internal sealed class CreateRetroItemCommandValidator : AbstractValidator<CreateRetroItemCommand>
{
    private readonly IRetroReader _reader;
    
    public CreateRetroItemCommandValidator(IRetroReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));

        RuleFor(c => c.TeamId)
            .NotEmpty()
            .MustAsync(HasNotActiveSession)
            .WithMessage(c => $"There is already an active retro session for this team {c.TeamId}.");
        
        RuleFor(c => c.ColumnId)
            .NotEmpty();
    }

    private async Task<bool> HasNotActiveSession(Guid teamId, CancellationToken token)
    {
        var activeSession = await _reader.FindSession(teamId, RetroSessionStateRules.Active, token);
        
        return activeSession is null;
    }
}