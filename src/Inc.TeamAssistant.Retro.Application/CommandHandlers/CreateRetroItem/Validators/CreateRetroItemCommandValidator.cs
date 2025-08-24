using FluentValidation;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.CreateRetroItem.Validators;

internal sealed class CreateRetroItemCommandValidator : AbstractValidator<CreateRetroItemCommand>
{
    private readonly IRetroSessionReader _retroSessionReader;
    
    public CreateRetroItemCommandValidator(IRetroSessionReader retroSessionReader)
    {
        _retroSessionReader = retroSessionReader ?? throw new ArgumentNullException(nameof(retroSessionReader));

        RuleFor(c => c.RoomId)
            .NotEmpty()
            .MustAsync(HasNotActiveSession)
            .WithMessage(c => $"There is already an active retro session for this team {c.RoomId}.");
        
        RuleFor(c => c.ColumnId)
            .NotEmpty();
    }

    private async Task<bool> HasNotActiveSession(Guid teamId, CancellationToken token)
    {
        var activeSession = await _retroSessionReader.FindSession(teamId, RetroSessionStateRules.Active, token);
        
        return activeSession is null;
    }
}