namespace Inc.TeamAssistant.Reviewer.All.Model;

public interface INextReviewerStrategy
{
    Player Next(Player owner);
}