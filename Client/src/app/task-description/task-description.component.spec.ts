import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { ITaskUserView } from '../models/task-user-view';
import { TaskService } from '../services/task.service';

import { TaskDescriptionComponent } from './task-description.component';

describe('TaskDescriptionComponent', () => {
  let component: TaskDescriptionComponent;
  let fixture: ComponentFixture<TaskDescriptionComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITaskUserView>();
  let routerSpy: jasmine.SpyObj<Router>;

  routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl', 'createUrlTree']);

  beforeEach(async () => {
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load', 'gotoTask'], { currentTask: taskSubject });
   
    await TestBed.configureTestingModule({
      declarations: [TaskDescriptionComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: TaskService,
          useValue: taskServiceSpy
        },
        {
          provide: Router,
          useValue: routerSpy
        },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TaskDescriptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should get the task html if changed', fakeAsync(() => {

    const testTask = { Description: 'test html' } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.currentTask).toEqual(testTask);
    tick();
    expect(component.taskHtml).toEqual('test html');
  }));

  it('should not get the task html if reloaded', fakeAsync(() => {

    const testTask1 = { Id: 66, Description: 'test html' } as unknown as ITaskUserView;

    taskSubject.next(testTask1);
    tick();
    expect(component.taskHtml).toEqual('test html');
    
    const testTask2 = { Id: 66, Description: 'test html new' } as unknown as ITaskUserView;
    taskSubject.next(testTask2);
    tick();
    expect(component.taskHtml).toEqual('test html');
  }));

  it('should disable get next task if no next task', () => {

    const testTask = { NextTaskId: 0, Completed: true,NextTaskIsStarted : false } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canGetNextTask()).toEqual(false);
  });

  it('should disable get next task if undefined next task', () => {

    const testTask = { NextTaskId: undefined, Completed: true,NextTaskIsStarted : false } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canGetNextTask()).toEqual(false);
  });

  it('should disable get next task if not Completed', () => {

    const testTask = { NextTaskId: 1, Completed: false, NextTaskIsStarted : false } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canGetNextTask()).toEqual(false);
  });

  it('should disable get next task if next task started', () => {

    const testTask = { NextTaskId: 1, Completed: true, NextTaskIsStarted : true } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canGetNextTask()).toEqual(false);
  });

  it('should disable view next task if no next task', () => {

    const testTask = { NextTaskId: 0, NextTaskIsStarted: true } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canViewNextTask()).toEqual(false);
  });

  it('should disable view next task if undefined next task', () => {

    const testTask = { NextTaskId: undefined, NextTaskIsStarted: true } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canViewNextTask()).toEqual(false);
  });

  it('should disable view next task if not started', () => {

    const testTask = { NextTaskId: 1, NextTaskIsStarted: false } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canViewNextTask()).toEqual(false);
  });

  it('should enable get next task', () => {

    const testTask = { NextTaskId: 1, Completed: true, NextTaskIsStarted: false } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canGetNextTask()).toEqual(true);
  });

  it('should enable view next task', () => {

    const testTask = { NextTaskId: 1, NextTaskIsStarted: true } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canViewNextTask()).toEqual(true);
  });

  it('should get the next task', () => {

    const testTask = { NextTaskId: 55, Completed: true, NextTaskIsStarted: false } as unknown as ITaskUserView;
    taskSubject.next(testTask);
    expect(component.canGetNextTask()).toEqual(true);
    component.getNextTask();
    expect(taskServiceSpy.gotoTask).toHaveBeenCalledWith(55);
  });

  it('should view the next task', () => {

    const testTask = { NextTaskId: 56, NextTaskIsStarted: true } as unknown as ITaskUserView;
    taskSubject.next(testTask);
    expect(component.canViewNextTask()).toEqual(true);
    component.viewNextTask();
    expect(taskServiceSpy.gotoTask).toHaveBeenCalledWith(56);
  });

  it('should disable previous task if no previous task', () => {

    const testTask = { PreviousTaskId: 0 } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canViewPreviousTask()).toEqual(false);
  });

  it('should disable previous task if undefined previous task', () => {

    const testTask = { PreviousTaskId: undefined } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canViewPreviousTask()).toEqual(false);
  });

  it('should enable previous task', () => {

    const testTask = { PreviousTaskId: 1 } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canViewPreviousTask()).toEqual(true);
  });

  it('should get the previous task', () => {

    const testTask = { PreviousTaskId: 44 } as unknown as ITaskUserView;
    taskSubject.next(testTask);
    expect(component.canViewPreviousTask()).toEqual(true);
    component.viewPreviousTask();
    expect(taskServiceSpy.gotoTask).toHaveBeenCalledWith(44);
  });

  it('should disable return to assignment next task', () => {

    const testTask = { NextTaskId: 1, Completed: true } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canReturnToAssignment()).toEqual(false);
  });

  it('should disable return to assignment not complete', () => {

    const testTask = { NextTaskId: 0, Completed: false } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canReturnToAssignment()).toEqual(false);
  });

  it('should enable return to assignment', () => {

    const testTask = { NextTaskId: 0, Completed: true } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canReturnToAssignment()).toEqual(true);
  });

  it('should navigate to assignment', () => {

    const testTask = { NextTaskId: 0, Completed: true, AssignmentId : 77 } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    component.returnToAssignment();

    expect(routerSpy.createUrlTree).toHaveBeenCalledWith([ '/dashboard/object'], { queryParams: Object({ o1: 'Model.Types.Assignment--77' })});
    expect(routerSpy.navigateByUrl).toHaveBeenCalled();
  });
});
