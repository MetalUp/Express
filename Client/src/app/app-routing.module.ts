import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {
    ApplicationPropertiesComponent,
    AttachmentComponent,
    DynamicErrorComponent,
    DynamicListComponent,
    DynamicObjectComponent,
    HomeComponent,
    LogoffComponent,
    MultiLineDialogComponent,
    RecentComponent
} from '@nakedobjects/gemini';
import { ViewType } from '@nakedobjects/services';
import { LandingComponent } from './landing/landing.component';
import { RegisteredService } from './services/registered.service';
import { TaskViewComponent } from './task-view/task-view.component';

const routes: Routes = [
    { path: '',  redirectTo: 'landing',  pathMatch: 'full' },
    {
        path: 'dashboard/home',
        component: HomeComponent,
        canActivate: [RegisteredService],
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
        canActivate: [RegisteredService],
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
        canActivate: [RegisteredService],
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
        canActivate: [RegisteredService],
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
        canActivate: [RegisteredService],
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
        canActivate: [RegisteredService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'dashboard/applicationProperties',
        component: ApplicationPropertiesComponent,
        canActivate: [RegisteredService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'dashboard/multiLineDialog',
        component: MultiLineDialogComponent,
        canActivate: [RegisteredService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'dashboard/logoff',
        component: LogoffComponent,
        canActivate: [RegisteredService],
        canDeactivate: [RegisteredService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'gemini/callback',
        component: LandingComponent
    },
    {
        path: 'landing',
        component: LandingComponent
    },
    { path: 'task/logoff',  redirectTo: 'dashboard/logoff', pathMatch: 'full'},
    { path: 'task/home',  redirectTo: 'dashboard/home', pathMatch: 'full'},
    { path: 'task/:id', component: TaskViewComponent, canActivate: [RegisteredService] },
    { path: 'task',  redirectTo: 'dashboard/home', pathMatch: 'full'},
    { path: 'dashboard',  redirectTo: 'dashboard/home', pathMatch: 'full'},
    { path: '**', redirectTo: 'landing', pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
    exports: [RouterModule],
    providers: []
})
export class RoutingModule { }
