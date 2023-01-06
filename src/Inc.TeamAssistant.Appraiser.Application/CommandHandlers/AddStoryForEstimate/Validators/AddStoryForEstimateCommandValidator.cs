using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate.Validators;

internal sealed class AddStoryForEstimateCommandValidator : AbstractValidator<AddStoryForEstimateCommand>
{
    public AddStoryForEstimateCommandValidator(IValidator<IWithAppraiser> appraiserValidator)
    {
        if (appraiserValidator is null)
            throw new ArgumentNullException(nameof(appraiserValidator));

        RuleFor(e => e).SetValidator(appraiserValidator);

        RuleFor(e => e.AssessmentSessionId).NotEmpty();
    }
}