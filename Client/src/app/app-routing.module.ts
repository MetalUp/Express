import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth-guard.guard';
import { StudentViewComponent } from './student-view/student-view.component';

const routes: Routes = [
  { path: '',  redirectTo: 'task/default_python', pathMatch: 'full'},
  { path: 'task/:id', component: StudentViewComponent, canActivate: [AuthGuard] },
  { path: '**',  redirectTo: 'task/default_python', pathMatch: 'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
