using Microsoft.AspNetCore.Mvc;
using TaskPlanner.Api.Contracts;
using TaskPlanner.Application.Common;

namespace TaskPlanner.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(result.Value!));
        }

        return ErrorResult<T>(result);
    }

    protected IActionResult CreatedFromResult<T>(string actionName, object routeValues, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(result.Value!));
        }

        return ErrorResult<T>(result);
    }

    protected IActionResult FromDeleteResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return StatusCode(ToStatusCode(result.Status), ApiResponse<object>.Fail(ToApiErrors(result.Errors)));
    }

    private IActionResult ErrorResult<T>(Result<T> result)
    {
        return StatusCode(ToStatusCode(result.Status), ApiResponse<T>.Fail(ToApiErrors(result.Errors)));
    }

    private static int ToStatusCode(ResultStatus status)
    {
        return status switch
        {
            ResultStatus.ValidationError => StatusCodes.Status400BadRequest,
            ResultStatus.NotFound => StatusCodes.Status404NotFound,
            ResultStatus.Conflict => StatusCodes.Status409Conflict,
            ResultStatus.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static IReadOnlyList<ApiError> ToApiErrors(IReadOnlyList<ResultError> errors)
    {
        return errors.Select(error => new ApiError(error.Code, error.Message)).ToList();
    }
}

