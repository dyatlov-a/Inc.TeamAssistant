namespace Inc.TeamAssistant.Appraiser.Domain;

public interface IEstimationStrategy
{
    IEnumerable<Estimation> GetValues();

    Estimation GetValue(int value);

    Estimation CalculateMean(Story story);
    
    Estimation CalculateMedian(Story story);
}