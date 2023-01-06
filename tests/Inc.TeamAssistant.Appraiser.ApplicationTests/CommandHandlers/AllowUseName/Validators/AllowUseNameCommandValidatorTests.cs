using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AllowUseName.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.AllowUseName.Validators;

public sealed class AllowUseNameCommandValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly AllowUseNameCommand _validCommand;
    private readonly AllowUseNameCommandValidator _target;

    public AllowUseNameCommandValidatorTests()
    {
        _validCommand = _fixture.Create<AllowUseNameCommand>();
        _target = new(new AppraiserValidator());
    }

    [Fact]
    public void Constructor_AppraiserValidatorIsNull_ThrowsException()
    {
        AllowUseNameCommandValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
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
    public void Validate_ValidCommand_ShouldBeValid()
    {
        var actual = _target.Validate(_validCommand);

        Assert.True(actual.IsValid);
    }
}