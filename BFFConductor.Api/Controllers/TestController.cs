using BFFConductor.Attributes;
using BFFConductor.Models;
using Microsoft.AspNetCore.Mvc;

namespace BFFConductor.Api.Controllers;

[ApiController]
[UseBFFResponseFilter]
[Route("[controller]")]
public class TestController : ControllerBase
{
    // POST /test/error
    // Body: { "errorCode": "1001", "errorMessage": "Something went wrong" }
    // If both are null → success (true)
    // Otherwise       → failure with the provided code and message
    [HttpPost("error")]
    public IActionResult PostError([FromBody] ErrorRequest request)
    {
        if (request.ErrorCode is null && request.ErrorMessage is null)
            return Ok(OperationResult<bool>.Ok(true));

        return Ok(OperationResult<bool>.Fail(
            request.ErrorMessage ?? "An error occurred.",
            request.ErrorCode));
    }
}

public record ErrorRequest(string? ErrorCode, string? ErrorMessage);
