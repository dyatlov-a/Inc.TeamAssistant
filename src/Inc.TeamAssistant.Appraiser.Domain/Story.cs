using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Domain.States;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed class Story : IStoryAccessor
{
	public static readonly Story Empty = new(nameof(Story), Array.Empty<string>());

	public string Title { get; }
	public int? ExternalId { get; private set; }

	private readonly List<StoryForEstimate> _storyForEstimates;
    public IReadOnlyCollection<StoryForEstimate> StoryForEstimates => _storyForEstimates;

    private readonly List<string> _links;
    public IReadOnlyCollection<string> Links => _links;

    public Story(string title, IReadOnlyCollection<string> links)
    {
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
        if (links is null)
            throw new ArgumentNullException(nameof(links));

		_storyForEstimates = new();
        Title = title;
        _links = new();

        foreach (var link in links)
            _links.Add(link);
	}

    public void SetExternalId(int storyExternalId) => ExternalId = storyExternalId;

    public bool EstimateEnded() => StoryForEstimates.All(s => s.Value != AssessmentValue.Value.None);

	public decimal? GetTotal()
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

	void IStoryAccessor.RemoveStoryForEstimate(ParticipantId participantId)
	{
		if (participantId is null)
			throw new ArgumentNullException(nameof(participantId));

		var storyForEstimate = _storyForEstimates.SingleOrDefault(a => a.Participant.Id == participantId);

		if (storyForEstimate is null)
			throw new AppraiserUserException(Messages.MissingTaskForEvaluate);

		_storyForEstimates.Remove(storyForEstimate);
	}

	void IStoryAccessor.Estimate(ParticipantId participantId, AssessmentValue.Value value)
    {
        if (participantId is null)
            throw new ArgumentNullException(nameof(participantId));

        var storyForEstimate = _storyForEstimates.SingleOrDefault(a => a.Participant.Id == participantId);

        if (storyForEstimate is null)
            throw new AppraiserUserException(Messages.MissingTaskForEvaluate);

        storyForEstimate.SetValue(value);
	}

	void IStoryAccessor.AddStoryForEstimate(StoryForEstimate storyForEstimate)
    {
        if (storyForEstimate is null)
            throw new ArgumentNullException(nameof(storyForEstimate));

        _storyForEstimates.Add(storyForEstimate);
	}

	void IStoryAccessor.Reset()
    {
        foreach (var storyForEstimate in _storyForEstimates)
            storyForEstimate.Reset();
	}
}