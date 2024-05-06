using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Commands.RemoveBot;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.RemoveBot.Validators;

internal sealed class RemoveBotCommandValidator : AbstractValidator<RemoveBotCommand>
{
    public RemoveBotCommandValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();

        RuleFor(e => e.CurrentUserId)
            .NotEmpty();
    }
}