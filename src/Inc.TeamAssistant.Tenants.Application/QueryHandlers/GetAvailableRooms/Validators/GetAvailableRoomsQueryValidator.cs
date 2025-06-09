using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableRooms;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetAvailableRooms.Validators;

internal sealed class GetAvailableRoomsQueryValidator : AbstractValidator<GetAvailableRoomsQuery>
{
    public GetAvailableRoomsQueryValidator()
    {
        RuleFor(e => e.RoomId)
            .NotEmpty()
            .When(e => e.RoomId.HasValue);
    }
}