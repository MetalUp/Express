import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StudentViewComponent } from './student-view/student-view.component';

const routes: Routes = [
  { path: '',  redirectTo: 'task/default_python', pathMatch: 'full'},
  { path: 'task/:id', component: StudentViewComponent },
  { path: '**',  redirectTo: 'task/default_python', pathMatch: 'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
