import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ITask } from '../services/task';
import { TaskService } from '../services/task.service';
import { of, Subject } from 'rxjs';

import { TestingComponent } from './testing.component';
import { JobeServerService } from '../services/jobe-server.service';

describe('TestingComponent', () => {
  let component: TestingComponent;
  let fixture: ComponentFixture<TestingComponent>;
  let jobeServerServiceSpy: jasmine.SpyObj<JobeServerService>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

  beforeEach(async () => {
    jobeServerServiceSpy = jasmine.createSpyObj('JobeServerService', ['hasFunctionDefinitions'], { "selectedLanguage": "csharp" });
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load', 'getHtml'], { currentTask: taskSubject });

    await TestBed.configureTestingModule({
      declarations: [TestingComponent],
      providers: [
        {
          provide: JobeServerService,
          useValue: jobeServerServiceSpy
        },
        {
          provide: TaskService,
          useValue: taskServiceSpy
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TestingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should get the test code file', () => {

    taskServiceSpy.getHtml.and.returnValue(of('test code'));

    const testTask = { Tests: 'testfile.cs' } as unknown as ITask;
    taskSubject.next(testTask);

    expect(taskServiceSpy.getHtml).toHaveBeenCalledWith('testfile.cs');
    expect(component.tests).toEqual('test code');
  });

  it('should disable run tests until code compiled', () => {

    jobeServerServiceSpy.hasFunctionDefinitions.and.returnValue(false);
    expect(component.canRunTests()).toEqual(false);
  });

  it('should enable run tests when code compiled', () => {

    jobeServerServiceSpy.hasFunctionDefinitions.and.returnValue(true);
    expect(component.canRunTests()).toEqual(true);
  });
});
