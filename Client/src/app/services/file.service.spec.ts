import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { InvokableActionMember, DomainObjectRepresentation, MenusRepresentation, ActionResultRepresentation, DomainServicesRepresentation, Value } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { EmptyFileView } from '../models/file-view';
import { FileService } from './file.service';


describe('FileService', () => {
  let service: FileService;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;

  contextServiceSpy = jasmine.createSpyObj('ConfigService', ['getServices'], { config: { appPath: 'testPath' } });
  repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['populate', 'invoke'])

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = new FileService(contextServiceSpy, repLoaderSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get the getFile action', fakeAsync(() => {
    const testAction = {} as InvokableActionMember;
    const testService = { actionMember: (s: string) => testAction } as unknown as DomainObjectRepresentation;
    const testServices = { getService: (s: string) => testService } as unknown as DomainServicesRepresentation;

    const tssp = Promise.resolve(testServices);
    const tsp = Promise.resolve(testService);

    contextServiceSpy.getServices.and.returnValue(tssp);
    repLoaderSpy.populate.and.returnValue(tsp);
  
    service.getFileAction();
    tick();

    expect(contextServiceSpy.getServices).toHaveBeenCalled();
    expect(repLoaderSpy.populate).toHaveBeenCalledWith(testService);
    tick();
    expect(service.fileAction).toEqual(testAction);
  }));

  it('should get the saveFile action', fakeAsync(() => {
    const testAction = {} as InvokableActionMember;
    const testService = { actionMember: (s: string) => testAction } as unknown as DomainObjectRepresentation;
    const testServices = { getService: (s: string) => testService } as unknown as DomainServicesRepresentation;

    const tssp = Promise.resolve(testServices);
    const tsp = Promise.resolve(testService);

    contextServiceSpy.getServices.and.returnValue(tssp);
    repLoaderSpy.populate.and.returnValue(tsp);
  
    service.getSaveAction();
    tick();

    expect(contextServiceSpy.getServices).toHaveBeenCalled();
    expect(repLoaderSpy.populate).toHaveBeenCalledWith(testService);
    tick();
    expect(service.saveAction).toEqual(testAction);
  }));

  it('should get the file', fakeAsync(() => {

    service.fileAction = {} as InvokableActionMember;

    const testAr = { result: () => ({ object: () => null }) } as ActionResultRepresentation;

    repLoaderSpy.invoke.and.returnValue(Promise.resolve(testAr));

    service.loadFile('fileid').then(o => expect(o).toBe(EmptyFileView));
    tick();

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.fileAction, { id: new Value("fileid") } as Dictionary<Value>, {} as Dictionary<Object>);
  }));

  it('should save the file', fakeAsync(() => {

    service.saveAction = {} as InvokableActionMember;

    const testAr = { result: () => ({ object: () => null }) } as ActionResultRepresentation;

    repLoaderSpy.invoke.and.returnValue(Promise.resolve(testAr));

    service.saveFile('fileid', 'test content').then(o => expect(o).toBe(true));
    tick();

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.saveAction, { id: new Value("fileid"), content: new Value('test content') } as Dictionary<Value>, {} as Dictionary<Object>);
  }));
});
