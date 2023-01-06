using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToDashboard;

public sealed record ConnectToDashboardCommand(ParticipantId AppraiserId, string AppraiserName)
	: IRequest<ConnectToDashboardResult>, IWithAppraiser;