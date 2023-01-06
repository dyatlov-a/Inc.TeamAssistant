using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;

public sealed record AllowUseNameCommand(ParticipantId AppraiserId, string AppraiserName)
    : IRequest<AllowUseNameResult>, IWithAppraiser;