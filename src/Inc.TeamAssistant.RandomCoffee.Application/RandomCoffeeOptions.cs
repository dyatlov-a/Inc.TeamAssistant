namespace Inc.TeamAssistant.RandomCoffee.Application;

public sealed class RandomCoffeeOptions
{
    public TimeSpan NotificationTime { get; set; }
    public TimeSpan ScheduleDelay { get; set; }
    public TimeSpan WaitingInterval { get; set; }
    public TimeSpan RoundInterval { get; set; }
}