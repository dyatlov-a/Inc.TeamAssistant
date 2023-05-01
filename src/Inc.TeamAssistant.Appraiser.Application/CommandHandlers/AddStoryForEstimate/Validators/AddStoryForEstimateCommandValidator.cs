using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate.Validators;

internal sealed class AddStoryForEstimateCommandValidator : AbstractValidator<AddStoryForEstimateCommand>
{
    public AddStoryForEstimateCommandValidator(IValidator<IWithModerator> moderatorValidator)
    {
        if (moderatorValidator is null)
            throw new ArgumentNullException(nameof(moderatorValidator));

        RuleFor(e => e).SetValidator(moderatorValidator);

        RuleFor(e => e.AssessmentSessionId).NotEmpty();
    }
}