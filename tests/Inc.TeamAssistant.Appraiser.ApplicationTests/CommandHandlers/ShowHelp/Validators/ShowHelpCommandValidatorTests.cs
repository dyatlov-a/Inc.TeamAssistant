using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ShowHelp.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Commands.ShowHelp;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.ShowHelp.Validators;

public sealed class ShowHelpCommandValidatorTests
{
    private readonly Dictionary<string, Dictionary<string, string>> _languages = new()
    {
        ["ru"] = new(),
        ["en"] = new()
    };

    private readonly Fixture _fixture = new();
    private readonly ShowHelpCommand _validCommand;
    private readonly ShowHelpCommandValidator _target;

    public ShowHelpCommandValidatorTests()
    {
        var messageProvider = Substitute.For<IMessageProvider>();
        messageProvider.Get().Returns(ServiceResult.Success(_languages));

        var messageBuilder = Substitute.For<IMessageBuilder>();
        messageBuilder
            .Build(Arg.Any<MessageId>(), Arg.Any<LanguageId>(), Arg.Any<object[]>())
            .Returns(_fixture.Create<string>());

        _validCommand = new(_fixture.Create<long>(), new("en"));
        _target = new ShowHelpCommandValidator(new LanguageValidator(messageProvider));
    }

    [Fact]
    public void Constructor_OptionsIsNull_ThrowsException()
    {
        ShowHelpCommandValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public async Task Validate_SetLanguageValidator_ShouldBeCalled()
    {
        var query = new ShowHelpCommand(_fixture.Create<long>(), _fixture.Create<LanguageId>());

        var actual = await _target.ValidateAsync(query);

        Assert.False(actual.IsValid);
    }

    [Fact]
    public async Task Validate_ValidQuery_ShouldBeValid()
    {
        var actual = await _target.ValidateAsync(_validCommand);

        Assert.True(actual.IsValid);
    }
}