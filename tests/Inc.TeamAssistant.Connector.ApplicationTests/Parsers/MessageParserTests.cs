using AutoFixture;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Parsers;
using Inc.TeamAssistant.Primitives;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Connector.ApplicationTests.Parsers;

public sealed class MessageParserTests
{
    private readonly Fixture _fixture = new();
    private readonly IPersonRepository _personRepository;
    private readonly MessageParser _target;

    public MessageParserTests()
    {
        _personRepository = Substitute.For<IPersonRepository>();
        _target = new MessageParser(_personRepository);
    }

    [Fact]
    public void Constructor_PersonRepositoryIsNull_ThrowsException()
    {
        MessageParser Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public async Task Parse_MessageIsNull_ThrowsException()
    {
        Task<(string Text, long? TargetPersonId)> Actual() => _target.Parse(null!, CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentNullException>(Actual);
    }

    [Fact]
    public async Task Parse_Message_ShouldRemovedBotName()
    {
        var botName = _fixture.Create<string>();
        var text = _fixture.Create<string>();
        var message = CreateMessage(botName, text);

        var actual = await _target.Parse(message, CancellationToken.None);
        
        Assert.Equal(text, actual.Text);
        Assert.Null(actual.TargetPersonId);
    }
    
    [Fact]
    public async Task Parse_MessageWithEntities_ShouldDetectTargetPerson()
    {
        var targetPerson = CreatePerson();
        var text = _fixture.Create<string>();
        var otherPersonId = _fixture.Create<long>();
        var message = CreateMessage(
            text: $"{text} {targetPerson.Name} {text}",
            personIds: [otherPersonId, targetPerson.Id]);
        _personRepository.Find(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(targetPerson);

        var actual = await _target.Parse(message, CancellationToken.None);
        
        Assert.Equal($"{text} {text}", actual.Text);
        Assert.Equal(targetPerson.Id, actual.TargetPersonId);
    }
    
    [Fact]
    public async Task Parse_MessageWithEntitiesWithoutTargetPerson_ShouldDetectTargetPerson()
    {
        var targetPerson = CreatePerson();
        var text = _fixture.Create<string>();
        var otherPersonId = _fixture.Create<long>();
        var message = CreateMessage(
            text: text,
            personIds: [otherPersonId, targetPerson.Id]);

        var actual = await _target.Parse(message, CancellationToken.None);
        
        Assert.Equal(text, actual.Text);
        Assert.Null(actual.TargetPersonId);
    }
    
    [Fact]
    public async Task Parse_MessageWithUserName_ShouldDetectTargetPerson()
    {
        var targetPerson = CreatePerson();
        var text = _fixture.Create<string>();
        var otherUserName = CreateUserName();
        var message = CreateMessage(text: $"{text} @{otherUserName} @{targetPerson.Username} {text}");
        _personRepository.Find(targetPerson.Username!, Arg.Any<CancellationToken>()).Returns(targetPerson);

        var actual = await _target.Parse(message, CancellationToken.None);
        
        Assert.Equal($"{text} @{otherUserName} {text}", actual.Text);
        Assert.Equal(targetPerson.Id, actual.TargetPersonId);
    }
    
    [Fact]
    public async Task Parse_MessageWithUserNameWithoutTargetPerson_ShouldNotDetectTargetPerson()
    {
        var targetPerson = CreatePerson();
        var text = _fixture.Create<string>();
        var otherUserName = CreateUserName();
        var message = CreateMessage(text: $"{text} @{otherUserName} @{targetPerson.Username} {text}");

        var actual = await _target.Parse(message, CancellationToken.None);
        
        Assert.Equal($"{text} @{otherUserName} @{targetPerson.Username} {text}", actual.Text);
        Assert.Null(actual.TargetPersonId);
    }
    
    [Fact]
    public async Task Parse_NewLineUserName_ShouldDetectTargetPerson()
    {
        var targetPerson = CreatePerson();
        var text = _fixture.Create<string>();
        var otherUserName = CreateUserName();
        var message = CreateMessage(text: $"{text} @{otherUserName}\n@{targetPerson.Username}");
        _personRepository.Find(targetPerson.Username!, Arg.Any<CancellationToken>()).Returns(targetPerson);

        var actual = await _target.Parse(message, CancellationToken.None);
        
        Assert.Equal($"{text} @{otherUserName}", actual.Text);
        Assert.Equal(targetPerson.Id, actual.TargetPersonId);
    }
    
    [Fact]
    public async Task Parse_UserNameNewLine_ShouldDetectTargetPerson()
    {
        var targetPerson = CreatePerson();
        var text = _fixture.Create<string>();
        var otherUserName = CreateUserName();
        var message = CreateMessage(text: $"{text} @{otherUserName} @{targetPerson.Username}\n");
        _personRepository.Find(targetPerson.Username!, Arg.Any<CancellationToken>()).Returns(targetPerson);

        var actual = await _target.Parse(message, CancellationToken.None);
        
        Assert.Equal($"{text} @{otherUserName} ", actual.Text);
        Assert.Equal(targetPerson.Id, actual.TargetPersonId);
    }
    
    [Fact]
    public async Task Parse_NewLineUserNameNewLine_ShouldDetectTargetPerson()
    {
        var targetPerson = CreatePerson();
        var text = _fixture.Create<string>();
        var otherUserName = CreateUserName();
        var message = CreateMessage(text: $"{text} @{otherUserName}\n@{targetPerson.Username}\n");
        _personRepository.Find(targetPerson.Username!, Arg.Any<CancellationToken>()).Returns(targetPerson);

        var actual = await _target.Parse(message, CancellationToken.None);
        
        Assert.Equal($"{text} @{otherUserName}", actual.Text);
        Assert.Equal(targetPerson.Id, actual.TargetPersonId);
    }

    private IInputMessage CreateMessage(
        string? botName = null,
        string? text = null,
        IReadOnlyCollection<long>? personIds = null)
    {
        var botNameValue = string.IsNullOrWhiteSpace(botName) ? _fixture.Create<string>() : botName;
        var textValue = string.IsNullOrWhiteSpace(text) ? _fixture.Create<string>() : text;
        
        var message = new TelegramMessageAdapter(
            botNameValue,
            $"@{botNameValue} {textValue}",
            personIds ?? _fixture.CreateMany<long>().ToArray());

        return message;
    }

    private Person CreatePerson() => _fixture.Create<Person>() with
    {
        Username = CreateUserName()
    };
    
    private string CreateUserName() => _fixture.Create<string>()[..32].Replace("-", "_");
}