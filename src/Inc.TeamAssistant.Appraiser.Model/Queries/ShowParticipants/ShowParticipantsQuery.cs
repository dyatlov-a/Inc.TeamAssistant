using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;

public sealed record ShowParticipantsQuery(long TargetChatId, ParticipantId AppraiserId, string AppraiserName)
    : IRequest<CommandResult>, IWithAppraiser;