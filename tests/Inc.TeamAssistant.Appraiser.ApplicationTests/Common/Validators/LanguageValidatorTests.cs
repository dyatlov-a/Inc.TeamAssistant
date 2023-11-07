using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.Common.Validators;

public sealed class LanguageValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Dictionary<string, Dictionary<string, string>> _languages = new()
    {
        ["ru"] = new(),
        ["en"] = new()
    };

    private readonly Fixture _fixture = new();
    private readonly LanguageValidator _target;

    public LanguageValidatorTests()
    {
        var messageProvider = Substitute.For<IMessageProvider>();
        messageProvider.Get().Returns(ServiceResult.Success(_languages));
        _target = new(messageProvider);
    }

    [Fact]
    public void Constructor_MessageProviderIsNull_ThrowsException()
    {
        LanguageValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Theory]
    [InlineData("text")]
    [InlineData("fr")]
    public async Task Validate_LanguageIncorrect_ShouldBeNotValid(string language)
    {
        var command = new ChangeLanguageCommand(
            _fixture.Create<long>(),
            _fixture.Create<ParticipantId>(),
            _fixture.Create<string>(),
            new(language));

        var actual = await _target.ValidateAsync(command);

        Assert.False(actual.IsValid);
    }

    [Fact]
    public async Task Validate_Language_ShouldBeValid()
    {
        foreach (var languageId in _languages.Keys)
        {
            var command = new ChangeLanguageCommand(
                _fixture.Create<long>(),
                _fixture.Create<ParticipantId>(),
                _fixture.Create<string>(),
                new(languageId));

            var actual = await _target.ValidateAsync(command);

            Assert.True(actual.IsValid);
        }
    }
}