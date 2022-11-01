﻿import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { LibModule as GeminiModule } from '@nakedobjects/gemini';
import { LibModule as ServicesModule } from '@nakedobjects/services';
import { LibModule as ViewModelModule } from '@nakedobjects/view-models';
import { RoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ExpressionEvaluationComponent } from './expression-evaluation/expression-evaluation.component';
import { FunctionDefinitionComponent } from './function-definition/function-definition.component';
import { HintComponent } from './hint/hint.component';
import { RulesService } from './services/rules.service';
import { TaskViewComponent } from './task-view/task-view.component';
import { TaskDescriptionComponent } from './task-description/task-description.component';
import { TestingComponent } from './testing/testing.component';
import { UserComponent } from './user/user.component';
import { LandingComponent } from './landing/landing.component';

@NgModule({
    declarations: [
        AppComponent,
        ExpressionEvaluationComponent,
        FunctionDefinitionComponent,
        UserComponent,
        TaskDescriptionComponent,
        TestingComponent,
        HintComponent,
        TaskViewComponent,
        LandingComponent,
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
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
