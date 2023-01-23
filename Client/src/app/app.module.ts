import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { LibModule as GeminiModule } from '@nakedobjects/gemini';
import { LibModule as ServicesModule } from '@nakedobjects/services';
import { LibModule as ViewModelModule } from '@nakedobjects/view-models';
import { RoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ExpressionEvaluationComponent } from './expression-evaluation/expression-evaluation.component';
import { CodeDefinitionComponent } from './code-definition/code-definition.component';
import { HintComponent } from './hint/hint.component';
import { TaskViewComponent } from './task-view/task-view.component';
import { TaskDescriptionComponent } from './task-description/task-description.component';
import { TestingComponent } from './testing/testing.component';
import { UserComponent } from './user/user.component';
import { LandingComponent } from './landing/landing.component';
import { AuthHttpInterceptor, AuthModule } from '@auth0/auth0-angular';
import { LogoffComponent } from './logoff/logoff.component';
import { ResultComponent } from './result/result.component';
import { InvitationComponent } from './invitation/invitation.component';
import { CustomEditorComponent } from './custom-editor/custom-editor.component';

@NgModule({
    declarations: [
        AppComponent,
        ExpressionEvaluationComponent,
        CodeDefinitionComponent,
        UserComponent,
        TaskDescriptionComponent,
        TestingComponent,
        HintComponent,
        TaskViewComponent,
        LandingComponent,
        LogoffComponent,
        ResultComponent,
        InvitationComponent,
        CustomEditorComponent,
    ],
    imports: [
        BrowserModule,
        FormsModule,
        RoutingModule,
        ReactiveFormsModule,
        HttpClientModule,
        ServicesModule.forRoot(),
        ViewModelModule.forRoot(),
        GeminiModule.forRoot(),
        AuthModule.forRoot({
            domain: 'nakedobjects.eu.auth0.com',
            clientId: 'UASxK8nzWzY2qiZzZg4RIDB4N6dRzXc1',
            audience: 'https://metalupadminserver.azurewebsites.net',
            httpInterceptor: {
                allowedList: [
                  'http://localhost:5000',
                  'http://localhost:5000/*',
                  'https://metalupadminserver.azurewebsites.net',
                  'https://metalupadminserver.azurewebsites.net/*',
                  '/*'
                ]
            }
          }),
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: AuthHttpInterceptor, multi: true },
      ],
    bootstrap: [AppComponent]
})
export class AppModule { }
