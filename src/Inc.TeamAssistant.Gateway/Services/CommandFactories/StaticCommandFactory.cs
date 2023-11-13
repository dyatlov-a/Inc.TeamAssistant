using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Appraiser.Model.Commands.ShowHelp;
using Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Languages;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.CommandFactories;

internal sealed class StaticCommandFactory : ICommandFactory
{
    private readonly Dictionary<string, Func<CommandContext, IRequest<CommandResult>?>> _commandList;
    private readonly HashSet<string> _targets;

	public StaticCommandFactory()
    {
        _targets = new(AssessmentValue.GetAssessments.Select(a => a.ToString()));
        _commandList = BuildCommands();
    }

	public IRequest<CommandResult>? TryCreate(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

		var commandKey = _commandList.Keys.FirstOrDefault(k => context.Cmd.StartsWith(
			k,
			StringComparison.InvariantCultureIgnoreCase));

        return commandKey is null ? null : _commandList[commandKey](context);
    }

    private Dictionary<string, Func<CommandContext, IRequest<CommandResult>?>> BuildCommands()
	{
        var commands = new Dictionary<string, Func<CommandContext, IRequest<CommandResult>?>>
        {
            [CommandList.Start] = CreateConnectAppraiserCommand,
            [CommandList.ExitFromAssessmentSession] = c => new ExitFromAssessmentSessionCommand(c.ChatId, c.UserId, c.UserName),
            [CommandList.CreateAssessmentSession] = c => new CreateAssessmentSessionCommand(c.ChatId, c.UserId, c.UserName, c.LanguageId),
            [CommandList.AddStoryToAssessmentSession] = CreateStartStorySelectionCommand
        };

        foreach (var language in LanguageSettings.LanguageIds)
        {
            var command = string.Format(CommandList.ChangeLanguageForAssessmentSession, language.Value);
            commands.Add(command, c => new ChangeLanguageCommand(c.ChatId, c.UserId, c.UserName, language));
        }

        foreach (var target in _targets)
            commands.Add(target, CreateEstimateStoryCommand);
        
        commands.Add(CommandList.AcceptEstimate, c => new AcceptEstimateCommand(c.UserId, c.UserName));
        commands.Add(CommandList.ReVoteEstimate, c => new ReVoteEstimateCommand(c.UserId, c.UserName));
        commands.Add(CommandList.FinishAssessmentSession, c => new FinishAssessmentSessionCommand(c.ChatId, c.UserId, c.UserName));
        commands.Add(CommandList.Help, c => new ShowHelpCommand(c.ChatId, c.LanguageId));

        return commands;
    }

    private StartStorySelectionCommand CreateStartStorySelectionCommand(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        return new(context.ChatId, context.UserId, context.UserName);
    }

    private IRequest<CommandResult> CreateEstimateStoryCommand(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        return new SetEstimateForStoryCommand(context.UserId, context.UserName, context.Cmd);
    }

    private IRequest<CommandResult> CreateConnectAppraiserCommand(CommandContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var assessmentSessionIdValue = context.Cmd.Replace(CommandList.Start, string.Empty).Trim();

        var assessmentSessionId = Guid.TryParse(assessmentSessionIdValue, out var value)
            ? new(value)
            : default(AssessmentSessionId);

        return new ConnectToAssessmentSessionCommand(
            context.ChatId,
            assessmentSessionId,
            context.LanguageId,
            context.UserId,
            context.UserName);
    }
}