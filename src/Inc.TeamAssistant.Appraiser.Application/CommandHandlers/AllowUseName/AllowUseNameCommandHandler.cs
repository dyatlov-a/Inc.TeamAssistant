using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AllowUseName;

internal sealed class AllowUseNameCommandHandler : IRequestHandler<AllowUseNameCommand, AllowUseNameResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IUserSettingsProvider _userSettingsProvider;

    public AllowUseNameCommandHandler(
        IAssessmentSessionRepository repository,
        IUserSettingsProvider userSettingsProvider)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _userSettingsProvider = userSettingsProvider ?? throw new ArgumentNullException(nameof(userSettingsProvider));
    }

    public async Task<AllowUseNameResult> Handle(
        AllowUseNameCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        await _userSettingsProvider.SetUserName(command.AppraiserId, command.AppraiserName, cancellationToken);

        var assessmentSession = _repository.Find(command.AppraiserId);

        if (assessmentSession is not null)
        {
            assessmentSession.SetAppraiserName(command.AppraiserId, command.AppraiserName);

            return new(AssessmentSessionConverter.ConvertTo(assessmentSession));
        }

        return new(null);
    }
}