using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Extensions;

internal static class AssessmentSessionExtensions
{
	public static AssessmentSession EnsureForModerator(this AssessmentSession? assessmentSession, string moderatorName)
	{
        if (string.IsNullOrWhiteSpace(moderatorName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(moderatorName));

		return Ensure(assessmentSession, Messages.SessionNotFoundForModerator, moderatorName);
	}

	public static AssessmentSession EnsureForAppraiser(this AssessmentSession? assessmentSession, string moderatorName)
	{
        if (string.IsNullOrWhiteSpace(moderatorName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(moderatorName));

		return Ensure(assessmentSession, Messages.SessionNotFoundForAppraiser, moderatorName);
	}

	private static AssessmentSession Ensure(
		this AssessmentSession? assessmentSession,
		MessageId messageId,
		string userName)
	{
        if (messageId is null)
			throw new ArgumentNullException(nameof(messageId));
		if (string.IsNullOrWhiteSpace(userName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

		if (assessmentSession is null)
			throw new AppraiserUserException(messageId, userName);

		return assessmentSession;
	}
}