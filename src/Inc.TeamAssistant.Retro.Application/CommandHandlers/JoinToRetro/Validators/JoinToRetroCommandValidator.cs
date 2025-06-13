using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.JoinToRetro;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.JoinToRetro.Validators;

internal sealed class JoinToRetroCommandValidator : AbstractValidator<JoinToRetroCommand>
{
    public JoinToRetroCommandValidator()
    {
        RuleFor(c => c.ConnectionId)
            .NotEmpty();
        
        RuleFor(c => c.RoomId)
            .NotEmpty();
    }
}