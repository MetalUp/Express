import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { LibModule as GeminiModule } from '@nakedobjects/gemini';
import { LibModule as ServicesModule } from '@nakedobjects/services';
//import { ObfuscateService } from '@nakedobjects/services';
import { LibModule as ViewModelModule } from '@nakedobjects/view-models';
import { RoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ExpressionEvaluationComponent } from './expression-evaluation/expression-evaluation.component';
import { FunctionDefinitionComponent } from './function-definition/function-definition.component';
import { HintComponent } from './hint/hint.component';
import { SelectedLanguageComponent } from './selected-language/selected-language.component';
import { rulesFactory, RulesService } from './services/rules.service';
import { StudentViewComponent } from './student-view/student-view.component';
import { TaskComponent } from './task/task.component';
import { TestingComponent } from './testing/testing.component';
import { UserComponent } from './user/user.component';
// import { Base64ObfuscateService } from './base64obfuscate.service';

@NgModule({
    declarations: [
        AppComponent,
        ExpressionEvaluationComponent,
        FunctionDefinitionComponent,
        UserComponent,
        SelectedLanguageComponent,
        TaskComponent,
        TestingComponent,
        HintComponent,
        StudentViewComponent,
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
    providers: [
        { provide: APP_INITIALIZER, useFactory: rulesFactory, deps: [RulesService], multi: true },
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
