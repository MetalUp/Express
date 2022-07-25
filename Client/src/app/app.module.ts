import { NgModule } from '@angular/core';
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

@NgModule({
  declarations: [
    AppComponent,
    ExpressionEvaluationComponent,
    FunctionDefinitionComponent,
    UserComponent,
    SelectedLanguageComponent,
    PlaceholderComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
