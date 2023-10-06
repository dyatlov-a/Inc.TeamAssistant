namespace Inc.TeamAssistant.Reviewer.Domain;

public interface INextReviewerStrategy
{
    Person Next(Person owner, Person? lastReviewer);
}