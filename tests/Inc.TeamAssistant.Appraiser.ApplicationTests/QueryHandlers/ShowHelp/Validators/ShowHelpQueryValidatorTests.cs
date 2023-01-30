using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Application.QueryHandlers.ShowHelp.Validators;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;
using Inc.TeamAssistant.Common.Messages;
using Inc.TeamAssistant.Primitives;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.QueryHandlers.ShowHelp.Validators;

public sealed class ShowHelpQueryValidatorTests
{
    private readonly Dictionary<string, Dictionary<string, string>> _languages = new()
    {
        ["ru"] = new(),
        ["en"] = new()
    };

    private readonly Fixture _fixture = new();
    private readonly ShowHelpQuery _validQuery;
    private readonly ShowHelpQueryValidator _target;

    public ShowHelpQueryValidatorTests()
    {
        var messageService = Substitute.For<IMessageService>();
        messageService.GetAll(Arg.Any<CancellationToken>()).Returns(ServiceResult.Success(_languages));

        var messageBuilder = Substitute.For<IMessageBuilder>();
        messageBuilder
            .Build(Arg.Any<MessageId>(), Arg.Any<LanguageId>(), Arg.Any<object[]>())
            .Returns(_fixture.Create<string>());

        _validQuery = new(new("en"));
        _target = new ShowHelpQueryValidator(new LanguageValidator(messageService));
    }

    [Fact]
    public void Constructor_OptionsIsNull_ThrowsException()
    {
        ShowHelpQueryValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public async Task Validate_SetLanguageValidator_ShouldBeCalled()
    {
        var query = new ShowHelpQuery(_fixture.Create<LanguageId>());

        var actual = await _target.ValidateAsync(query);

        Assert.False(actual.IsValid);
    }

    [Fact]
    public async Task Validate_ValidQuery_ShouldBeValid()
    {
        var actual = await _target.ValidateAsync(_validQuery);

        Assert.True(actual.IsValid);
    }
}