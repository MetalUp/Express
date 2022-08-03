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
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load', 'getHtml', 'gotoTask'], { currentTask: taskSubject });

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

  it('should get the task html file', () => {

    taskServiceSpy.getHtml.and.returnValue(of('test html'));

    const testTask = { Description: 'testfile.html' } as unknown as ITask;
    taskSubject.next(testTask);

    expect(taskServiceSpy.getHtml).toHaveBeenCalledWith('testfile.html');
    expect(component.currentTask).toEqual(testTask);
    expect(component.taskHtml).toEqual('test html');
  });

  it('should get the hint html file', () => {

    taskServiceSpy.getHtml.and.returnValue(of('hint1 html'));

    const testTask = { Hints: ['hint1.html', 'hint2.html']} as unknown as ITask;
    taskSubject.next(testTask);

    expect(component.hasHint()).toEqual(true);

    component.onFirstHint(); 

    expect(taskServiceSpy.getHtml).toHaveBeenCalledWith('hint1.html');
    expect(component.hintIndex).toEqual(0);
    expect(component.hintHtml).toEqual('hint1 html');
    expect(component.hasPreviousHint()).toEqual(false);
    expect(component.hasNextHint()).toEqual(true);

    taskServiceSpy.getHtml.and.returnValue(of('hint2 html'));

    component.onNextHint(); 

    expect(taskServiceSpy.getHtml).toHaveBeenCalledWith('hint2.html');
    expect(component.hintIndex).toEqual(1);
    expect(component.hintHtml).toEqual('hint2 html');
    expect(component.hasPreviousHint()).toEqual(true);
    expect(component.hasNextHint()).toEqual(false);

    taskServiceSpy.getHtml.and.returnValue(of('hint1 html'));

    component.onPreviousHint(); 

    expect(taskServiceSpy.getHtml).toHaveBeenCalledWith('hint1.html');
    expect(component.hintIndex).toEqual(0);
    expect(component.hintHtml).toEqual('hint1 html');
    expect(component.hasPreviousHint()).toEqual(false);
    expect(component.hasNextHint()).toEqual(true);

    component.onFirstHint(); 

    expect(taskServiceSpy.getHtml).toHaveBeenCalledWith('hint1.html');
    expect(component.hintIndex).toEqual(0);
    expect(component.hintHtml).toEqual('hint1 html');
    expect(component.hasPreviousHint()).toEqual(false);
    expect(component.hasNextHint()).toEqual(true);
  });

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

});
