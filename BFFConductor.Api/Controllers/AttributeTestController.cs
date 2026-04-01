using BFFConductor.Attributes;
using BFFConductor.Models;
using BFFConductor.Results;
using Microsoft.AspNetCore.Mvc;

namespace BFFConductor.Api.Controllers;

// Spec defaults:  1001 → inline,   1002 → toast
// Controller overrides: 1001 → toast, 1002 → silent
[ApiController]
[UseBffExceptionFilter]
[Route("[controller]")]
[ErrorDisplay(ErrorCodes.ValidationFailed, DisplayMethod.Toast)]
[ErrorDisplay(ErrorCodes.NotFound, DisplayMethod.Silent)]
public class AttributeTestController : ControllerBase
{
    // POST /attributetest/controller
    // Effective: 1001 → toast (controller), 1002 → silent (controller)
    [HttpPost("controller")]
    public IActionResult ControllerLevel([FromBody] ErrorRequest request)
    {
        if (request.ErrorCode is null && request.ErrorMessage is null)
            return new OperationActionResult<bool>(OperationResult<bool>.Ok(true));

        return new OperationActionResult<bool>(
            OperationResult<bool>.Fail(
                request.ErrorMessage ?? "An error occurred.",
                request.ErrorCode));
    }

    // POST /attributetest/action
    // Effective: 1001 → snackbar (action overrides controller's toast), 1002 → silent (inherited from controller)
    [HttpPost("action")]
    [ErrorDisplay(ErrorCodes.ValidationFailed, DisplayMethod.Snackbar)]
    public IActionResult ActionLevel([FromBody] ErrorRequest request)
    {
        if (request.ErrorCode is null && request.ErrorMessage is null)
            return new OperationActionResult<bool>(OperationResult<bool>.Ok(true));

        return new OperationActionResult<bool>(
            OperationResult<bool>.Fail(
                request.ErrorMessage ?? "An error occurred.",
                request.ErrorCode));
    }
}
