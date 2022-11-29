import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {
    ApplicationPropertiesComponent,
    AttachmentComponent,
    DynamicErrorComponent,
    DynamicListComponent,
    DynamicObjectComponent,
    HomeComponent,
    MultiLineDialogComponent,
    RecentComponent
} from '@nakedobjects/gemini';
import { ViewType } from '@nakedobjects/services';
import { ArmliteComponent } from './armlite/armlite.component';
import { LandingComponent } from './landing/landing.component';
import { LogoffComponent } from './logoff/logoff.component';
import { RegistrationService } from './services/registration.service';
import { TaskViewComponent } from './task-view/task-view.component';

const routes: Routes = [
    { path: '',  redirectTo: 'landing',  pathMatch: 'full' },
    {
        path: 'dashboard/home',
        component: HomeComponent,
        canActivate: [RegistrationService],
        data: { pane: 1, paneType: 'single' },
        children: [
            { path: 'home', component: HomeComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'object', component: DynamicObjectComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'list', component: DynamicListComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'attachment', component: AttachmentComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'recent', component: RecentComponent, data: { pane: 2, paneType: 'split' } }
        ]
    },
    {
        path: 'dashboard/object',
        component: DynamicObjectComponent,
        canActivate: [RegistrationService],
        data: { pane: 1, paneType: 'single', dynamicType: ViewType.Object },
        children: [
            { path: 'home', component: HomeComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'object', component: DynamicObjectComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'list', component: DynamicListComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'attachment', component: AttachmentComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'recent', component: RecentComponent, data: { pane: 2, paneType: 'split' } }
        ]
    },
    {
        path: 'dashboard/list',
        component: DynamicListComponent,
        canActivate: [RegistrationService],
        data: { pane: 1, paneType: 'single' },
        children: [
            { path: 'home', component: HomeComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'object', component: DynamicObjectComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'list', component: DynamicListComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'attachment', component: AttachmentComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'recent', component: RecentComponent, data: { pane: 2, paneType: 'split' } }
        ]
    },
    {
        path: 'dashboard/attachment',
        component: AttachmentComponent,
        canActivate: [RegistrationService],
        data: { pane: 1, paneType: 'single' },
        children: [
            { path: 'home', component: HomeComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'object', component: DynamicObjectComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'list', component: DynamicListComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'attachment', component: AttachmentComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'recent', component: RecentComponent, data: { pane: 2, paneType: 'split' } }
        ]
    },
    {
        path: 'dashboard/recent',
        component: RecentComponent,
        canActivate: [RegistrationService],
        data: { pane: 1, paneType: 'single' },
        children: [
            { path: 'home', component: HomeComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'object', component: DynamicObjectComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'list', component: DynamicListComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'attachment', component: AttachmentComponent, data: { pane: 2, paneType: 'split' } },
            { path: 'recent', component: RecentComponent, data: { pane: 2, paneType: 'split' } }
        ]
    },
    {
        path: 'dashboard/error',
        component: DynamicErrorComponent,
        canActivate: [RegistrationService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'dashboard/applicationProperties',
        component: ApplicationPropertiesComponent,
        canActivate: [RegistrationService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'dashboard/multiLineDialog',
        component: MultiLineDialogComponent,
        canActivate: [RegistrationService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'dashboard/logoff',
        component: LogoffComponent,
        canActivate: [RegistrationService],
        canDeactivate: [RegistrationService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'landing',
        component: LandingComponent
    },
    { path: 'task/logoff',  redirectTo: 'dashboard/logoff', pathMatch: 'full'},
    { path: 'task/home',  redirectTo: 'dashboard/home', pathMatch: 'full'},
    { path: 'task/:id', component: TaskViewComponent, canActivate: [RegistrationService] },
    { path: 'task',  redirectTo: 'dashboard/home', pathMatch: 'full'},
    { path: 'dashboard',  redirectTo: 'dashboard/home', pathMatch: 'full'},
    { path: 'armlite',  component: ArmliteComponent, canActivate: [RegistrationService]},
    { path: '**', redirectTo: 'landing', pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
    exports: [RouterModule],
    providers: []
})
export class RoutingModule { }
