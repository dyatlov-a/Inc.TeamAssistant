namespace Inc.TeamAssistant.Reviewer.All.Model;

public interface INextReviewerStrategy
{
    Person Next(Person owner, Person? lastReviewer);
}