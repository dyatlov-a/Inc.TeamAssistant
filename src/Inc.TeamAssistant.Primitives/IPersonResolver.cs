namespace Inc.TeamAssistant.Primitives;

public interface IPersonResolver
{
    void TrySet(Person person);

    void Reset();
    
    Person GetCurrentPerson();
}