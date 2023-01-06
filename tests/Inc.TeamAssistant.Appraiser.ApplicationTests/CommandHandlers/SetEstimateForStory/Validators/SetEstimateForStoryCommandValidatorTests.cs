using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.SetEstimateForStory.Validators;

public sealed class SetEstimateForStoryCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Fixture _fixture = new();
    private readonly SetEstimateForStoryCommand _validCommand;
    private readonly SetEstimateForStoryCommandValidator _target;

    public SetEstimateForStoryCommandValidatorTests()
    {
        _validCommand = _fixture.Create<SetEstimateForStoryCommand>() with
        {
            Value = (int?) AssessmentValue.Value.Sp1
        };
        _target = new(new AppraiserValidator());
    }

    [Fact]
    public void Constructor_AppraiserValidatorIsNull_ThrowsException()
    {
        SetEstimateForStoryCommandValidator Actual() => new(null!);

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

    [Theory]
    [MemberData(nameof(CreateValues))]
    public void Validate_SetValue_ShouldBe(int? value, bool isValid)
    {
        var command = _validCommand with
        {
            Value = value
        };

        var actual = _target.Validate(command);

        Assert.Equal(isValid, actual.IsValid);
    }

    public static IEnumerable<object[]> CreateValues()
    {
        foreach (var value in Enum.GetValues<AssessmentValue.Value>())
            yield return new object[] { (int?)value, true};

        yield return  new object[] { (int?)AssessmentValue.Value.Unknown - 1, false};
        yield return  new object[] { (int?)AssessmentValue.Value.Sp21 + 1, false};
    }
}