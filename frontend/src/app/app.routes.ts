import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { APP_NAME } from './globals';
import { Verify } from './components/verify/verify';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
    },
    {
        path: 'login',
        component: Login,
        title: `${APP_NAME} - Login`
    },
    {
        path: 'verify',
        component: Verify,
        title: `${APP_NAME} - Verify`
    },
];
