using FluentValidation;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class BotDetailsItemFormModelValidator : AbstractValidator<BotDetailsItemFormModel>
{
    public BotDetailsItemFormModelValidator()
    {
        RuleFor(e => e.LanguageId)
            .Must(f => LanguageSettings.LanguageIds.Any(l => l.Value.Equals(
                f,
                StringComparison.InvariantCultureIgnoreCase)));
        
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(64);
        
        RuleFor(e => e.ShortDescription)
            .NotEmpty()
            .MaximumLength(120);
        
        RuleFor(e => e.Description)
            .NotEmpty()
            .MaximumLength(512);
    }
}