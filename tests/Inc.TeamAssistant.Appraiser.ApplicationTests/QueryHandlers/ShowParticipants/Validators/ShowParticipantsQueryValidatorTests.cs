using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Application.QueryHandlers.ShowParticipants.Validators;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.QueryHandlers.ShowParticipants.Validators;

public sealed class ShowParticipantsQueryValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly ShowParticipantsQuery _validQuery;
    private readonly ShowParticipantsQueryValidator _target;

    public ShowParticipantsQueryValidatorTests()
    {
        _validQuery = _fixture.Create<ShowParticipantsQuery>();
        _target = new(new AppraiserValidator());
    }

    [Fact]
    public void Constructor_ModeratorValidatorIsNull_ThrowsException()
    {
        ShowParticipantsQueryValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public async Task Validate_ValidQuery_ShouldBeValid()
    {
        var actual = await _target.ValidateAsync(_validQuery);

        Assert.True(actual.IsValid);
    }

    [Fact]
    public async Task Validate_SetModeratorValidator_ShouldBeCalled()
    {
        var query = _validQuery with
        {
            AppraiserName = string.Empty
        };

        var actual = await _target.ValidateAsync(query);

        Assert.False(actual.IsValid);
    }
}