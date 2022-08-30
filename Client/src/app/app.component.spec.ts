import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Params } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { Subject} from 'rxjs'
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { TaskService } from './services/task.service';
import { ITask } from './services/task';

describe('AppComponent', () => {
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

  beforeEach(async () => {

    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], { currentTask: taskSubject });

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        AppComponent
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

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have as title 'ile-client'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('ile-client');
  });

  it(`should set the language`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;

    app.ngOnInit();

    taskSubject.next({Language: 'testlanguage'} as ITask);

    expect(app.language).toEqual('testlanguage');
  });
});
