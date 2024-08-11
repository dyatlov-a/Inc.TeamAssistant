using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner
{
    public sealed record GetCalendarByOwnerQuery(long OwnerId)
        : IRequest<GetCalendarByOwnerResult?>;
}