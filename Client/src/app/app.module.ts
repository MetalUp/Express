import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ExpressionEvaluationComponent } from './expression-evaluation/expression-evaluation.component';
import { FormsModule } from '@angular/forms';
import { FunctionDefinitionComponent } from './function-definition/function-definition.component';
import { UserComponent } from './user/user.component';
import { SelectedLanguageComponent } from './selected-language/selected-language.component';
import { PlaceholderComponent } from './placeholder/placeholder.component';
import { rulesFactory, RulesService } from './services/rules.service';
import { TaskComponent } from './task/task.component';

@NgModule({
  declarations: [
    AppComponent,
    ExpressionEvaluationComponent,
    FunctionDefinitionComponent,
    UserComponent,
    SelectedLanguageComponent,
    PlaceholderComponent,
    TaskComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [
    { provide: APP_INITIALIZER, useFactory: rulesFactory, deps: [RulesService], multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
