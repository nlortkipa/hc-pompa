import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { APP_NAME } from './globals';
import { Verify } from './components/verify/verify';
import { Console } from './components/console/console';
import { New } from './components/console/new/new';
import { History } from './components/console/history/history';
import { Users } from './components/console/users/users';
import { Companies } from './components/console/companies/companies';

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
    {
        path: 'console',
        component: Console,
        title: `${APP_NAME} - Console`,
        children: [
            {
                path: 'new',
                component: New
            },
            {
                path: 'history',
                component: History
            },
            {
                path: 'users',
                component: Users
            },
            {
                path: 'companies',
                component: Companies
            }
        ]
    }
];
