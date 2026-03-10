using BFFConductor.Attributes;
using BFFConductor.Configuration;
using BFFConductor.Models;
using BFFConductor.Results;
using Microsoft.AspNetCore.Mvc;

namespace BFFConductor.Api.Controllers;

[ApiController]
[UseBffExceptionFilter]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ErrorMappingRegistry _registry;

    public TestController(ErrorMappingRegistry registry)
    {
        _registry = registry;
    }

    // POST /test/error
    // Body: { "errorCode": "1001", "errorMessage": "Something went wrong" }
    // If both are null → success (true)
    // Otherwise       → failure with the provided code and message
    [HttpPost("error")]
    public IActionResult PostError([FromBody] ErrorRequest request)
    {
        if (request.ErrorCode is null && request.ErrorMessage is null)
            return new OperationActionResult<bool>(OperationResult<bool>.Ok(true), _registry);

        return new OperationActionResult<bool>(
            OperationResult<bool>.Fail(
                request.ErrorMessage ?? "An error occurred.",
                request.ErrorCode),
            _registry);
    }
}

public record ErrorRequest(string? ErrorCode, string? ErrorMessage);
