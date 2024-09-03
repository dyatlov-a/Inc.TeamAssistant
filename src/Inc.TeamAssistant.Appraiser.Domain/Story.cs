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

	private readonly List<StoryForEstimate> _storyForEstimates;
    public IReadOnlyCollection<StoryForEstimate> StoryForEstimates => _storyForEstimates;

    public ICollection<string> Links { get; private set; }

    private IEstimationStrategy EstimationStrategy => EstimationStrategyFactory.Create(StoryType);

    private Story()
    {
	    _storyForEstimates = new();
	    Links = new List<string>();
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
	}

    public void SetExternalId(int storyExternalId) => ExternalId = storyExternalId;

	public bool Estimate(long participantId, int value)
    {
	    if (Accepted)
		    return false;
	    
        var storyForEstimate = _storyForEstimates.SingleOrDefault(a => a.ParticipantId == participantId);
        if (storyForEstimate is null)
            throw new TeamAssistantUserException(Messages.Appraiser_MissingTaskForEvaluate);
        
        return storyForEstimate.SetValue(ToEstimation(value));
	}

	public void AddStoryForEstimate(StoryForEstimate storyForEstimate)
    {
        ArgumentNullException.ThrowIfNull(storyForEstimate);

        _storyForEstimates.Add(storyForEstimate);
	}

	public void AddLink(string link)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(link);
		
		Links.Add(link);
	}

	public void Reset(long participantId)
	{
		if (Accepted)
			return;
		
		if (ModeratorId != participantId)
			throw new TeamAssistantException("User has not rights for action.");
	    
        foreach (var storyForEstimate in _storyForEstimates)
            storyForEstimate.Reset();
	}

	public void Finish(long participantId)
	{
		if (Accepted)
			return;
		
		if (ModeratorId != participantId)
			throw new TeamAssistantException("User has not rights for action.");
		
		foreach (var storyForEstimate in _storyForEstimates)
			if (storyForEstimate.Value == Estimation.None.Value)
				storyForEstimate.SetValue(Estimation.NoIdea);
	}
	
	public IEnumerable<Estimation> GetAssessments() => EstimationStrategy.GetValues();
	public Estimation ToEstimation(int value) => EstimationStrategy.GetValue(value);

	public void Accept(long participantId, int value)
	{
		if (ModeratorId != participantId)
			throw new TeamAssistantException("User has not rights for action.");

		TotalValue = ToEstimation(value).Value;
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
}