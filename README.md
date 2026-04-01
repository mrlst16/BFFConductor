# BFFConductor — Backend Usage Guide

BFFConductor is a .NET 8 library for ASP.NET Core BFF (Backend for Frontend) APIs. It standardizes error responses, maps business errors and exceptions to HTTP status codes, and sends display-mode hints to the client via response headers — so your frontend always knows how to present errors to users.

---

## Companion Package

A corresponding Angular library is available on npm: [bffconductor](https://www.npmjs.com/package/bffconductor). It reads the `x-handle-message-as` header and dispatches error display to the appropriate UI handler on the client side.

```bash
npm install bffconductor
```

---

## Installation

```bash
dotnet add package BFFConductor
```

---

## Quick Start

### 1. Create `error-mapping.json`

Place this file in your project root and ensure it is copied to the output directory:

```json
{
  "defaults": {
    "displayMode": "toast"
  },
  "mappings": [
    {
      "errorCode": "VALIDATION_FAILED",
      "httpStatus": 422,
      "displayMode": "inline"
    },
    {
      "errorCode": "NOT_FOUND",
      "httpStatus": 404,
      "displayMode": "toast"
    }
  ],
  "exceptionMappings": [
    {
      "exceptionType": "UnauthorizedAccessException",
      "httpStatus": 401,
      "displayMode": "redirect"
    },
    {
      "exceptionType": "ArgumentNullException",
      "httpStatus": 400,
      "displayMode": "toast",
      "errorCode": "VALIDATION_FAILED"
    }
  ]
}
```

In your `.csproj`:

```xml
<ItemGroup>
  <Content Include="error-mapping.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

### 2. Register in `Program.cs`

```csharp
builder.Services.AddBFFConductor(options =>
{
    options.MappingSpecPath = "error-mapping.json";  // default
    options.FallbackDisplayMode = "toast";           // default
});
```

### 3. Decorate Your Controllers

```csharp
[ApiController]
[UseBffExceptionFilter]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
    {
        var result = _orderService.Create(request);
        return new OperationActionResult<Order>(result);
    }
}
```

---

## Core Concepts

### `OperationResult<T>` — Service Layer Return Type

Use `OperationResult<T>` in your service/application layer to represent success or failure without throwing exceptions.

```csharp
// Success
return OperationResult<Order>.Ok(order);

// Failure with a single error
return OperationResult<Order>.Fail("Order not found", "NOT_FOUND");

// Failure with multiple errors
return OperationResult<Order>.Fail(new[]
{
    new OperationError { Message = "Name is required", Code = "VALIDATION_FAILED" },
    new OperationError { Message = "Email is invalid", Code = "VALIDATION_FAILED" }
});

