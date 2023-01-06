using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToDashboard;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.ShowHelp;

internal sealed class ShowHelpQueryHandler : IRequestHandler<ShowHelpQuery, ShowHelpResult>
{
    private readonly ICommandProvider _commandProvider;
    private readonly IMessageBuilder _messageBuilder;
    private readonly IEnumerable<LanguageContext> _languagesInfo;

    public ShowHelpQueryHandler(
        ICommandProvider commandProvider,
        IMessageBuilder messageBuilder,
        IEnumerable<LanguageContext> languagesInfo)
    {
        _commandProvider = commandProvider ?? throw new ArgumentNullException(nameof(commandProvider));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _languagesInfo = languagesInfo ?? throw new ArgumentNullException(nameof(languagesInfo));
    }

    public async Task<ShowHelpResult> Handle(ShowHelpQuery query, CancellationToken cancellationToken)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var commandsHelp = new List<string>();

        var createAssessmentSessionCommand = _commandProvider.GetCommand(typeof(CreateAssessmentSessionCommand));
        commandsHelp.Add(await _messageBuilder.Build(
            Messages.CreateAssessmentSessionHelp,
            query.LanguageId,
            createAssessmentSessionCommand));

        var allowUseNameCommand = _commandProvider.GetCommand(typeof(AllowUseNameCommand));
        commandsHelp.Add(await _messageBuilder.Build(Messages.AllowUseNameHelp, query.LanguageId, allowUseNameCommand));

        foreach (var languageInfo in _languagesInfo)
        {
            var changeLanguageCommand = string.Format(
                _commandProvider.GetCommand(typeof(ChangeLanguageCommand)),
                languageInfo.LanguageId.Value);

            commandsHelp.Add(await _messageBuilder.Build(
                Messages.ChangeLanguageHelp,
                query.LanguageId,
                changeLanguageCommand,
                languageInfo.LanguageId.Value));
        }

        var connectToDashboardCommand = _commandProvider.GetCommand(typeof(ConnectToDashboardCommand));
        commandsHelp.Add(await _messageBuilder.Build(
            Messages.ConnectToDashboardHelp,
            query.LanguageId,
            connectToDashboardCommand));

        var addStoryToAssessmentSessionCommand = _commandProvider.GetCommand(typeof(AddStoryToAssessmentSessionCommand));
        commandsHelp.Add(await _messageBuilder.Build(
            Messages.AddStoryToAssessmentSessionHelp,
            query.LanguageId,
            addStoryToAssessmentSessionCommand));

        var reVoteEstimateCommand = _commandProvider.GetCommand(typeof(ReVoteEstimateCommand));
        commandsHelp.Add(await _messageBuilder.Build(
            Messages.ReVoteEstimateHelp,
            query.LanguageId,
            reVoteEstimateCommand));

        var acceptEstimateCommand = _commandProvider.GetCommand(typeof(AcceptEstimateCommand));
        commandsHelp.Add(await _messageBuilder.Build(
            Messages.AcceptEstimateHelp,
            query.LanguageId,
            acceptEstimateCommand));

        var finishAssessmentSessionCommand = _commandProvider.GetCommand(typeof(FinishAssessmentSessionCommand));
        commandsHelp.Add(await _messageBuilder.Build(
            Messages.FinishAssessmentSessionHelp,
            query.LanguageId,
            finishAssessmentSessionCommand));

        var showParticipantsQuery = _commandProvider.GetCommand(typeof(ShowParticipantsQuery));
        commandsHelp.Add(await _messageBuilder.Build(
            Messages.ShowParticipantsHelp,
            query.LanguageId,
            showParticipantsQuery));

        var joinToAssessmentSessionCommand = _commandProvider.GetCommand(typeof(JoinToAssessmentSessionCommand));
        commandsHelp.Add(await _messageBuilder.Build(
            Messages.JoinToAssessmentSessionHelp,
            query.LanguageId,
            joinToAssessmentSessionCommand));

        var exitFromAssessmentSessionCommand = _commandProvider.GetCommand(typeof(ExitFromAssessmentSessionCommand));
        commandsHelp.Add(await _messageBuilder.Build(
            Messages.ExitFromAssessmentSessionHelp,
            query.LanguageId,
            exitFromAssessmentSessionCommand));

        var setEstimateForStoryCommand = _commandProvider.GetCommand(typeof(SetEstimateForStoryCommand));
        foreach (var item in AssessmentValue.GetAssessments)
            commandsHelp.Add(await _messageBuilder.Build(
                Messages.SetEstimateForStoryHelp,
                query.LanguageId,
                setEstimateForStoryCommand,
                (int)item));

        return new(query.LanguageId, commandsHelp);
    }
}