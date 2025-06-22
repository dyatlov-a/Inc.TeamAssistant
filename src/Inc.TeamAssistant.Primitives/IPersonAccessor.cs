namespace Inc.TeamAssistant.Primitives;

public interface IPersonAccessor
{
    Task<Person> Ensure(Person person, CancellationToken token);
}