// Success with a display mode hint
return OperationResult<Order>.Ok(order, displayMode: "toast");
```

`OperationResult<T>` properties:

| Property | Type | Description |
|---|---|---|
| `Success` | `bool` | Whether the operation succeeded |
| `Data` | `T?` | The result payload (null on failure) |
| `Errors` | `List<OperationError>` | Business errors (message + optional code) |
| `DisplayMode` | `string?` | Optional client display hint override |

### `OperationActionResult<T>` — Controller Return Type

`OperationActionResult<T>` converts an `OperationResult<T>` into an HTTP response. It:

- Returns `200 OK` on success with the data payload
- Looks up the error code in the registry to determine HTTP status on failure
- Sets the `x-handle-message-as` response header with the display mode
- Applies `[ErrorDisplay]` attribute overrides (action-level beats controller-level beats registry)

```csharp
[HttpPut("{id}")]
public IActionResult UpdateOrder(int id, [FromBody] UpdateOrderRequest request)
{
    var result = _orderService.Update(id, request);
    return new OperationActionResult<Order>(result);
}
```

### Response Format

All responses use a consistent envelope:

**Success (`200 OK`)**
```json
{
  "success": true,
  "data": { "id": 42, "status": "pending" },
  "errors": []
}
```
```
x-handle-message-as: toast
```

**Failure (`422 Unprocessable Entity`)**
```json
{
  "success": false,
  "data": null,
  "errors": [
    { "message": "Name is required", "code": "VALIDATION_FAILED" }
  ]
}
```
```
x-handle-message-as: inline
```

---

## Display Modes

The `x-handle-message-as` header tells the frontend how to display the error or success message. You define your own display mode strings — common conventions include:

| Mode | Description |
|---|---|
| `silent` | Suppress UI; handle programmatically |
| `toast` | Brief notification (auto-dismissing) |
| `snackbar` | Bottom-of-screen notification bar |
| `inline` | Show validation errors alongside form fields |
| `modal` | Blocking modal dialog |
| `page` | Full-page error display |
| `redirect` | Redirect user (pair with `x-redirect-url` header) |

---

## Attributes

### `[UseBffExceptionFilter]`

Apply to a controller to enable automatic exception handling. Unhandled exceptions are caught, matched against `exceptionMappings` in your JSON config, and returned as structured `ApiResponse` errors.

```csharp
[UseBffExceptionFilter]
public class MyController : ControllerBase { }
```

### `[ErrorDisplay(errorCode, displayMode)]`

Override the display mode for a specific error code at the controller or action level. The most specific match wins: action-level overrides controller-level, which overrides the global registry.

```csharp
// Controller-level: all VALIDATION_FAILED errors use "toast" by default for this controller
[ErrorDisplay("VALIDATION_FAILED", "toast")]
public class MyController : ControllerBase
{
    // Action-level: overrides the controller to use "inline" for this endpoint
    [ErrorDisplay("VALIDATION_FAILED", "inline")]
    [HttpPost]
    public IActionResult Create([FromBody] CreateRequest req) { ... }
}
```

### `[ExceptionDisplay(exceptionType, displayMode)]`

Override the display mode for a specific exception type at the controller or action level.

```csharp
[ExceptionDisplay(typeof(UnauthorizedAccessException), "redirect")]
[HttpDelete("{id}")]
public IActionResult Delete(int id) { ... }
```

---

## `error-mapping.json` Reference

```json
{
  "defaults": {
    "displayMode": "toast"          // Fallback display mode for unmapped errors
  },
  "mappings": [
    {
      "errorCode": "VALIDATION_FAILED",    // Business error code string
      "httpStatus": 422,                   // HTTP status to return
      "displayMode": "inline",             // x-handle-message-as value
      "additionalHeaders": {               // Optional: extra response headers
        "x-my-header": "my-value"
      }
    }
  ],
  "exceptionMappings": [
    {
      "exceptionType": "ArgumentNullException",  // Exception class name
      "httpStatus": 400,
      "displayMode": "toast",
      "errorCode": "VALIDATION_FAILED",          // Optional: included in error response
      "additionalHeaders": {}
    }
  ]
}
```

**Exception type matching** uses inheritance: if you map `Exception`, it will match any unhandled exception not matched by a more specific type.

---

## DI Registration Reference

`AddBFFConductor` registers the following services:

| Service | Lifetime | Description |
|---|---|---|
| `BffResponseOptions` | Singleton | Configuration options |
| `ErrorMappingRegistry` | Singleton | Error code → HTTP status + display mode |
| `ExceptionMappingRegistry` | Singleton | Exception type → HTTP status + display mode |
| `BffExceptionFilter` | Transient | Exception filter (used by `[UseBffExceptionFilter]`) |

`ErrorMappingRegistry` is resolved automatically by `OperationActionResult<T>` — no need to inject it into your controllers.

---

## Full Example

**`error-mapping.json`**
```json
{
  "defaults": { "displayMode": "toast" },
  "mappings": [
    { "errorCode": "VALIDATION_FAILED", "httpStatus": 422, "displayMode": "inline" },
    { "errorCode": "NOT_FOUND",          "httpStatus": 404, "displayMode": "toast"   },
    { "errorCode": "CONFLICT",           "httpStatus": 409, "displayMode": "modal"   }
  ],
  "exceptionMappings": [
    { "exceptionType": "UnauthorizedAccessException", "httpStatus": 401, "displayMode": "redirect" }
  ]
}
```

**`Program.cs`**
```csharp
builder.Services.AddBFFConductor(options =>
{
    options.MappingSpecPath = "error-mapping.json";
    options.FallbackDisplayMode = "toast";
});

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
```

**`ProductsController.cs`**
```csharp
[ApiController]
[UseBffExceptionFilter]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var result = _service.GetById(id);
        return new OperationActionResult<Product>(result);
    }

    [HttpPost]
    [ErrorDisplay("VALIDATION_FAILED", "inline")]   // override global mapping for this action
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        var result = _service.Create(request);
        return new OperationActionResult<Product>(result);
    }
}
```

**`ProductService.cs`**
```csharp
public class ProductService : IProductService
{
    public OperationResult<Product> GetById(int id)
    {
        var product = _repo.Find(id);
        if (product is null)
            return OperationResult<Product>.Fail("Product not found", "NOT_FOUND");

        return OperationResult<Product>.Ok(product);
    }

    public OperationResult<Product> Create(CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return OperationResult<Product>.Fail("Name is required", "VALIDATION_FAILED");

        var product = _repo.Save(new Product(request.Name));
        return OperationResult<Product>.Ok(product);
    }
}
```

---

## Interfaces

If you need to abstract over response or result types, BFFConductor exposes:

```csharp
// For API responses (returned to clients)
public interface IApiResponse<out T>
{
    bool Success { get; }
    T? Data { get; }
    IReadOnlyList<ApiError> Errors { get; }
}

// For service/application layer results
public interface IOperationResult
{
    bool Success { get; }
    IReadOnlyList<OperationError> Errors { get; }
    string? DisplayMode { get; }
    object? GetData();
}

public interface IOperationResult<out T> : IOperationResult
{
    T? Data { get; }
}
```
