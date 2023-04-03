import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { FileService } from '../services/file.service';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { CustomEditorComponent } from './custom-editor.component';
import { Subject } from 'rxjs';
import { IFileView } from '../models/file-view';
import { Location } from '@angular/common';
import { CompileServerService } from '../services/compile-server.service';
import { ILanguageView } from '../models/language-view';

describe('CustomEditorComponent', () => {
  let component: CustomEditorComponent;
  let fixture: ComponentFixture<CustomEditorComponent>;

  const mapSub = new Subject<ParamMap>();
  const testFileView = { Content: "test content", Mime: 'test/mime', LanguageAlphaName: 'test language' } as IFileView;
  const languagesSub = new Subject<ILanguageView[]>();

  const activatedRouteSpy: jasmine.SpyObj<ActivatedRoute> = jasmine.createSpyObj('ActivatedRoute', ['navigate'], { paramMap: mapSub });

  const fileServiceSpy: jasmine.SpyObj<FileService> = jasmine.createSpyObj('FileService', ['loadFile', 'saveFile']);

  const compileServiceSpy: jasmine.SpyObj<CompileServerService> = jasmine.createSpyObj('CompileService', [], {languages$ : languagesSub});

  const locationSpy: jasmine.SpyObj<Location> = jasmine.createSpyObj('Location', ['back', 'path']);

  beforeEach(async () => {

    fileServiceSpy.loadFile.and.returnValue(Promise.resolve(testFileView));
   

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
        },
        {
          provide: CompileServerService,
          useValue: compileServiceSpy
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

  it('should load the file view', fakeAsync(() => {
    mapSub.next({ get: () => 'file_id' } as unknown as ParamMap);

    expect(fileServiceSpy.loadFile).toHaveBeenCalledWith('file_id');
    tick();

    expect(component.editContent).toBe('test content');
    expect(component.mime).toBe('test/mime');
    expect(component.selectedLanguage).toBe('test language');
    expect(component.loaded).toBeTrue();
  }));

  it('should save the file content and return if ok', fakeAsync(() => {
    fileServiceSpy.saveFile.and.returnValue(Promise.resolve(true));
    mapSub.next({ get: () => 'file_id' } as unknown as ParamMap);
  
    component.editContent = "updated content";
    component.onSave();

    expect(fileServiceSpy.saveFile).toHaveBeenCalledWith('file_id', 'updated content');
    
  }));


  it('should save and close the file content and return if ok', fakeAsync(() => {
    fileServiceSpy.saveFile.and.returnValue(Promise.resolve(true));
    mapSub.next({ get: () => 'file_id' } as unknown as ParamMap);
  
    component.editContent = "updated content";
    component.onSaveAndClose();

    expect(fileServiceSpy.saveFile).toHaveBeenCalledWith('file_id', 'updated content');
    tick();

    expect(locationSpy.back).toHaveBeenCalled();
  }));

  it('should save the file content and warn if not ok', fakeAsync(() => {
    fileServiceSpy.saveFile.and.returnValue(Promise.resolve(false));
    mapSub.next({ get: () => 'file_id' } as unknown as ParamMap);
  
    component.editContent = "updated content";
    component.onSave();

    expect(fileServiceSpy.saveFile).toHaveBeenCalledWith('file_id', 'updated content');
    tick();

    //expect(locationSpy.back).not.toHaveBeenCalled();
    expect(component.warning).toBe('Save failed!');
  }));

});
