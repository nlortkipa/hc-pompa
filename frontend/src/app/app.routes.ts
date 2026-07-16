import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { APP_NAME } from './globals';
import { Verify } from './components/verify/verify';
import { Console } from './components/console/console';
import { New } from './components/console/new/new';
import { History } from './components/console/history/history';
import { Users } from './components/console/users/users';
import { Companies } from './components/console/companies/companies';
import { Plates } from './components/console/plates/plates';
import { Review } from './components/review/review';
import { NotFound } from './components/not-found/not-found';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
    },
    {
        path: 'login',
        component: Login,
        title: `${APP_NAME} - შესვლა`
    },
    {
        path: 'verify',
        component: Verify,
        title: `${APP_NAME} - დადასტურება`
    },
    {
        path: 'review/:token',
        component: Review,
        title: `${APP_NAME} - შემოწმება`
    },
    {
        path: 'console',
        component: Console,
        title: `${APP_NAME} - პანელი`,
        children: [
            {
                path: '',
                redirectTo: 'history',
                pathMatch: 'full'
            },
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
            },
            {
                path: 'plates',
                component: Plates
            }
        ]
    },
    {
        path: '**',
        component: NotFound,
        title: `${APP_NAME} - გვერდი ვერ მოიძებნა`
    }
];
