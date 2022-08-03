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
    jobeServerServiceSpy = jasmine.createSpyObj('JobeServerService', ['submit_run', 'get_languages'], { "selectedLanguage": "csharp" });
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
});
