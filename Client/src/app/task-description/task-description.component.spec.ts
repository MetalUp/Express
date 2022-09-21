import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Subject } from 'rxjs';
import { ITask } from '../services/task';
import { TaskService } from '../services/task.service';

import { TaskDescriptionComponent } from './task-description.component';

describe('TaskDescriptionComponent', () => {
  let component: TaskDescriptionComponent;
  let fixture: ComponentFixture<TaskDescriptionComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

  beforeEach(async () => {
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load', 'getFile', 'gotoTask'], { currentTask: taskSubject });
    taskServiceSpy.getFile.and.returnValue(Promise.resolve('test html'));

    await TestBed.configureTestingModule({
      declarations: [TaskDescriptionComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: TaskService,
          useValue: taskServiceSpy
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskDescriptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should get the task html file', fakeAsync(() => {

    const testTask = { Description: ['testUrl', 'testMediaType'] } as unknown as ITask;
    taskSubject.next(testTask);

    expect(taskServiceSpy.getFile).toHaveBeenCalledWith(['testUrl', 'testMediaType']);
    expect(component.currentTask).toEqual(testTask);
    tick();
    expect(component.taskHtml).toEqual('test html');
  }));

  it('should disable next task if no next task', () => {

    const testTask = { NextTask: ""} as unknown as ITask;
    taskSubject.next(testTask);

    expect(component.hasNextTask()).toEqual(false);   
  });


  it('should get the next task', () => {

    const testTask = { NextTask: "nexttask.json"} as unknown as ITask;
    taskSubject.next(testTask);
    expect(component.hasNextTask()).toEqual(true);
    component.onNextTask();
    expect(taskServiceSpy.gotoTask).toHaveBeenCalledWith('nexttask.json');
  });

  it('should disable previous task if no previous task', () => {

    const testTask = { PreviousTask: ""} as unknown as ITask;
    taskSubject.next(testTask);

    expect(component.hasPreviousTask()).toEqual(false);   
  });

  it('should get the previous task', () => {

    const testTask = { PreviousTask: "previoustask.json"} as unknown as ITask;
    taskSubject.next(testTask);
    expect(component.hasPreviousTask()).toEqual(true);
    component.onPreviousTask();
    expect(taskServiceSpy.gotoTask).toHaveBeenCalledWith('previoustask.json');
  });

  it('should handle errors when getting task html file', () => {

    taskServiceSpy.getFile.and.returnValue(Promise.reject(() => { status: 404 }));

    const testTask = { Description: ['testUrl', 'testMediaType'] } as unknown as ITask;
    taskSubject.next(testTask);

    expect(taskServiceSpy.getFile).toHaveBeenCalledWith(['testUrl', 'testMediaType']);
    expect(component.currentTask).toEqual(testTask);
    expect(component.taskHtml).toEqual('');
  });
});
