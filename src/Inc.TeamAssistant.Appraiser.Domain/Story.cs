using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed class Story
{
	public Guid Id { get; private set; }
	public Guid BotId { get; private set; }
	public StoryType StoryType { get; private set; }
	public DateTimeOffset Created { get; private set; }
	public Guid TeamId { get; private set; }
	public long ChatId { get; private set; }
	public long ModeratorId { get; private set; }
	public LanguageId LanguageId { get; private set; } = default!;
	public string Title { get; private set; } = default!;
	public int? ExternalId { get; private set; }
	public bool Accepted => TotalValue.HasValue;
	public int? TotalValue { get; private set; }
	public int RoundsCount { get; private set; }
	public string? Url { get; private set; }

	private readonly List<StoryForEstimate> _storyForEstimates = new();
    public IReadOnlyCollection<StoryForEstimate> StoryForEstimates => _storyForEstimates;

    private IEstimationStrategy EstimationStrategy => EstimationStrategyFactory.Create(StoryType);

    private Story()
    {
    }
    
    public Story(
	    Guid id,
	    Guid botId,
	    Guid teamId,
	    StoryType storyType,
	    long chatId,
	    long moderatorId,
	    LanguageId languageId,
	    string title)
		: this()
    {
	    ArgumentException.ThrowIfNullOrWhiteSpace(title);
        
		Id = id;
		BotId = botId;
		StoryType = storyType;
		Created = DateTimeOffset.UtcNow;
		ChatId = chatId;
		ModeratorId = moderatorId;
		LanguageId = languageId ?? throw new ArgumentNullException(nameof(languageId));
		TeamId = teamId;
        Title = title;
        RoundsCount = 1;
    }

    public Story SetExternalId(int storyExternalId)
    {
	    ExternalId = storyExternalId;

	    return this;
    }

	public bool? Estimate(long participantId, int value)
    {
	    if (Accepted)
		    return false;
	    
        var storyForEstimate = _storyForEstimates.SingleOrDefault(a => a.ParticipantId == participantId);

        return storyForEstimate?.SetValue(ToEstimation(value));
	}

	public void AddStoryForEstimate(StoryForEstimate storyForEstimate)
    {
        ArgumentNullException.ThrowIfNull(storyForEstimate);

        _storyForEstimates.Add(storyForEstimate);
	}

	public void AddLink(string link)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(link);

		Url = link;
	}

	public Story Reset(long participantId, bool hasManagerAccess)
	{
		if (Accepted)
			return this;

		var story = EnsureRights(participantId, hasManagerAccess);

		foreach (var storyForEstimate in story.StoryForEstimates)
			storyForEstimate.Reset();

		story.RoundsCount++;

		return story;
	}

	public Story Finish(long participantId, bool hasManagerAccess)
	{
		if (Accepted)
			return this;

		var story = EnsureRights(participantId, hasManagerAccess);

		foreach (var storyForEstimate in story.StoryForEstimates)
			if (storyForEstimate.Value == Estimation.None.Value)
				storyForEstimate.SetValue(Estimation.NoIdea);

		return story;
	}

	public IEnumerable<Estimation> GetAssessments() => EstimationStrategy.GetValues();
	public Estimation ToEstimation(int value) => EstimationStrategy.GetValue(value);

	public Estimation Accept(long participantId, bool hasManagerAccess, int value)
	{
		var story = EnsureRights(participantId, hasManagerAccess);
		var totalEstimation = story.ToEstimation(value);
		
		story.TotalValue = totalEstimation.Value;

		return totalEstimation;
	}
	
	public bool EstimateEnded => StoryForEstimates.All(s => s.Value != Estimation.None.Value);

	public IReadOnlyCollection<Estimation> GetTopValues()
	{
		return _storyForEstimates
			.Where(i => i.Value > Estimation.NoIdea.Value)
			.Select(t => t.Value)
			.Distinct()
			.OrderByDescending(t => t)
			.Select(ToEstimation)
			.Take(2)
			.ToArray();
	}
	
	public Estimation AcceptedValue => TotalValue.HasValue
		? ToEstimation(TotalValue.Value)
		: Estimation.None;

	public Estimation CalculateMean() => EstimationStrategy.CalculateMean(this);
    
	public Estimation CalculateMedian() => EstimationStrategy.CalculateMedian(this);

	public int GetWeight() => EstimationStrategy.GetWeight(this);
	
	private Story EnsureRights(long participantId, bool hasManagerAccess)
	{
		if (ModeratorId != participantId && !hasManagerAccess)
			throw new TeamAssistantUserException(Messages.Connector_HasNoRights, participantId);

		return this;
	}
}