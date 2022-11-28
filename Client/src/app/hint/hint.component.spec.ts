import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Subject } from 'rxjs';
import { EmptyHintUserView, IHintUserView } from '../models/hint';
import { EmptyTaskUserView, ITaskUserView } from '../models/task';
import { TaskService } from '../services/task.service';

import { HintComponent } from './hint.component';

describe('HintComponent', () => {
  let component: HintComponent;
  let fixture: ComponentFixture<HintComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITaskUserView>();
  const testTask = structuredClone(EmptyTaskUserView);
  testTask.Id = 66;

  const hint0 = structuredClone(EmptyHintUserView) as IHintUserView;
  hint0.Title = 'hint0 title';
  hint0.Contents = '';
  hint0.CostOfNextHint = 1;
  hint0.NextHintNo = 1;
  hint0.PreviousHintNo = 0;

  const hint0bis = structuredClone(EmptyHintUserView) as IHintUserView;
  hint0bis.Title = 'hint0 title';
  hint0bis.Contents = '';
  hint0bis.CostOfNextHint = 0;
  hint0bis.NextHintNo = 0;
  hint0bis.PreviousHintNo = 0;

  const hintError = structuredClone(EmptyHintUserView);
  
  const hint1 = structuredClone(EmptyHintUserView) as IHintUserView;
  hint1.Title = 'hint1 title';
  hint1.Contents = 'hint1 contents';
  hint1.CostOfNextHint = 1;
  hint1.NextHintNo = 2;
  hint1.PreviousHintNo = 0;

  const hint1bis = structuredClone(EmptyHintUserView) as IHintUserView;
  hint1bis.Title = 'hint1 title';
  hint1bis.Contents = 'hint1 contents';
  hint1bis.CostOfNextHint = 0;
  hint1bis.NextHintNo = 2;
  hint1bis.PreviousHintNo = 0;

  const hint2 = structuredClone(EmptyHintUserView) as IHintUserView;
  hint2.Title = 'hint2 title';
  hint2.Contents = 'hint2 contents';
  hint2.CostOfNextHint = 0;
  hint2.NextHintNo = 0;
  hint2.PreviousHintNo = 1;

  beforeEach(async () => {
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['loadHint', 'loadTask'], { currentTask: taskSubject });
    taskServiceSpy.loadHint.and.returnValue(Promise.resolve(hint0));

    await TestBed.configureTestingModule({
      declarations: [HintComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: TaskService,
          useValue: taskServiceSpy
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HintComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should get the hint if new task', fakeAsync(() => {
    
    // hint 0
    taskSubject.next(testTask);
    expect(taskServiceSpy.loadHint).toHaveBeenCalledWith(66, 0);
    expect(taskServiceSpy.loadTask).not.toHaveBeenCalled();
    tick();
    
    expect(component.canGetNextHint()).toEqual(true);
    expect(component.canViewNextHint()).toEqual(false);
    expect(component.canViewPreviousHint()).toEqual(false);
    expect(component.title).toEqual("hint0 title");
    expect(component.hintHtml).toEqual("Click 'Next Hint' to use the first Hint");

    // hint 1
    taskServiceSpy.loadHint.and.returnValue(Promise.resolve(hint1));
    component.getNextHint(); 
    expect(taskServiceSpy.loadHint).toHaveBeenCalledWith(66, 1);
    tick();
    expect(taskServiceSpy.loadTask).toHaveBeenCalledWith(66);
    taskServiceSpy.loadTask.calls.reset();
    expect(component.canGetNextHint()).toEqual(true);
    expect(component.canViewNextHint()).toEqual(false);
    expect(component.canViewPreviousHint()).toEqual(false);
    expect(component.title).toEqual("hint1 title");
    expect(component.hintHtml).toEqual('hint1 contents');

    // hint2
    taskServiceSpy.loadHint.and.returnValue(Promise.resolve(hint2));
    component.getNextHint(); 
    expect(taskServiceSpy.loadHint).toHaveBeenCalledWith(66, 2);
    tick();
    expect(taskServiceSpy.loadTask).toHaveBeenCalledWith(66);
    taskServiceSpy.loadTask.calls.reset();

    expect(component.canGetNextHint()).toEqual(false);
    expect(component.canViewNextHint()).toEqual(false);
    expect(component.canViewPreviousHint()).toEqual(true);
    expect(component.title).toEqual("hint2 title");
    expect(component.hintHtml).toEqual('hint2 contents');

    // back to 1
    taskServiceSpy.loadHint.and.returnValue(Promise.resolve(hint1bis));
    component.viewPreviousHint(); 
    expect(taskServiceSpy.loadHint).toHaveBeenCalledWith(66, 1);
    tick();
    expect(taskServiceSpy.loadTask).not.toHaveBeenCalled();

    expect(component.canGetNextHint()).toEqual(false);
    expect(component.canViewNextHint()).toEqual(true);
    expect(component.canViewPreviousHint()).toEqual(false);
    expect(component.title).toEqual("hint1 title");
    expect(component.hintHtml).toEqual('hint1 contents');

  }));

  it('should not get the hint if same task', fakeAsync(() => {
    
    component.currentTask = testTask;

    taskSubject.next(testTask);
    expect(taskServiceSpy.loadHint).not.toHaveBeenCalled();
    expect(taskServiceSpy.loadTask).not.toHaveBeenCalled();
    
  }));

  it('should display message if no hints', fakeAsync(() => {
    taskServiceSpy.loadHint.and.returnValue(Promise.resolve(hint0bis));

    // hint 0
    taskSubject.next(testTask);
    expect(taskServiceSpy.loadHint).toHaveBeenCalledWith(66, 0);
    tick();
    
    expect(component.canGetNextHint()).toEqual(false);
    expect(component.canViewNextHint()).toEqual(false);
    expect(component.canViewPreviousHint()).toEqual(false);
    expect(component.title).toEqual("hint0 title");
    expect(component.hintHtml).toEqual("There are no Hints for this task.");
  }));

  it('should handle error', fakeAsync(() => {
    taskServiceSpy.loadHint.and.returnValue(Promise.resolve(hintError));

    taskSubject.next(testTask);
    expect(taskServiceSpy.loadHint).toHaveBeenCalledWith(66, 0);
    tick();
    
    expect(component.canGetNextHint()).toEqual(false);
    expect(component.canViewNextHint()).toEqual(false);
    expect(component.canViewPreviousHint()).toEqual(false);
    expect(component.title).toEqual("Hint");
    expect(component.hintHtml).toEqual("There are no Hints for this task.");
  }));
});
