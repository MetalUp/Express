import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Params } from '@angular/router';
import { JobeServerService } from '../services/jobe-server.service';
import { ExpressionEvaluationComponent } from './expression-evaluation.component';
import { of } from 'rxjs';
import { HttpParams } from '@angular/common/http';

describe('ExpressionEvaluationComponent', () => {
  let component: ExpressionEvaluationComponent;
  let fixture: ComponentFixture<ExpressionEvaluationComponent>;
  let mockJobeServerService : jasmine.SpyObj<JobeServerService>;
  let mockActivatedRoute: jasmine.SpyObj<ActivatedRoute>;

  beforeEach(async () => {
    mockJobeServerService = jasmine.createSpyObj('JobeServerService', ['submit_run', 'get_languages']);
    mockActivatedRoute = jasmine.createSpyObj('ActivatedRoute', [], {'queryParams' : of<Params>(new HttpParams() )  });
    mockJobeServerService.get_languages.and.returnValue(of<[string, string][]>([["1", "2"]]));

    await TestBed.configureTestingModule({
      declarations: [ExpressionEvaluationComponent],
      providers: [
        {
          provide: JobeServerService,
          useValue: mockJobeServerService
        },
        {
          provide: ActivatedRoute,
          useValue: mockActivatedRoute
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ExpressionEvaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    
    
    expect(component).toBeTruthy();
  });
});
