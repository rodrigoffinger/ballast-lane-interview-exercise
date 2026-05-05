namespace TaskPlanner.Application.Common;

public class Result
{
    protected Result(ResultStatus status, IReadOnlyList<ResultError> errors)
    {
        Status = status;
        Errors = errors;
    }

    public ResultStatus Status { get; }

    public IReadOnlyList<ResultError> Errors { get; }

    public bool IsSuccess => Status == ResultStatus.Success;

    public static Result Success()
    {
        return new Result(ResultStatus.Success, []);
    }

    public static Result Failure(ResultStatus status, IReadOnlyList<ResultError> errors)
    {
        return new Result(status, errors);
    }
}

public sealed class Result<T> : Result
{
    private Result(ResultStatus status, T? value, IReadOnlyList<ResultError> errors)
        : base(status, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(ResultStatus.Success, value, []);
    }

    public static new Result<T> Failure(ResultStatus status, IReadOnlyList<ResultError> errors)
    {
        return new Result<T>(status, default, errors);
    }
}
