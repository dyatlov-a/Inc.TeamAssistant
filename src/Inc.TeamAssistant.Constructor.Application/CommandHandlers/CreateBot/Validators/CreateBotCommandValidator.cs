using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.CreateBot.Validators;

internal sealed class CreateBotCommandValidator : AbstractValidator<CreateBotCommand>
{
    public CreateBotCommandValidator(IValidator<BotDetails> botDetailsValidator)
    {
        ArgumentNullException.ThrowIfNull(botDetailsValidator);
        
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(e => e.Token)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(e => e.CalendarId)
            .NotEmpty();
        
        RuleFor(e => e.FeatureIds)
            .NotEmpty();

        RuleForEach(e => e.FeatureIds)
            .NotEmpty();

        RuleForEach(e => e.Properties)
            .ChildRules(p =>
            {
                p.RuleFor(i => i.Key)
                    .NotEmpty();
                
                p.RuleFor(i => i.Value)
                    .NotEmpty();
            });
        
        RuleFor(e => e.SupportedLanguages)
            .NotEmpty();

        RuleForEach(e => e.SupportedLanguages)
            .ChildRules(p =>
            {
                p.RuleFor(i => i)
                    .NotEmpty()
                    .Must(e => LanguageSettings.LanguageIds.Any(l => l.Value.Equals(
                        e,
                        StringComparison.InvariantCultureIgnoreCase)));
            });
        
        RuleFor(e => e.BotDetails)
            .NotEmpty();
        
        RuleForEach(e => e.BotDetails)
            .SetValidator(botDetailsValidator);
    }
}