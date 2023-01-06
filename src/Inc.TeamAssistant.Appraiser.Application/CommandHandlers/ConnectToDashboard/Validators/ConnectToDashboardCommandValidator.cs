using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToDashboard;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectToDashboard.Validators;

internal sealed class ConnectToDashboardCommandValidator : AbstractValidator<ConnectToDashboardCommand>
{
    public ConnectToDashboardCommandValidator(IValidator<IWithAppraiser> appraiserValidator)
    {
        if (appraiserValidator is null)
            throw new ArgumentNullException(nameof(appraiserValidator));

        RuleFor(e => e).SetValidator(appraiserValidator);
    }
}