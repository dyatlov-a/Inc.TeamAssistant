using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.ShowParticipants.Validators;

internal sealed class ShowParticipantsQueryValidator : AbstractValidator<ShowParticipantsQuery>
{
    public ShowParticipantsQueryValidator(IValidator<IWithAppraiser> appraiserValidator)
    {
        if (appraiserValidator == null)
            throw new ArgumentNullException(nameof(appraiserValidator));

        RuleFor(e => e).SetValidator(appraiserValidator);
    }
}