namespace Inc.TeamAssistant.Appraiser.Domain;

public static class AssessmentValue
{
	public enum Value
	{
		None = -2,
		More = -1,
		NoIdea = 0,
		
		Sp1 = 1,
		Sp2 = 2,
		Sp3 = 3,
		Sp5 = 5,
		Sp8 = 8,
		Sp13 = 13,
		Sp21 = 21,
		
		Xs = 100,
		S = 200,
		M = 300,
		L = 400,
		Xl = 500,
		Xxl = 600
	}

	internal static readonly IReadOnlyCollection<Value> ScrumAssessments = new[]
	{
		Value.Sp1,
		Value.Sp2,
		Value.Sp3,
		Value.Sp5,
		Value.Sp8,
		Value.Sp13,
		Value.Sp21,
		Value.More,
		Value.NoIdea
	};
	
	internal static readonly IReadOnlyCollection<Value> KanbanAssessments = new[]
	{
		Value.Xs,
		Value.S,
		Value.M,
		Value.L,
		Value.Xl,
		Value.Xxl,
		Value.More,
		Value.NoIdea
	};

	public static IReadOnlyCollection<Value> GetAllAssessments()
	{
		return ScrumAssessments.Union(KanbanAssessments).Distinct().ToArray();
	}

	public static Value ToAssessmentValue(this string assessment)
	{
		if (Enum.TryParse<Value>(assessment, out var value))
			return value;

		return Value.None;
	}
}