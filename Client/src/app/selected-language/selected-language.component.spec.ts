import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Params } from '@angular/router';
import { of, Subject } from 'rxjs';
import { JobeServerService } from '../services/jobe-server.service';
import { ITask } from '../services/task';
import { TaskService } from '../services/task.service';
import { SelectedLanguageComponent } from './selected-language.component';

describe('SelectedLanguageComponent', () => {
  let component: SelectedLanguageComponent;
  let fixture: ComponentFixture<SelectedLanguageComponent>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

  beforeEach(async () => {
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], { currentTask: taskSubject });

    await TestBed.configureTestingModule({
      declarations: [SelectedLanguageComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: TaskService,
          useValue: taskServiceSpy
        },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SelectedLanguageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should setup selected language', () => {
   
    component.ngOnInit();

    taskSubject.next({Language: 'language1'} as ITask);

    expect(component.selectedLanguage).toEqual("language1");
  });

});
