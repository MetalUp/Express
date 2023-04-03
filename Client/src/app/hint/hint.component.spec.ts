import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Subject } from 'rxjs';
import { EmptyHintUserView, IHintUserView } from '../models/hint-user-view';
import { EmptyTaskUserView, ITaskUserView } from '../models/task-user-view';
import { TaskService } from '../services/task.service';

import { HintComponent } from './hint.component';

describe('HintComponent', () => {
  let component: HintComponent;
  let fixture: ComponentFixture<HintComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  const taskSubject = new Subject<ITaskUserView>();
  const testTask = structuredClone(EmptyTaskUserView);
  testTask.Id = 66;

  const hint0 = structuredClone(EmptyHintUserView) as IHintUserView;
  hint0.HintNo = 0;
  hint0.Title = 'hint0 title';
  hint0.Contents = '';
  hint0.NextHintAlreadyUsed = false;
  hint0.NextHintNo = 1;
  hint0.PreviousHintNo = 0;

  const hint0bis = structuredClone(EmptyHintUserView) as IHintUserView;
  hint0bis.HintNo = 0;
  hint0bis.Title = 'hint0 title';
  hint0bis.Contents = '';
  hint0bis.NextHintAlreadyUsed = true;
  hint0bis.NextHintNo = 0;
  hint0bis.PreviousHintNo = 0;

  const hintError = structuredClone(EmptyHintUserView);
  
  const hint1 = structuredClone(EmptyHintUserView) as IHintUserView;
  hint1.HintNo = 1;
  hint1.Title = 'hint1 title';
  hint1.Contents = 'hint1 contents';
  hint1.NextHintAlreadyUsed = false;
  hint1.NextHintNo = 2;
  hint1.PreviousHintNo = 0;

  const hint1bis = structuredClone(EmptyHintUserView) as IHintUserView;
  hint1bis.HintNo = 1;
  hint1bis.Title = 'hint1 title';
  hint1bis.Contents = 'hint1 contents';
  hint1bis.NextHintAlreadyUsed = true;
  hint1bis.NextHintNo = 2;
  hint1bis.PreviousHintNo = 0;

  const hint2 = structuredClone(EmptyHintUserView) as IHintUserView;
  hint2.HintNo = 2;
  hint2.Title = 'hint2 title';
  hint2.Contents = 'hint2 contents';
  hint2.NextHintAlreadyUsed = true;
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

  it('should  refresh the hint if same task', fakeAsync(() => {
    
    component.currentTask = testTask;
    component.currentHint = hint2;

    taskSubject.next(testTask);
    expect(taskServiceSpy.loadHint).toHaveBeenCalledWith(66, 2);
    expect(taskServiceSpy.loadTask).not.toHaveBeenCalled();
    
  }));

  it('should  get 0 hint if new task', fakeAsync(() => {
    
    component.currentTask = { Id:100} as unknown as ITaskUserView;
    component.currentHint = hint2;

    taskSubject.next(testTask);
    expect(taskServiceSpy.loadHint).toHaveBeenCalledWith(66, 0);
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

  it('should display cost of next hint', fakeAsync(() => {
    taskServiceSpy.loadHint.and.returnValue(Promise.resolve(hint0));

    // hint 0
    taskSubject.next(testTask);
    expect(taskServiceSpy.loadHint).toHaveBeenCalledWith(66, 0);
    tick();
    

    taskServiceSpy.loadHint.and.returnValue(Promise.resolve(hint1bis));
    component.getNextHint(); 
    tick();

    
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
