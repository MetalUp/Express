import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { TaskService } from './task.service';
import { first } from 'rxjs';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { ActionResultRepresentation, DomainObjectRepresentation, DomainServicesRepresentation, EntryType, InvokableActionMember, IPropertyMember, PropertyMember } from '@nakedobjects/restful-objects';

describe('TaskService', () => {
  let service: TaskService;
  let routerSpy: jasmine.SpyObj<Router>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    contextServiceSpy = jasmine.createSpyObj('ConfigService', ['getServices'], { config: { appPath: 'testPath' } });
    repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['populate', 'invoke']);

    TestBed.configureTestingModule({});
    service = new TaskService(routerSpy, contextServiceSpy, repLoaderSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get the service', fakeAsync(() => {

    const testAction = {} as InvokableActionMember;
    const testService = { actionMember: (_s: string) => testAction } as unknown as DomainObjectRepresentation;
    const testServices = { getService: (_s: string) => testService } as unknown as DomainServicesRepresentation;

    const tssp = Promise.resolve(testServices);
    const tsp = Promise.resolve(testService);

    contextServiceSpy.getServices.and.returnValue(tssp);
    repLoaderSpy.populate.and.returnValue(tsp);

    service.getService();
    tick();

    expect(contextServiceSpy.getServices).toHaveBeenCalled();
    expect(repLoaderSpy.populate).toHaveBeenCalledWith(testService);
    tick();
    expect(service.taskAccess).toEqual(testService);

  }));

  it('should get the task', fakeAsync(() => {

    const testAction = {} as InvokableActionMember;
    service.taskAccess = { actionMember: (_s: string) => testAction } as unknown as DomainObjectRepresentation;

    const object = new DomainObjectRepresentation();
    const pm = new PropertyMember({ value: 'testlanguage' } as IPropertyMember, object, 'Language');
    pm.entryType = () => EntryType.FreeForm;
    pm.isScalar = () => true;

    object.propertyMembers = () => ({ 'Language': pm });

    const actionResult = { result: () => ({ object: () => object }) } as unknown as ActionResultRepresentation;
    const promise = Promise.resolve(actionResult);

    repLoaderSpy.invoke.and.returnValue(promise);

    service.loadTask(1);
    service.currentTask.pipe(first()).subscribe(t => {
      expect(t.Language).toEqual('testlanguage');
    });
    tick();
    const params = service.params(1);
    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(testAction, jasmine.objectContaining(params), jasmine.objectContaining({}));

  }));

  it('should load empty task if task not found', fakeAsync(() => {

    const testAction = {} as InvokableActionMember;
    service.taskAccess = { actionMember: (_s: string) => testAction } as unknown as DomainObjectRepresentation;

    repLoaderSpy.invoke.and.returnValue(Promise.reject({ status: 404 }));

    service.loadTask(1);
    service.currentTask.pipe(first()).subscribe(t =>
      expect(t.Language).toEqual('')
    );
    tick();
    const params = service.params(1);
    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(testAction, jasmine.objectContaining(params), jasmine.objectContaining({}));


  }));

  it('should goto a new task', () => {
    service.currentTask.pipe(first()).subscribe(t =>
      expect(t.Language).toEqual('testlanguage')
    );

    service.gotoTask(9);

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/task/9']);
  });

  it('should return a hint', fakeAsync(() => {

    const testAction = {} as InvokableActionMember;
    service.taskAccess = { actionMember: (_s: string) => testAction } as unknown as DomainObjectRepresentation;

    const object = new DomainObjectRepresentation();
    const pm = new PropertyMember({ value: 'hint title' } as IPropertyMember, object, 'Title');
    pm.entryType = () => EntryType.FreeForm;
    pm.isScalar = () => true;

    object.propertyMembers = () => ({ 'Title': pm });

    const actionResult = { result: () => ({ object: () => object }) } as unknown as ActionResultRepresentation;
    const promise = Promise.resolve(actionResult);

    repLoaderSpy.invoke.and.returnValue(promise);

    service.loadHint(1, 1).then(t => {
      expect(t.Title).toEqual('hint title');
    });
    tick();

    const params = service.params(1, 1);
    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(testAction, jasmine.objectContaining(params), jasmine.objectContaining({}));

  }));

  it('should return empty hint if not found', fakeAsync(() => {

    const testAction = {} as InvokableActionMember;
    service.taskAccess = { actionMember: (_s: string) => testAction } as unknown as DomainObjectRepresentation;


    repLoaderSpy.invoke.and.returnValue(Promise.reject({ status: 404 }));

    service.loadHint(1, 1).then(t => {
      expect(t.Title).toEqual('');
    });
    tick();

    const params = service.params(1, 1);
    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(testAction, jasmine.objectContaining(params), jasmine.objectContaining({}));

  }));
});
