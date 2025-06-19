using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroAssessment;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetRetroAssessment;

internal sealed class SetRetroAssessmentCommandHandler : IRequestHandler<SetRetroAssessmentCommand>
{
    private readonly IPersonResolver _personResolver;
    private readonly IRetroAssessmentRepository _repository;

    public SetRetroAssessmentCommandHandler(IPersonResolver personResolver, IRetroAssessmentRepository repository)
    {
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task Handle(SetRetroAssessmentCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var assessment = await _repository.Find(command.SessionId, currentPerson.Id, token)
            ?? new RetroAssessment(command.SessionId, currentPerson.Id);

        await _repository.Upsert(assessment.ChangeValue(command.Value), token);
    }
}