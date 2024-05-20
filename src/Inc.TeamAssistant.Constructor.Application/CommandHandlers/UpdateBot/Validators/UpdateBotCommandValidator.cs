using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.UpdateBot.Validators;

internal sealed class UpdateBotCommandValidator : AbstractValidator<UpdateBotCommand>
{
    public UpdateBotCommandValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();
        
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(e => e.Token)
            .NotEmpty()
            .MaximumLength(255);
        
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
    }
}