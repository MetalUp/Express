import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { ITaskUserView } from '../models/task';
import { TaskService } from '../services/task.service';

import { TaskDescriptionComponent } from './task-description.component';

describe('TaskDescriptionComponent', () => {
  let component: TaskDescriptionComponent;
  let fixture: ComponentFixture<TaskDescriptionComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITaskUserView>();
  let routerSpy: jasmine.SpyObj<Router>;

  routerSpy = jasmine.createSpyObj('Router', ['navigate']);

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

  it('should get the task html file', fakeAsync(() => {

    const testTask = { Description: 'test html' } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.currentTask).toEqual(testTask);
    tick();
    expect(component.taskHtml).toEqual('test html');
  }));

  it('should disable next task if no next task', () => {

    const testTask = { NextTaskId: undefined, NextTaskEnabled: true } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canViewNextTask()).toEqual(false);
  });

  it('should disable next task if not enabled', () => {

    const testTask = { NextTaskId: 1, NextTaskEnabled: false } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canViewNextTask()).toEqual(false);
  });

  it('should enable next task', () => {

    const testTask = { NextTaskId: 1, NextTaskEnabled: true } as unknown as ITaskUserView;
    taskSubject.next(testTask);

    expect(component.canViewNextTask()).toEqual(true);
  });

  it('should get the next task', () => {

    const testTask = { NextTaskId: 55, NextTaskEnabled: true } as unknown as ITaskUserView;
    taskSubject.next(testTask);
    expect(component.canViewNextTask()).toEqual(true);
    component.viewNextTask();
    expect(taskServiceSpy.gotoTask).toHaveBeenCalledWith(55);
  });

  it('should disable previous task if no previous task', () => {

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
});
