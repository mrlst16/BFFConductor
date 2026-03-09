import { Injectable, signal } from '@angular/core';
import { ApiError } from 'bffconductor';

@Injectable({ providedIn: 'root' })
export class InlineErrorService {
  readonly errors = signal<ApiError[]>([]);

  set(errors: ApiError[]): void { this.errors.set(errors); }
  clear(): void { this.errors.set([]); }
}
