import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-error-page',
  imports: [RouterLink],
  template: `
    <div class="error-page">
      <h1>Access Denied</h1>
      <p>{{ message }}</p>
      <a routerLink="/">Go back</a>
    </div>
  `,
  styles: [`
    .error-page {
      max-width: 640px;
      margin: 60px auto;
      padding: 0 24px;
      font-family: sans-serif;

      h1 { color: #991b1b; }
      p  { color: #555; margin: 16px 0 24px; }
      a  { color: #2563eb; }
    }
  `]
})
export class ErrorPage {
  protected readonly message: string;

  constructor() {
    const params = inject(ActivatedRoute).snapshot.queryParamMap;
    this.message = params.get('message') ?? 'An unexpected error occurred.';
  }
}
