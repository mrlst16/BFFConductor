import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgxSonnerToaster } from 'ngx-sonner';
import { Nav } from './nav/nav';

@Component({
  selector: 'app-root',
  imports: [NgxSonnerToaster, RouterOutlet, Nav],
  template: `
    <ngx-sonner-toaster richColors />
    <app-nav />
    <router-outlet />
  `
})
export class App {}
