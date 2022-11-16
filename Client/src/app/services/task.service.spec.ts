import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { TaskService } from './task.service';
import { first } from 'rxjs';
import { ConfigService, ContextService, RepLoaderService } from '@nakedobjects/services';
import { ActionResultRepresentation, CollectionMember, DomainObjectRepresentation, DomainServicesRepresentation, EntryType, InvokableActionMember, PropertyMember } from '@nakedobjects/restful-objects';
import { TimePickerComponent } from '@nakedobjects/gemini';

describe('TaskService', () => {
  let service: TaskService;
  let routerSpy: jasmine.SpyObj<Router>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;

  // let testService = { actionMember: (s: string) => ({}) } as unknown as DomainObjectRepresentation;
  // let testServices = { getService: (s: string) => testService } as unknown as DomainServicesRepresentation;

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    contextServiceSpy = jasmine.createSpyObj('ConfigService', ['getServices'], { config: { appPath: 'testPath' } });
    repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['populate', 'invoke'])

    //contextServiceSpy.getServices.and.returnValue(Promise.resolve(testServices));


    TestBed.configureTestingModule({});
    service = new TaskService(routerSpy, contextServiceSpy, repLoaderSpy)
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get the service', fakeAsync(() => {
   
    const testAction = {} as InvokableActionMember;
    const testService = { actionMember: (s: string) => testAction } as unknown as DomainObjectRepresentation;
    const testServices = { getService: (s: string) => testService } as unknown as DomainServicesRepresentation;

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
    service.taskAccess = { actionMember: (s: string) => testAction } as unknown as DomainObjectRepresentation;

    const object = new DomainObjectRepresentation();
    object.hateoasUrl = `testPath/objects/Model.Types.Task/testTask`;
    const pm = new PropertyMember({value : 'testlanguage'} as any, object, 'Language');
    pm.entryType = () => EntryType.FreeForm;
    pm.isScalar = () => true;

    object.propertyMembers = () => ({'Language': pm});
    
    const actionResult = {result: () => ({object: object})} as unknown as ActionResultRepresentation;

    const promise = Promise.resolve(actionResult);

    repLoaderSpy.invoke.and.returnValue(promise);

    service.loadTask(1);
    tick();
    
    const params = service.params(1);

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(testAction, jasmine.objectContaining(params),  jasmine.objectContaining({}));
    
    service.currentTask.pipe(first()).subscribe(t => {
        expect(t.Language).toEqual('testlanguage');
      }
    );
   
  }));

  // it('should load empty task if task not found', fakeAsync(() => {

  //   repLoaderSpy.populate.and.returnValue(Promise.reject({ status: 404 }));

  //   service.loadTask('testTask');

  //   expect(repLoaderSpy.populate).toHaveBeenCalledWith(jasmine.objectContaining({ hateoasUrl: `testPath/objects/Model.Types.Task/testTask` }), true);
  //   service.currentTask.pipe(first()).subscribe(t =>
  //     expect(t.Language).toEqual('')
  //   );
  // }));

  // it('should get the html file for the task', () => {
   
  //   repLoaderSpy.getFile.and.returnValue(Promise.resolve(new Blob(['task description'])));
  //   service.getFile(['testUrl', 'testMt']).then(t => expect(t).toBe('task description'));
  //   expect(repLoaderSpy.getFile).toHaveBeenCalledWith('testUrl', 'testMt', true);
  // });

  // it('should handle error when get the html file for the task', () => {
   
  //   repLoaderSpy.getFile.and.returnValue(Promise.reject());
  //   service.getFile(['testUrl', 'testMt']).then(t => expect(t).toBe(''));
  //   expect(repLoaderSpy.getFile).toHaveBeenCalledWith('testUrl', 'testMt', true);
  // });

  // it('should goto a new task', () => {
  //   service.currentTask.pipe(first()).subscribe(t =>
  //     expect(t.Language).toEqual('testlanguage')
  //   );

  //   service.gotoTask(9);

  //   expect(routerSpy.navigate).toHaveBeenCalledWith(['/task/9']);
  // });
});
