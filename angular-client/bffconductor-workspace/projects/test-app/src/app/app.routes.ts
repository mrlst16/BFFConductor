import { Routes } from '@angular/router';
import { Home } from './home/home';
import { ErrorPage } from './error-page/error-page';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'error', component: ErrorPage }
];
