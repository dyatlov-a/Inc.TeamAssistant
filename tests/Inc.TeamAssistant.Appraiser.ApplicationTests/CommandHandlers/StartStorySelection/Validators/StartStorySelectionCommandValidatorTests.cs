using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.StartStorySelection.Validators;

public sealed class StartStorySelectionCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Fixture _fixture = new();
    private readonly StartStorySelectionCommand _validCommand;
    private readonly StartStorySelectionCommandValidator _target;

    public StartStorySelectionCommandValidatorTests()
    {
        _validCommand = _fixture.Create<StartStorySelectionCommand>();
        _target = new(new ModeratorValidator());
    }

    [Fact]
    public void Constructor_ModeratorValidatorIsNull_ThrowsException()
    {
        StartStorySelectionCommandValidator Actual() => new(null!);

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