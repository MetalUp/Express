import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Subject } from 'rxjs';
import { EmptyHint } from '../models/hint';
import { EmptyTask, ITask } from '../models/task';
import { TaskService } from '../services/task.service';

import { HintComponent } from './hint.component';

describe('HintComponent', () => {
  let component: HintComponent;
  let fixture: ComponentFixture<HintComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();
  const testTask = structuredClone(EmptyTask);

  const hint1 = structuredClone(EmptyHint);
  hint1.Title = 'hint1 title';
  hint1.HtmlFile = ['hint1url', 'hint1mt'];
  hint1.CostInMarks = 1;

  const hint2 = structuredClone(EmptyHint);
  hint2.Title = 'hint2 title';
  hint2.HtmlFile = ['hint2url', 'hint2mt'];
  hint2.CostInMarks = 2;

  testTask.Hints = [hint1, hint2];

  beforeEach(async () => {
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load', 'getFile', 'gotoTask'], { currentTask: taskSubject });
    taskServiceSpy.getFile.and.returnValue(Promise.resolve('test html'));

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

  it('should get the hint html file', fakeAsync(() => {

    taskServiceSpy.getFile.and.returnValue(Promise.resolve('hint1 html'));
    
    taskSubject.next(testTask);

    expect(component.hasNextHint()).toEqual(true);

    expect(component.title).toEqual("Hint 0/2 -0 marks");
    expect(component.hintHtml).toEqual('Click Next to use the first Hint');

    component.onNextHint(); 

    expect(taskServiceSpy.getFile).toHaveBeenCalledWith(['hint1url', 'hint1mt']);
    expect(component.hintIndex).toEqual(0);
    tick();
    expect(component.title).toEqual("Hint 1/2 -1 marks");
    expect(component.hintHtml).toEqual('hint1 html');
    expect(component.hasPreviousHint()).toEqual(false);
    expect(component.hasNextHint()).toEqual(true);

    taskServiceSpy.getFile.and.returnValue(Promise.resolve('hint2 html'));

    component.onNextHint(); 

    expect(taskServiceSpy.getFile).toHaveBeenCalledWith(['hint2url', 'hint2mt']);
    expect(component.hintIndex).toEqual(1);
    tick();
    expect(component.title).toEqual("Hint 2/2 -2 marks");
    expect(component.hintHtml).toEqual('hint2 html');
    expect(component.hasPreviousHint()).toEqual(true);
    expect(component.hasNextHint()).toEqual(false);

    taskServiceSpy.getFile.and.returnValue(Promise.resolve('hint1 html'));

    component.onPreviousHint(); 

    expect(taskServiceSpy.getFile).toHaveBeenCalledWith(['hint1url', 'hint1mt']);
    expect(component.hintIndex).toEqual(0);
    tick();
    expect(component.title).toEqual("Hint 1/2 -1 marks");
    expect(component.hintHtml).toEqual('hint1 html');
    expect(component.hasPreviousHint()).toEqual(false);
    expect(component.hasNextHint()).toEqual(true);

  }));
});
