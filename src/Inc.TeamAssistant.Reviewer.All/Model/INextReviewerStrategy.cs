namespace Inc.TeamAssistant.Reviewer.All.Model;

public interface INextReviewerStrategy
{
    Player Next(Person owner, Person? lastReviewer);
}