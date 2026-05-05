namespace TaskPlanner.Api.Contracts;

public sealed record ApiResponse<T>(bool Success, T? Data, IReadOnlyList<ApiError> Errors)
{
    public static ApiResponse<T> Ok(T data)
    {
        return new ApiResponse<T>(true, data, []);
    }

    public static ApiResponse<T> Fail(IReadOnlyList<ApiError> errors)
    {
        return new ApiResponse<T>(false, default, errors);
    }
}

