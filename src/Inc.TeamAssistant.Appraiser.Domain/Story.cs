using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed class Story
{
	const string UnknownValue = "?";
	
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
	public AssessmentValue.Value? TotalValue { get; private set; }

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

	public string GetTotalValue()
	{
		if (!EstimateEnded)
			return UnknownValue;
		
		return StoryType switch
		{
			StoryType.Scrum => CalculateMean()?.ToString(".## SP") ?? UnknownValue,
			StoryType.Kanban => CalculateMedianValue()?.ToString().ToUpperInvariant() ?? UnknownValue,
			_ => UnknownValue
		};
	}

	public string GetAcceptedValue()
	{
		if (!TotalValue.HasValue)
			return UnknownValue;
		
		return StoryType switch
		{
			StoryType.Scrum => $"{(int)TotalValue.Value}SP",
			StoryType.Kanban => TotalValue.Value.ToString().ToUpperInvariant(),
			_ => UnknownValue
		};
	}

	public bool Estimate(long participantId, AssessmentValue.Value value)
    {
	    if (Accepted)
		    return false;
	    
        var storyForEstimate = _storyForEstimates.SingleOrDefault(a => a.ParticipantId == participantId);

        if (storyForEstimate is null)
            throw new TeamAssistantUserException(Messages.Appraiser_MissingTaskForEvaluate);
        
        return storyForEstimate.SetValue(value);
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
			if (storyForEstimate.Value == AssessmentValue.Value.None)
				storyForEstimate.SetValue(AssessmentValue.Value.NoIdea);
	}
	
	public IReadOnlyCollection<AssessmentValue.Value> GetAssessments()
	{
		return StoryType switch
		{
			StoryType.Scrum => AssessmentValue.ScrumAssessments,
			StoryType.Kanban => AssessmentValue.KanbanAssessments,
			_ => throw new TeamAssistantException("StoryType is not valid.")
		};
	}

	public void Accept(long participantId, AssessmentValue.Value value)
	{
		if (ModeratorId != participantId)
			throw new TeamAssistantException("User has not rights for action.");

		TotalValue = value;
	}
	
	public bool EstimateEnded => StoryForEstimates.All(s => s.Value != AssessmentValue.Value.None);
	
	private decimal? CalculateMean()
	{
		var values = _storyForEstimates
			.Where(i => i.Value > AssessmentValue.Value.NoIdea)
			.Select(i => (int)i.Value)
			.ToArray();

		var result = values.Any()
			? (decimal?)values.Sum() / values.Length
			: null;

		return result;
	}

	private AssessmentValue.Value? CalculateMedianValue()
	{
		var values = _storyForEstimates
			.Where(i => i.Value > AssessmentValue.Value.NoIdea)
			.OrderBy(i => i.Value)
			.Select(i => i.Value)
			.ToArray();
		
		if (!values.Any())
			return null;

		var middle = values.Length / 2;
		return values[middle];
	}

	public string? CalculateMedian()
	{
		if (StoryType == StoryType.Kanban)
			return null;
		if (!EstimateEnded)
			return UnknownValue;
		
		var values = _storyForEstimates
			.Where(i => i.Value > AssessmentValue.Value.NoIdea)
			.OrderBy(i => i.Value)
			.Select(i => (int)i.Value)
			.ToArray();
		
		if (!values.Any())
			return null;

		var middle = values.Length / 2;
		
		var value = values.Length % 2 == 0
			? (values[middle] + values[middle - 1]) / 2m
			: values[middle];

		return value.ToString(".## SP");
	}

	public IReadOnlyCollection<AssessmentValue.Value> GetTopValues()
	{
		return _storyForEstimates
			.Where(i => i.Value > AssessmentValue.Value.NoIdea)
			.Select(t => t.Value)
			.Distinct()
			.OrderByDescending(t => (int)t)
			.Take(2)
			.ToArray();
	}
}