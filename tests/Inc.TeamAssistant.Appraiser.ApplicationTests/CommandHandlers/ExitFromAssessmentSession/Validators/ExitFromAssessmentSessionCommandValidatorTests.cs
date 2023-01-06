using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ExitFromAssessmentSession.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.ExitFromAssessmentSession.Validators;

public sealed class ExitFromAssessmentSessionCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Fixture _fixture = new();
    private readonly ExitFromAssessmentSessionCommand _validSessionCommand;
    private readonly ExitFromAssessmentSessionCommandValidator _target;

    public ExitFromAssessmentSessionCommandValidatorTests()
    {
        _validSessionCommand = _fixture.Create<ExitFromAssessmentSessionCommand>();
        _target = new(new AppraiserValidator());
    }

    [Fact]
    public void Constructor_AppraiserValidatorIsNull_ThrowsException()
    {
        ExitFromAssessmentSessionCommandValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldBeValid()
    {
        var actual = _target.Validate(_validSessionCommand);

        Assert.True(actual.IsValid);
    }

    [Fact]
    public void Validate_SetAppraiserValidator_ShouldBeCalled()
    {
        var command = _validSessionCommand with
        {
            AppraiserName = string.Empty
        };

        var actual = _target.Validate(command);

        Assert.False(actual.IsValid);
    }
}