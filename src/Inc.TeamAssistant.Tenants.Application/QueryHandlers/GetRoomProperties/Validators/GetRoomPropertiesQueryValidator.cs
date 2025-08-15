using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetRoomProperties.Validators;

internal sealed class GetRoomPropertiesQueryValidator : AbstractValidator<GetRoomPropertiesQuery>
{
    public GetRoomPropertiesQueryValidator()
    {
        RuleFor(q => q.RoomId)
            .NotEmpty();
    }
}