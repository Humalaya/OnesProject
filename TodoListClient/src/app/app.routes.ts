import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/todo-list/todo-list.component').then((m) => m.TodoListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register.component').then((m) => m.RegisterComponent)
  },
  {
    path: 'profile',
    loadComponent: () => import('./pages/profile/profile.component').then((m) => m.ProfileComponent),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '' }
];
