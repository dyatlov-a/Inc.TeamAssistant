namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record ServiceResult<T>(ServiceResultState State, T Result, string ErrorMessage)
{
	public static readonly ServiceResult<T> Empty = new(ServiceResultState.IsLoading, default!, string.Empty);
}

public static class ServiceResult
{
    public static ServiceResult<T> Success<T>(T result) => new(ServiceResultState.Success, result, string.Empty);

	public static ServiceResult<T> Failed<T>(string errorMessage) => new(ServiceResultState.Failed, default!, errorMessage);

	public static ServiceResult<T> NotFound<T>() => new(ServiceResultState.NotFound, default!, string.Empty);
}