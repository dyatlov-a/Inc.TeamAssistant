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

	private readonly List<StoryForEstimate> _storyForEstimates = new();
    public IReadOnlyCollection<StoryForEstimate> StoryForEstimates => _storyForEstimates;

    public ICollection<string> Links { get; private set; } = new List<string>();

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
	}

    public void SetExternalId(int storyExternalId) => ExternalId = storyExternalId;

	public bool Estimate(long participantId, int value)
    {
	    if (Accepted)
		    return false;
	    
        var storyForEstimate = _storyForEstimates.SingleOrDefault(a => a.ParticipantId == participantId);
        if (storyForEstimate is null)
            throw new TeamAssistantUserException(Messages.Appraiser_MissingTaskForEvaluate);
        
        return storyForEstimate.SetValue(EstimationProvider.Create(StoryType, value));
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
			if (storyForEstimate.Value == Estimation.None)
				storyForEstimate.SetValue(EstimationProvider.Create(StoryType, Estimation.NoIdea));
	}
	
	public IReadOnlyCollection<Estimation> GetAssessments() => EstimationProvider.GetAssessments(StoryType);

	public void Accept(long participantId, int value)
	{
		if (ModeratorId != participantId)
			throw new TeamAssistantException("User has not rights for action.");

		TotalValue = EstimationProvider.Create(StoryType, value).Value;
	}
	
	public bool EstimateEnded => StoryForEstimates.All(s => s.Value != Estimation.None);

	public IReadOnlyCollection<Estimation> GetTopValues()
	{
		return _storyForEstimates
			.Where(i => i.Value > Estimation.NoIdea)
			.Select(t => t.Value)
			.Distinct()
			.OrderByDescending(t => t)
			.Select(t => EstimationProvider.Create(StoryType, t))
			.Take(2)
			.ToArray();
	}
}