using FluentValidation;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.InviteForCoffee;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.InviteForCoffee.Validators;

internal sealed class InviteForCoffeeCommandValidator : AbstractValidator<InviteForCoffeeCommand>
{
    public InviteForCoffeeCommandValidator()
    {
        RuleFor(c => c.MessageContext.ChatName)
            .NotEmpty();
    }
}