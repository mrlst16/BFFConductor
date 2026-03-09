import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-nav',
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav>
      <a routerLink="/"           routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Basic</a>
      <a routerLink="/controller" routerLinkActive="active">Controller Attrs</a>
      <a routerLink="/action"     routerLinkActive="active">Action Attrs</a>
    </nav>
  `
})
export class Nav {}
