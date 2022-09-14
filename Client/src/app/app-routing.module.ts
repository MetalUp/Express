import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {
    ApplicationPropertiesComponent,
    AttachmentComponent,
    CallbackComponent,
    DynamicErrorComponent,
    DynamicListComponent,
    DynamicObjectComponent,
    HomeComponent,
    LogoffComponent,
    MultiLineDialogComponent,
    RecentComponent
} from '@nakedobjects/gemini';
import { AuthService, ViewType } from '@nakedobjects/services';
import { TaskViewComponent } from './task-view/task-view.component';

const routes: Routes = [
    { path: '',  redirectTo: 'task',  pathMatch: 'full' },
    {
        path: 'dashboard/home',
        component: HomeComponent,
        canActivate: [AuthService],
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
        canActivate: [AuthService],
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
        canActivate: [AuthService],
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
        canActivate: [AuthService],
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
        canActivate: [AuthService],
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
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'dashboard/applicationProperties',
        component: ApplicationPropertiesComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'dashboard/multiLineDialog',
        component: MultiLineDialogComponent,
        canActivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'dashboard/logoff',
        component: LogoffComponent,
        canActivate: [AuthService],
        canDeactivate: [AuthService],
        data: { pane: 1, paneType: 'single' }
    },
    {
        path: 'dashboard/callback',
        component: CallbackComponent
    },
    { path: 'task/:id', component: TaskViewComponent, canActivate: [AuthService] },
    { path: 'task',  redirectTo: 'task/9', pathMatch: 'full'},
    { path: 'dashboard',  redirectTo: 'dashboard/home', pathMatch: 'full'},
    { path: '**', redirectTo: 'task/9', pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
    exports: [RouterModule],
    providers: []
})
export class RoutingModule { }
