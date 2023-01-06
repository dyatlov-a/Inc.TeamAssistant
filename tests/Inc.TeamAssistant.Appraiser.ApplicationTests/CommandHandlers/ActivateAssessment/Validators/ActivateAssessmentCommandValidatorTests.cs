using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessment.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.ActivateAssessment;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.ActivateAssessment.Validators;

public sealed class ActivateAssessmentCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Fixture _fixture = new();
    private readonly ActivateAssessmentCommand _validCommand;
    private readonly ActivateAssessmentCommandValidator _target;

    public ActivateAssessmentCommandValidatorTests()
    {
        _validCommand = _fixture.Create<ActivateAssessmentCommand>();
        _target = new(new ModeratorValidator());
    }

    [Fact]
    public void Constructor_ModeratorValidatorIsNull_ThrowsException()
    {
        ActivateAssessmentCommandValidator Actual() => new(null!);

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

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("Title", true)]
    public void Validate_Title_ShouldBe(string title, bool isValid)
    {
        var command = _validCommand with
        {
            Title = title
        };

        var actual = _target.Validate(command);

        Assert.Equal(isValid, actual.IsValid);
    }
}