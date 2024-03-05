using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Commands.MarkMessageForDelete;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.MarkMessageForDelete.Validators;

internal sealed class MarkMessageForDeleteCommandValidator : AbstractValidator<MarkMessageForDeleteCommand>
{
    public MarkMessageForDeleteCommandValidator()
    {
        RuleFor(e => e.MessageId)
            .NotEmpty();
    }
}