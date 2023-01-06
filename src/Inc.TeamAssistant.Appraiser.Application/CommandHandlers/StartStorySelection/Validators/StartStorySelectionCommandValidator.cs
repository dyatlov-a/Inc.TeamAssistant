using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection.Validators;

internal sealed class StartStorySelectionCommandValidator : AbstractValidator<StartStorySelectionCommand>
{
    public StartStorySelectionCommandValidator(IValidator<IWithModerator> moderatorValidator)
    {
        if (moderatorValidator is null)
            throw new ArgumentNullException(nameof(moderatorValidator));

        RuleFor(e => e).SetValidator(moderatorValidator);
    }
}