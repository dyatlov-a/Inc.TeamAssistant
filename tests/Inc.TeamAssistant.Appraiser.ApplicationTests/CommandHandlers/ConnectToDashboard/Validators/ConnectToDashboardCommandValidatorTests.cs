using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectToDashboard.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToDashboard;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.ConnectToDashboard.Validators;

public sealed class ConnectToDashboardCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Fixture _fixture = new();
    private readonly ConnectToDashboardCommand _validCommand;
    private readonly ConnectToDashboardCommandValidator _target;

    public ConnectToDashboardCommandValidatorTests()
    {
        _validCommand = _fixture.Create<ConnectToDashboardCommand>();
        _target = new(new AppraiserValidator());
    }

    [Fact]
    public void Constructor_AppraiserValidatorIsNull_ThrowsException()
    {
        ConnectToDashboardCommandValidator Actual() => new(null!);

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
}