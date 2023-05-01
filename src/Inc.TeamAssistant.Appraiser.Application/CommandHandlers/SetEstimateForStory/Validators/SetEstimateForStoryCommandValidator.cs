using FluentValidation;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory.Validators;

internal sealed class SetEstimateForStoryCommandValidator : AbstractValidator<SetEstimateForStoryCommand>
{
    public SetEstimateForStoryCommandValidator(IValidator<IWithAppraiser> appraiserValidator)
    {
        if (appraiserValidator is null)
            throw new ArgumentNullException(nameof(appraiserValidator));

        RuleFor(e => e).SetValidator(appraiserValidator);

        RuleFor(e => e.Value).Must(i => Enum.IsDefined(typeof(AssessmentValue.Value), i));
    }
}