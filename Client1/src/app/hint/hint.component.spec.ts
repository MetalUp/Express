import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { ITask } from '../services/task';
import { TaskService } from '../services/task.service';

import { HintComponent } from './hint.component';

describe('TaskComponent', () => {
  let component: HintComponent;
  let fixture: ComponentFixture<HintComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

  beforeEach(async () => {
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load', 'getHtml', 'gotoTask'], { currentTask: taskSubject });
    taskServiceSpy.getHtml.and.returnValue(of('test html'));

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

  it('should get the hint html file', () => {

    taskServiceSpy.getHtml.and.returnValue(of('hint1 html'));

    const testTask = { Hints: ['hint1.html', 'hint2.html']} as unknown as ITask;
    taskSubject.next(testTask);

    expect(component.hasNextHint()).toEqual(true);

    expect(component.title).toEqual("Hint: ");
    expect(component.hintHtml).toEqual('Click Next to use the first Hint');

    component.onNextHint(); 

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

  });

});
