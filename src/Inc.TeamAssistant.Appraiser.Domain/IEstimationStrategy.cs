namespace Inc.TeamAssistant.Appraiser.Domain;

public interface IEstimationStrategy
{
    IEnumerable<Estimation> GetValues();

    Estimation GetValue(int value);
    
    int GetWeight(Story story);

    Estimation CalculateMean(Story story);
    
    Estimation CalculateMedian(Story story);
}