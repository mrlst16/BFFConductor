# bffconductor

Angular client library for the [BFFConductor](https://www.nuget.org/packages/BFFConductor) .NET pattern. Reads the `x-handle-message-as` response header and routes errors to the correct UI handler — toast, inline validation, modal, redirect, etc. — based on what the server specifies.

---

## Companion Package

The server-side NuGet package is available at [nuget.org/packages/BFFConductor](https://www.nuget.org/packages/BFFConductor). It sets the `x-handle-message-as` header and standardizes the `ApiResponse` envelope that this library consumes.

```bash
dotnet add package BFFConductor
```

---

## Installation

```bash
npm install bffconductor
```

---

## Quick Start

### 1. Register the interceptor in `app.config.ts`

```typescript
import { bffResponseInterceptor, BFF_ERROR_ROUTER } from 'bffconductor';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([bffResponseInterceptor])),
    { provide: BFF_ERROR_ROUTER, useClass: AppErrorRouter }
  ]
};
```

### 2. Implement `BffErrorRouter`

```typescript
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BffErrorRouter, ApiError } from 'bffconductor';

@Injectable({ providedIn: 'root' })
export class AppErrorRouter implements BffErrorRouter {
  constructor(private router: Router) {}

  route(displayMode: string, errors: ApiError[], headers: Record<string, string>): void {
    const message = errors[0]?.message ?? 'An error occurred.';

    switch (displayMode) {
      case 'toast':
        // e.g. this.toastService.show(message);
        break;
      case 'inline':
        // e.g. this.formErrorService.set(errors);
        break;
      case 'modal':
        // e.g. this.dialogService.open(message);
        break;
      case 'redirect':
        this.router.navigateByUrl(headers['x-redirect-url'] ?? '/error');
        break;
      case 'silent':
        break;
    }
  }
}
```

---

## How It Works

The `bffResponseInterceptor` hooks into Angular's `HttpClient` pipeline and does two things:

**On success** — unwraps the `ApiResponse<T>` envelope so your services receive `data` directly, not the wrapper object.

**On error** — reads the `x-handle-message-as` response header, then calls `BffErrorRouter.route()` with the display mode, errors array, and all response headers. Your router implementation decides how to present it.

```
HTTP error response
  └─ x-handle-message-as: inline
  └─ body: { success: false, errors: [{ message: "Name is required", code: "VALIDATION_FAILED" }] }
        ↓
  bffResponseInterceptor
        ↓
  BffErrorRouter.route("inline", errors, headers)
        ↓
  Your UI logic (show inline validation, toast, redirect, etc.)
```

If no `BFF_ERROR_ROUTER` is provided, the interceptor still processes responses but skips routing — errors are re-thrown for your own `catchError` handling.

---

## API Reference

### `bffResponseInterceptor`

`HttpInterceptorFn` — register via `withInterceptors([bffResponseInterceptor])`.

### `BFF_ERROR_ROUTER`

`InjectionToken<BffErrorRouter>` — provide your implementation to handle routed errors.

### `BffErrorRouter`

```typescript
interface BffErrorRouter {
  route(displayMode: string, errors: ApiError[], headers: Record<string, string>): void;
}
```

### `ApiResponse<T>`

```typescript
interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  errors: ApiError[];
}
```

### `ApiError`

```typescript
interface ApiError {
  message: string;
  code?: string;
}
```

### `BFF_DISPLAY_MODE_HEADER`

String constant: `'x-handle-message-as'`

---

## Display Modes

Display modes are open strings — you define what they mean in your `BffErrorRouter`. Common conventions from the server-side library:

| Mode | Typical UI behaviour |
|---|---|
| `silent` | Suppress UI; handle programmatically |
| `toast` | Brief auto-dismissing notification |
| `snackbar` | Bottom-of-screen notification bar |
| `inline` | Validation errors next to form fields |
| `modal` | Blocking dialog |
| `page` | Full-page error |
| `redirect` | Navigate away (check `x-redirect-url` header) |
