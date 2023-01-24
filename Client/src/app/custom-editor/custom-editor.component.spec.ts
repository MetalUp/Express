import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { FileService } from '../services/file.service';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { CustomEditorComponent } from './custom-editor.component';
import { Subject } from 'rxjs';

describe('CustomEditorComponent', () => {
  let component: CustomEditorComponent;
  let fixture: ComponentFixture<CustomEditorComponent>;


  let fileServiceSpy: jasmine.SpyObj<FileService>;
  let activatedRouteSpy: jasmine.SpyObj<ActivatedRoute>;
  let locationSpy: jasmine.SpyObj<Location>;
  let mapSub = new Subject<ParamMap>();

  activatedRouteSpy = jasmine.createSpyObj('ActivatedRoute', ['navigate'], { paramMap: mapSub });

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CustomEditorComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: activatedRouteSpy
        },
        {
          provide: Location,
          useValue: locationSpy
        },
        {
          provide: FileService,
          useValue: fileServiceSpy
        }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(CustomEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
