using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.CreateBot.Validators;

internal sealed class CreateBotCommandValidator : AbstractValidator<CreateBotCommand>
{
    public CreateBotCommandValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(e => e.Token)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(e => e.OwnerId)
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
    }
}