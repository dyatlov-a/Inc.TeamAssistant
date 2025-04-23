using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview;

internal sealed class MoveToReviewCommandHandler : IRequestHandler<MoveToReviewCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IDraftTaskForReviewRepository _draftRepository;
    private readonly DraftTaskForReviewService _draftService;
    private readonly INextReviewerStrategyFactory _reviewerFactory;

    public MoveToReviewCommandHandler(
        ITaskForReviewRepository repository,
        IReviewMessageBuilder reviewMessageBuilder,
        ITeamAccessor teamAccessor,
        IDraftTaskForReviewRepository draftRepository,
        DraftTaskForReviewService draftService,
        INextReviewerStrategyFactory reviewerFactory)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _draftRepository = draftRepository ?? throw new ArgumentNullException(nameof(draftRepository));
        _draftService = draftService ?? throw new ArgumentNullException(nameof(draftService));
        _reviewerFactory = reviewerFactory ?? throw new ArgumentNullException(nameof(reviewerFactory));
    }

    public async Task<CommandResult> Handle(MoveToReviewCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var botContext = command.MessageContext.Bot;
        
        var draft = await command.DraftId.Required(_draftRepository.Find, token);
        draft.CheckRights(command.MessageContext.Person.Id);
        
        var teammates = await _teamAccessor.GetTeammates(draft.TeamId, DateTimeOffset.UtcNow, token);
        if (!teammates.Any())
            throw new TeamAssistantUserException(Messages.Reviewer_TeamWithoutUsers, draft.TeamId);
        
        var targetTeam = command.MessageContext.EnsureTeam(draft.TeamId);
        var nextReviewerStrategy = await _reviewerFactory.Create(
            draft.TeamId,
            draft.OwnerId,
            draft.GetStrategy(),
            draft.TargetPersonId,
            teammates.Select(t => t.Id).ToArray(),
            excludePersonId: null,
            token);
        var taskForReview = new TaskForReview(
            Guid.NewGuid(),
            draft,
            botContext.Id,
            DateTimeOffset.UtcNow,
            botContext.GetNotificationIntervals(),
            targetTeam.ChatId,
            nextReviewerStrategy.GetReviewer());
        
        await _repository.Upsert(taskForReview, token);

        var notifications = (await _reviewMessageBuilder.Build(
                taskForReview,
                botContext,
                fromOwner: false,
                token))
            .Union(await _draftService.Delete(draft, token))
            .ToArray();
        
        return CommandResult.Build(notifications);
    }
}