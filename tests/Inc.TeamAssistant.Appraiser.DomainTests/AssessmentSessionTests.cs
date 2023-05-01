using AutoFixture;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Primitives;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.DomainTests;

public sealed class AssessmentSessionTests
{
	private readonly Fixture _fixture = new ();
	private readonly Participant _moderator;
	private readonly AssessmentSession _target;

	public AssessmentSessionTests()
	{
		_moderator = _fixture.Create<Participant>();
		_target = new(_fixture.Create<long>(), _moderator, _fixture.Create<LanguageId>());
	}

	[Theory]
	[InlineData(AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp2)]
	[InlineData(AssessmentValue.Value.Sp3)]
	[InlineData(AssessmentValue.Value.Sp5)]
	[InlineData(AssessmentValue.Value.Sp8)]
	[InlineData(AssessmentValue.Value.Sp13)]
	[InlineData(AssessmentValue.Value.Sp21)]
	public void Estimate_Value_ReturnsValue(AssessmentValue.Value value)
	{
		_target.Activate(_moderator.Id, _fixture.Create<string>());

		_target.StartStorySelection(_moderator.Id);
		_target.Connect(_moderator.Id, _moderator.Name);
		_target.StorySelected(_moderator.Id, _fixture.Create<string>(), _fixture.Create<string[]>());
		_target.Estimate(_moderator, value);

		var actual = _target.CurrentStory.GetTotal();

		Assert.Equal((decimal?)value, actual);
	}

	[Theory]
	[InlineData(AssessmentValue.Value.Sp1, AssessmentValue.Value.Sp21)]
	[InlineData(AssessmentValue.Value.Sp2, AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp3, AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp5, AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp8, AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp13, AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp21, AssessmentValue.Value.Sp1)]
	public void Reset_SecondValue_ReturnsSecondValue(
		AssessmentValue.Value firstValue,
		AssessmentValue.Value secondValue)
	{
		var participant = new Participant(_fixture.Create<ParticipantId>(), _fixture.Create<string>());
		_target.Activate(_moderator.Id, _fixture.Create<string>());

		_target.StartStorySelection(_moderator.Id);
		_target.Connect(_moderator.Id, _moderator.Name);
		_target.Connect(participant.Id, participant.Name);
		_target.StorySelected(_moderator.Id, _fixture.Create<string>(), _fixture.Create<string[]>());
		_target.Estimate(_moderator, firstValue);

		_target.Estimate(_moderator, secondValue);

		var actual = _target.CurrentStory.StoryForEstimates
			.Where(s => s.Value != AssessmentValue.Value.None)
			.Sum(s => (decimal)s.Value);

		Assert.Equal((decimal?)secondValue, actual);
	}

	[Theory]
	[InlineData(AssessmentValue.Value.Sp1, AssessmentValue.Value.Sp21)]
	[InlineData(AssessmentValue.Value.Sp2, AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp3, AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp5, AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp8, AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp13, AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp21, AssessmentValue.Value.Sp1)]
	public void Reset_ResetValue_ReturnsSecondValue(AssessmentValue.Value firstValue, AssessmentValue.Value secondValue)
	{
		_target.Activate(_moderator.Id, _fixture.Create<string>());

		_target.StartStorySelection(_moderator.Id);
		_target.Connect(_moderator.Id, _moderator.Name);
		_target.StorySelected(_moderator.Id, _fixture.Create<string>(), _fixture.Create<string[]>());
		_target.Estimate(_moderator, firstValue);

		_target.Reset(_moderator.Id);
		_target.Estimate(_moderator, secondValue);

		var actual = _target.CurrentStory.GetTotal();

		Assert.Equal((decimal?)secondValue, actual);
	}

	[Theory]
	[InlineData(AssessmentValue.Value.Sp1)]
	[InlineData(AssessmentValue.Value.Sp2)]
	[InlineData(AssessmentValue.Value.Sp3)]
	[InlineData(AssessmentValue.Value.Sp5)]
	[InlineData(AssessmentValue.Value.Sp8)]
	[InlineData(AssessmentValue.Value.Sp13)]
	[InlineData(AssessmentValue.Value.Sp21)]
	public void Estimate_SecondStory_ReturnsValue(AssessmentValue.Value value)
	{
		_target.Activate(_moderator.Id, _fixture.Create<string>());

		_target.StartStorySelection(_moderator.Id);
		_target.Connect(_moderator.Id, _moderator.Name);
		_target.StorySelected(_moderator.Id, _fixture.Create<string>(), _fixture.Create<string[]>());
		_target.Estimate(_moderator, value);

		var secondStory = _fixture.Create<string>();
		_target.StartStorySelection(_moderator.Id);
		_target.StorySelected(_moderator.Id, secondStory, _fixture.Create<string[]>());
		_target.Estimate(_moderator, value);

		var actual = _target.CurrentStory.GetTotal();

		Assert.Equal((decimal?)value, actual);
		Assert.Equal(secondStory, _target.CurrentStory.Title);
	}

    [Fact]
    public void Connect_ParticipantIdIsNull_ThrowsException()
    {
        _target.Activate(_moderator.Id, _fixture.Create<string>());

        void Actual() => _target.Connect(null!, _fixture.Create<string>());

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Connect_InvalidParticipantName_ThrowsException(string name)
    {
        _target.Activate(_moderator.Id, _fixture.Create<string>());

        void Actual() => _target.Connect(_fixture.Create<ParticipantId>(), name);

        Assert.Throws<ArgumentException>(Actual);
    }

    [Fact]
    public void Connect_ParticipantIdAlreadyExists_ThrowsException()
    {
        var participantId = _fixture.Create<ParticipantId>();
        _target.Activate(_moderator.Id, _fixture.Create<string>());
        _target.Connect(participantId, _fixture.Create<string>());

        void Actual() => _target.Connect(participantId, _fixture.Create<string>());

        var ex = Assert.Throws<AppraiserUserException>(Actual);
        Assert.Equal(Messages.AppraiserConnectWithError, ex.MessageId);
    }

    [Fact]
    public void Connect_Participant_ShouldBeConnected()
    {
        var otherParticipantId = _fixture.Create<ParticipantId>();
        _target.Activate(_moderator.Id, _fixture.Create<string>());

        _target.Connect(otherParticipantId, _fixture.Create<string>());

        Assert.Contains(_target.Participants, p => p.Id == otherParticipantId);
    }

    [Fact]
    public void Disconnect_IdleAssessmentSession_ShouldBeDisconnected()
    {
        var otherParticipantId = _fixture.Create<ParticipantId>();
        _target.Activate(_moderator.Id, _fixture.Create<string>());
        _target.Connect(otherParticipantId, _fixture.Create<string>());

        _target.Disconnect(otherParticipantId);

        Assert.DoesNotContain(_target.Participants, p => p.Id == otherParticipantId);
    }

    [Fact]
    public void Disconnect_Moderator_ThrowsException()
    {
        _target.Activate(_moderator.Id, _fixture.Create<string>());

        void Actual() => _target.Disconnect(_moderator.Id);

        var ex = Assert.Throws<AppraiserUserException>(Actual);
        Assert.Equal(Messages.ModeratorCannotDisconnectedFromSession, ex.MessageId);
    }
}