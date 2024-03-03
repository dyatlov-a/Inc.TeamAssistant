using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed class Story
{
	public static readonly Story Empty = new()
	{
		LanguageId = LanguageSettings.DefaultLanguageId,
		Title = string.Empty
	};

	public Guid Id { get; private set; }
	public StoryType StoryType { get; private set; }
	public DateTimeOffset Created { get; private set; }
	public Guid TeamId { get; private set; }
	public long ChatId { get; private set; }
	public long ModeratorId { get; private set; }
	public LanguageId LanguageId { get; private set; } = default!;
	public string Title { get; private set; } = default!;
	public int? ExternalId { get; private set; }

	private readonly List<StoryForEstimate> _storyForEstimates = new();
    public IReadOnlyCollection<StoryForEstimate> StoryForEstimates => _storyForEstimates;

    public ICollection<string> Links { get; private set; } = new List<string>();

    private Story()
    {
    }
    
    public Story(Guid teamId, StoryType storyType, long chatId, long moderatorId, LanguageId languageId, string title)
		: this()
    {
	    if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
        
		Id = Guid.NewGuid();
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
		const string unknownValue = "?";

		if (!EstimateEnded)
			return unknownValue;
		
		return StoryType switch
		{
			StoryType.Scrum => GetScrumTotal()?.ToString(".## SP") ?? unknownValue,
			StoryType.Kanban => GetKanbanTotal().ToString(),
			_ => unknownValue
		};
	}

	public void Estimate(long participantId, AssessmentValue.Value value)
    {
        var storyForEstimate = _storyForEstimates.SingleOrDefault(a => a.ParticipantId == participantId);

        if (storyForEstimate is null)
            throw new TeamAssistantUserException(Messages.Appraiser_MissingTaskForEvaluate);

        storyForEstimate.SetValue(value);
	}

	public void AddStoryForEstimate(StoryForEstimate storyForEstimate)
    {
        if (storyForEstimate is null)
            throw new ArgumentNullException(nameof(storyForEstimate));

        _storyForEstimates.Add(storyForEstimate);
	}

	public void AddLink(string link)
	{
		if (string.IsNullOrWhiteSpace(link))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(link));
		
		Links.Add(link);
	}

	public void Reset(long participantId)
	{
		if (ModeratorId != participantId)
			throw new TeamAssistantException("User has not rights for action.");
	    
        foreach (var storyForEstimate in _storyForEstimates)
            storyForEstimate.Reset();
	}

	public void Accept(long participantId)
	{
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
	
	public bool EstimateEnded => StoryForEstimates.All(s => s.Value != AssessmentValue.Value.None);
	
	private decimal? GetScrumTotal()
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

	private AssessmentValue.Value GetKanbanTotal()
	{
		var values = _storyForEstimates
			.Where(i => i.Value > AssessmentValue.Value.NoIdea)
			.OrderBy(i => i.Value)
			.Select(i => i.Value)
			.ToArray();

		var index = values.Length / 2;
		return values[index];
	}
}