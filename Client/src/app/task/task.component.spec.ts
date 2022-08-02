import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { ITask } from '../services/task';
import { TaskService } from '../services/task.service';

import { TaskComponent } from './task.component';

describe('TaskComponent', () => {
  let component: TaskComponent;
  let fixture: ComponentFixture<TaskComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
 
  beforeEach(async () => {
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load', 'getHtml'], {currentTask : new Subject<Task>()}   );

    await TestBed.configureTestingModule({
      declarations: [TaskComponent],
      providers: [
        {
          provide: TaskService,
          useValue: taskServiceSpy
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
