using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ChangeLanguage;

internal sealed class ChangeLanguageCommandHandler : IRequestHandler<ChangeLanguageCommand, ChangeLanguageResult>
{
    private readonly IAssessmentSessionRepository _repository;

    public ChangeLanguageCommandHandler(IAssessmentSessionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Task<ChangeLanguageResult> Handle(ChangeLanguageCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

        assessmentSession.ChangeLanguage(command.ModeratorId, command.LanguageId);

        return Task.FromResult(new ChangeLanguageResult(command.LanguageId));
    }
}