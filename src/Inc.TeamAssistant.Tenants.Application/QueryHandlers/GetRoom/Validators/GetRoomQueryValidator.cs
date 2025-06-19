using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoom;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetRoom.Validators;

internal sealed class GetRoomQueryValidator : AbstractValidator<GetRoomQuery>
{
    public GetRoomQueryValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();
    }
}