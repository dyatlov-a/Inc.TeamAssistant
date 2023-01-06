using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.ReVoteEstimate.Validators;

public sealed class ReVoteEstimateCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Fixture _fixture = new();
    private readonly ReVoteEstimateCommand _validCommand;
    private readonly ReVoteEstimateCommandValidator _target;

    public ReVoteEstimateCommandValidatorTests()
    {
        _validCommand = _fixture.Create<ReVoteEstimateCommand>();
        _target = new(new ModeratorValidator());
    }

    [Fact]
    public void Constructor_ModeratorValidatorIsNull_ThrowsException()
    {
        ReVoteEstimateCommandValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldBeValid()
    {
        var actual = _target.Validate(_validCommand);

        Assert.True(actual.IsValid);
    }

    [Fact]
    public void Validate_SetModeratorValidator_ShouldBeCalled()
    {
        var command = _validCommand with
        {
            ModeratorName = string.Empty
        };

        var actual = _target.Validate(command);

        Assert.False(actual.IsValid);
    }
}