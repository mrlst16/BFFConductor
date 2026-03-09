import { Routes } from '@angular/router';
import { Home } from './home/home';
import { ControllerTest } from './controller-test/controller-test';
import { ActionTest } from './action-test/action-test';
import { ErrorPage } from './error-page/error-page';

export const routes: Routes = [
  { path: '',           component: Home },
  { path: 'controller', component: ControllerTest },
  { path: 'action',     component: ActionTest },
  { path: 'error',      component: ErrorPage }
];
