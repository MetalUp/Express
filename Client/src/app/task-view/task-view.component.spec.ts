import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { Subject} from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { TaskViewComponent } from './task-view.component';
import { TaskService } from '../services/task.service';
import { ITaskUserView } from '../models/task-user-view';

describe('TaskViewComponent', () => {
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITaskUserView>();

  beforeEach(async () => {

    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], { currentTask: taskSubject });

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        TaskViewComponent
      ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: TaskService,
          useValue: taskServiceSpy
        }
      ]
    }).compileComponents();
  });

  it(`should set the language`, () => {
    const fixture = TestBed.createComponent(TaskViewComponent);
    const app = fixture.componentInstance;

    app.ngOnInit();

    taskSubject.next({Language: 'testlanguage'} as ITaskUserView);

    expect(app.language).toEqual('testlanguage');
  });
});
