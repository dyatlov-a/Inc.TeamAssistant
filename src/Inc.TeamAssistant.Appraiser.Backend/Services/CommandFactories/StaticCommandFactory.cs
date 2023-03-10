using Inc.TeamAssistant.WebUI;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToDashboard;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Backend.Services.CommandFactories;

internal sealed class StaticCommandFactory : ICommandFactory
{
    private readonly Dictionary<string, Func<CommandContext, IBaseRequest?>> _commandList;
    private readonly HashSet<string> _targets;

	public StaticCommandFactory()
    {
        _targets = new(AssessmentValue.GetAssessments.Select(a => ((int)a).ToString()));
        _commandList = BuildCommands();
    }

	public IBaseRequest? TryCreate(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

		var commandKey = _commandList.Keys.FirstOrDefault(k => context.Cmd.StartsWith(
			k,
			StringComparison.InvariantCultureIgnoreCase));

        return commandKey is null ? null : _commandList[commandKey](context);
    }

    private Dictionary<string, Func<CommandContext, IBaseRequest?>> BuildCommands()
	{
        var commands = new Dictionary<string, Func<CommandContext, IBaseRequest?>>
        {
            [CommandList.Start] = CreateConnectAppraiserCommand,
            [CommandList.JoinToSession] = c => new JoinToAssessmentSessionCommand(c.UserId, c.LanguageId),
            [CommandList.AllowUseName] = c => new AllowUseNameCommand(c.UserId, c.RealUserName),
            [CommandList.ExitFromAssessmentSession] = c => new ExitFromAssessmentSessionCommand(c.UserId, c.UserName),
            [CommandList.CreateAssessmentSession] = c => new CreateAssessmentSessionCommand(c.ChatId, c.UserId, c.UserName, c.LanguageId),
            [CommandList.ConnectToDashboard] = c => new ConnectToDashboardCommand(c.UserId, c.UserName),
            [CommandList.ShowParticipants] = c => new ShowParticipantsQuery(c.UserId, c.UserName),
            [CommandList.AddStoryToAssessmentSession] = CreateStartStorySelectionCommand
        };

        foreach (var language in Settings.LanguageIds)
        {
            var command = string.Format(CommandList.ChangeLanguageForAssessmentSession, language.Value);
            commands.Add(command, c => new ChangeLanguageCommand(c.UserId, c.UserName, language));
        }

        foreach (var target in _targets)
            commands.Add(CommandList.SetEstimateForStory + target, CreateEstimateStoryCommand);

        commands.Add(CommandList.NoIdea, c => new SetEstimateForStoryCommand(c.UserId, c.UserName, Value: null));
        commands.Add(CommandList.AcceptEstimate, c => new AcceptEstimateCommand(c.UserId, c.UserName));
        commands.Add(CommandList.ReVoteEstimate, c => new ReVoteEstimateCommand(c.UserId, c.UserName));
        commands.Add(CommandList.FinishAssessmentSession, c => new FinishAssessmentSessionCommand(c.UserId, c.UserName));
        commands.Add(CommandList.Help, c => new ShowHelpQuery(c.LanguageId));

        return commands;
    }

    private StartStorySelectionCommand CreateStartStorySelectionCommand(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        return new(context.UserId, context.UserName);
    }

    private IBaseRequest CreateEstimateStoryCommand(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var parameter = new string(context.Cmd.Where(Char.IsDigit).ToArray());

        return new SetEstimateForStoryCommand(context.UserId, context.UserName, int.Parse(parameter));
    }

    private IBaseRequest CreateConnectAppraiserCommand(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var assessmentSessionIdValue = context.Cmd.Replace(CommandList.Start, string.Empty).Trim();

        var assessmentSessionId = Guid.TryParse(assessmentSessionIdValue, out var value)
            ? new(value)
            : default(AssessmentSessionId);

        return new ConnectToAssessmentSessionCommand(
            assessmentSessionId,
            context.LanguageId,
            context.UserId,
            context.UserName);
    }
}