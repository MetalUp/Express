﻿import { NgModule } from '@angular/core';
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
import { CustomEditorComponent } from './custom-editor/custom-editor.component';
import { InvitationComponent } from './invitation/invitation.component';
import { LandingComponent } from './landing/landing.component';
import { LogoffComponent } from './logoff/logoff.component';
import { RestViewerComponent } from './rest-viewer/rest-viewer.component';
import { RegistrationService } from './services/registration.service';
import { TaskViewComponent } from './task-view/task-view.component';

const routes: Routes = [
    { path: '',  redirectTo: 'landing',  pathMatch: 'full' },
    { path: 'invitation/:id', component: InvitationComponent },
    { path: 'invitation', component: InvitationComponent },
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
    { path: 'task/recent',  redirectTo: 'dashboard/recent', pathMatch: 'full'},
    { path: 'task/applicationProperties',  redirectTo: 'dashboard/applicationProperties', pathMatch: 'full'},
    { path: 'roapiviewer/logoff',  redirectTo: 'dashboard/logoff', pathMatch: 'full'},
    { path: 'roapiviewer/home',  redirectTo: 'dashboard/home', pathMatch: 'full'},
    { path: 'roapiviewer/recent',  redirectTo: 'dashboard/recent', pathMatch: 'full'},
    { path: 'roapiviewer/applicationProperties',  redirectTo: 'dashboard/applicationProperties', pathMatch: 'full'},
    { path: 'task/:id', component: TaskViewComponent, canActivate: [RegistrationService] },
    { path: 'task',  redirectTo: 'dashboard/home', pathMatch: 'full'},
    { path: 'dashboard/editor/:id',  component: CustomEditorComponent, canActivate: [RegistrationService] },
    { path: 'roapiviewer',  component: RestViewerComponent, canActivate: [RegistrationService] },
    { path: 'dashboard',  redirectTo: 'dashboard/home', pathMatch: 'full'},
    { path: '**', redirectTo: 'landing', pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
    providers: []
})
export class RoutingModule { }
