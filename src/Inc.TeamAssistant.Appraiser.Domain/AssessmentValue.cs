namespace Inc.TeamAssistant.Appraiser.Domain;

public static class AssessmentValue
{
	public enum Value
	{
		Unknown = -1,
		None = 0,
		Sp1 = 1,
		Sp2 = 2,
		Sp3 = 3,
		Sp5 = 5,
		Sp8 = 8,
		Sp13 = 13,
		Sp21 = 21
	}

	public static readonly IReadOnlyCollection<Value> GetAssessments = Enum.GetValues<Value>().Where(i => i > 0).ToArray();

	public static Value ToAssessmentValue(this int? assessment)
	{
		if (assessment.HasValue && Enum.IsDefined(typeof(Value), assessment.Value))
			return (Value) assessment.Value;

		return Value.Unknown;
	}
}