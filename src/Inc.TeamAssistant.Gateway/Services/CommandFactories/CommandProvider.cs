using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;

namespace Inc.TeamAssistant.Gateway.Services.CommandFactories;

internal sealed class CommandProvider : ICommandProvider
{
    private readonly Dictionary<Type, string> _commands = new()
    {
        [typeof(JoinToAssessmentSessionCommand)] = CommandList.JoinToSession,
        [typeof(CreateAssessmentSessionCommand)] = CommandList.CreateAssessmentSession,
        [typeof(ChangeLanguageCommand)] = CommandList.ChangeLanguageForAssessmentSession,
        [typeof(AddStoryToAssessmentSessionCommand)] = CommandList.AddStoryToAssessmentSession,
        [typeof(ReVoteEstimateCommand)] = CommandList.ReVoteEstimate,
        [typeof(AcceptEstimateCommand)] = CommandList.AcceptEstimate,
        [typeof(FinishAssessmentSessionCommand)] = CommandList.FinishAssessmentSession,
        [typeof(ShowParticipantsQuery)] = CommandList.ShowParticipants,
        [typeof(ExitFromAssessmentSessionCommand)] = CommandList.ExitFromAssessmentSession
    };

    public string GetCommand(Type commandType) => _commands[commandType];
}