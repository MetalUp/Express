import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Params } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

import { Subject} from 'rxjs'
import { NO_ERRORS_SCHEMA } from '@angular/core';


import { StudentViewComponent } from './student-view.component';
import { TaskService } from '../services/task.service';
import { ITask } from '../services/task';

describe('StudentViewComponentComponent', () => {
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

  beforeEach(async () => {

    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], { currentTask: taskSubject });

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        StudentViewComponent
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
    const fixture = TestBed.createComponent(StudentViewComponent);
    const app = fixture.componentInstance;

    app.ngOnInit();

    taskSubject.next({Language: 'testlanguage'} as ITask);

    expect(app.language).toEqual('testlanguage');
  });
});
