using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ChangeLanguage;

internal sealed class ChangeLanguageCommandHandler : IRequestHandler<ChangeLanguageCommand, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IMessageBuilder _messageBuilder;

    public ChangeLanguageCommandHandler(IAssessmentSessionRepository repository, IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(ChangeLanguageCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

        assessmentSession.ChangeLanguage(command.ModeratorId, command.LanguageId);
        
        var message = await _messageBuilder.Build(
            Messages.LanguageChanged,
            command.LanguageId,
            command.LanguageId.Value);

        return CommandResult.Build(NotificationMessage.Create(command.TargetChatId, message));
    }
}