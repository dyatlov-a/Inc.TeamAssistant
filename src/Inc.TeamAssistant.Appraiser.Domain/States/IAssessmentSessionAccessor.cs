using System.Runtime.CompilerServices;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal interface IAssessmentSessionAccessor
{
	string Title { get; }
	Participant Moderator { get; }
	IReadOnlyCollection<Participant> Participants { get; }
	IStoryAccessor Story { get; }

	IAssessmentSessionAccessor AsModerator(ParticipantId moderatorId, [CallerMemberName]string operationName = null!);
	IAssessmentSessionAccessor ChangeTitle(string title);
    IAssessmentSessionAccessor ChangeStory(string title, IReadOnlyCollection<string> links);
    bool EstimateEnded();
    void AddParticipant(Participant participant);
    void RemoveParticipant(Participant participant);
	void MoveToState(Func<IAssessmentSessionAccessor, AssessmentSessionState> nextStateFactory);
    void ChangeLanguage(LanguageId languageId);
}