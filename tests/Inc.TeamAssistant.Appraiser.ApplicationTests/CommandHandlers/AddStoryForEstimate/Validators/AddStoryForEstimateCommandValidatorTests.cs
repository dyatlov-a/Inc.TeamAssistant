using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryForEstimate.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.AddStoryForEstimate.Validators;

public sealed class AddStoryForEstimateCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Fixture _fixture = new();
    private readonly AddStoryForEstimateCommand _validCommand;
    private readonly AddStoryForEstimateCommandValidator _target;

    public AddStoryForEstimateCommandValidatorTests()
    {
        _validCommand = _fixture.Create<AddStoryForEstimateCommand>();
        _target = new(new AppraiserValidator());
    }

    [Fact]
    public void Constructor_AppraiserValidatorIsNull_ThrowsException()
    {
        AddStoryForEstimateCommandValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldBeValid()
    {
        var actual = _target.Validate(_validCommand);

        Assert.True(actual.IsValid);
    }

    [Fact]
    public void Validate_SetAppraiserValidator_ShouldBeCalled()
    {
        var command = _validCommand with
        {
            AppraiserName = string.Empty
        };

        var actual = _target.Validate(command);

        Assert.False(actual.IsValid);
    }

    [Fact]
    public void Validate_AssessmentSessionIdIsEmpty_ShouldBeNotValid()
    {
        var command = _validCommand with
        {
            AssessmentSessionId = default!
        };

        var actual = _target.Validate(command);

        Assert.False(actual.IsValid);
    }
}