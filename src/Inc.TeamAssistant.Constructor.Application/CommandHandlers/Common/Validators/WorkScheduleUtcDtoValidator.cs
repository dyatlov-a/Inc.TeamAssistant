using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Common;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.Common.Validators;

internal sealed class WorkScheduleUtcDtoValidator : AbstractValidator<WorkScheduleUtcDto>
{
    public WorkScheduleUtcDtoValidator()
    {
        RuleFor(e => e.Start)
            .LessThan(e => e.End);
    }
}