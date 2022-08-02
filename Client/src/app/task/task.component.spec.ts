import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { ITask } from '../services/task';
import { TaskService } from '../services/task.service';

import { TaskComponent } from './task.component';

describe('TaskComponent', () => {
  let component: TaskComponent;
  let fixture: ComponentFixture<TaskComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

  beforeEach(async () => {
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load', 'getHtml'], { currentTask: taskSubject });

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

  it('should get the html file', () => {

    taskServiceSpy.getHtml.and.returnValue(of('test html'));

    const testTask = { Description: 'testfile.html' } as unknown as ITask;
    taskSubject.next(testTask);

    expect(taskServiceSpy.getHtml).toHaveBeenCalledWith('testfile.html');
    expect(component.currentTask).toEqual(testTask);
    expect(component.innerHtml).toEqual('test html');
  });

});
