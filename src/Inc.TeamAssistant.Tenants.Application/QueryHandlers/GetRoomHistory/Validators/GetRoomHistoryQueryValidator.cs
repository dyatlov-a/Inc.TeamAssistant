using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomHistory;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetRoomHistory.Validators;

internal sealed class GetRoomHistoryQueryValidator : AbstractValidator<GetRoomHistoryQuery>
{
    public GetRoomHistoryQueryValidator()
    {
        RuleFor(q => q.RoomId)
            .NotEmpty();
    }
}