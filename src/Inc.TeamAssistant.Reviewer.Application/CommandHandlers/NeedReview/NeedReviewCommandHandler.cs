using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.NeedReview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.NeedReview;

internal sealed class NeedReviewCommandHandler : IRequestHandler<NeedReviewCommand, CommandResult>
{
    private readonly IDraftTaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;

    public NeedReviewCommandHandler(
        IDraftTaskForReviewRepository repository,
        IReviewMessageBuilder reviewMessageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
    }

    public async Task<CommandResult> Handle(NeedReviewCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var nextReviewerType = Enum.Parse<NextReviewerType>(command.Strategy);
        var draft = new DraftTaskForReview(
            Guid.NewGuid(),
            command.TeamId,
            command.MessageContext.Person.Id,
            command.MessageContext.ChatMessage.ChatId,
            command.MessageContext.ChatMessage.MessageId,
            command.Description,
            DateTimeOffset.UtcNow,
            nextReviewerType);

        await _repository.Upsert(draft.SetTargetPerson(command.MessageContext.TargetPersonId), token);

        var notification = await _reviewMessageBuilder.Build(draft, command.MessageContext.LanguageId, token);
        return CommandResult.Build(notification);
    }
}