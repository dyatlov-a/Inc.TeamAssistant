using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Primitives.Bots;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.SetBotDetails.Validators;

internal sealed class UpdateBotDetailsCommandValidator : AbstractValidator<SetBotDetailsCommand>
{
    public UpdateBotDetailsCommandValidator(IValidator<BotDetails> botDetailsValidator)
    {
        ArgumentNullException.ThrowIfNull(botDetailsValidator);
        
        RuleFor(e => e.Token)
            .NotEmpty();
        
        RuleFor(e => e.BotDetails)
            .NotEmpty();
        
        RuleForEach(e => e.BotDetails)
            .SetValidator(botDetailsValidator);
    }
}