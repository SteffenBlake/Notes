﻿using Microsoft.AspNetCore.Mvc;

namespace Notes.Website.Controllers;

/// <summary>
/// Standard base controller for all API controllers to inherit from
/// </summary>
public abstract class NotesControllerBase : ControllerBase
{
    /// <summary>
    /// Parses ModelState and returns whether it has errors or not, 
    /// and produces an Error json result if it does
    /// </summary>
    [NonAction]
    protected bool TryCheckModelState(out JsonResult? errorResult)
    {
        if (ModelState.IsValid)
        {
            errorResult = null;
            return true;
        }

        var modelErrors = ModelState.ToDictionary(
            m => m.Key,
            m => m.Value?.Errors.Select(e => e.ErrorMessage).ToHashSet() ?? new()
        );

        var result = new TryResult<object>(new ResultErrors(modelErrors));
        errorResult = TryResultResponse(result);
        return false;
    }

    /// <summary>
    /// Handles setting of the status code of the response
    /// as well as json serialization
    /// </summary>
    [NonAction]
    protected JsonResult TryResultResponse<T>(TryResult<T> tryResult)
        where T: class
    {
        HttpContext.Response.StatusCode = tryResult.StatusCode;
        return new JsonResult(tryResult);
    }
}
