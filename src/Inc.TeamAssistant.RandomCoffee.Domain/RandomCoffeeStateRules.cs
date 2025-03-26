namespace Inc.TeamAssistant.RandomCoffee.Domain;

public static class RandomCoffeeStateRules
{
    public static readonly IReadOnlyCollection<RandomCoffeeState> ActiveStates =
    [
        RandomCoffeeState.Waiting,
        RandomCoffeeState.Idle
    ];
}