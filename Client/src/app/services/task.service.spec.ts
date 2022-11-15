import { fakeAsync, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { TaskService } from './task.service';
import { first } from 'rxjs';
import { ConfigService, ContextService, RepLoaderService } from '@nakedobjects/services';
import { CollectionMember, DomainObjectRepresentation, EntryType, PropertyMember } from '@nakedobjects/restful-objects';

describe('TaskService', () => {
  let service: TaskService;
  let routerSpy: jasmine.SpyObj<Router>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    contextServiceSpy = jasmine.createSpyObj('ConfigService', [], { config: { appPath: 'testPath' } });
    repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['populate', 'getFile'])

    TestBed.configureTestingModule({});
    service = new TaskService(routerSpy, contextServiceSpy, repLoaderSpy)
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  // it('should get the task', fakeAsync(() => {
   
  //   const object = new DomainObjectRepresentation();
  //   object.hateoasUrl = `testPath/objects/Model.Types.Task/testTask`;
  //   const pm = new PropertyMember({value : 'testlanguage'} as any, object, 'Language');
  //   const cm = new CollectionMember({value : []} as any, object, 'Hints');
  //   pm.entryType = () => EntryType.FreeForm;
  //   pm.isScalar = () => true;

  //   object.propertyMembers = () => ({'Language': pm});
  //   object.collectionMembers = () => ({'Hints': cm});
    
  //   const promise = Promise.resolve(object);

  //   repLoaderSpy.populate.and.returnValue(promise);

  //   service.loadTask('testTask');
    
  //   expect(repLoaderSpy.populate).toHaveBeenCalledWith(jasmine.objectContaining({hateoasUrl: `testPath/objects/Model.Types.Task/testTask`}), true);
    
  //   service.currentTask.pipe(first()).subscribe(t => {
  //       expect(t.Language).toEqual('testlanguage');
  //       expect(t.Hints.length).toEqual(0);
  //     }
  //   );
    
   
  // }));

  // it('should load empty task if task not found', fakeAsync(() => {

  //   repLoaderSpy.populate.and.returnValue(Promise.reject({ status: 404 }));

  //   service.loadTask('testTask');

  //   expect(repLoaderSpy.populate).toHaveBeenCalledWith(jasmine.objectContaining({ hateoasUrl: `testPath/objects/Model.Types.Task/testTask` }), true);
  //   service.currentTask.pipe(first()).subscribe(t =>
  //     expect(t.Language).toEqual('')
  //   );
  // }));

  it('should get the html file for the task', () => {
   
    repLoaderSpy.getFile.and.returnValue(Promise.resolve(new Blob(['task description'])));
    service.getFile(['testUrl', 'testMt']).then(t => expect(t).toBe('task description'));
    expect(repLoaderSpy.getFile).toHaveBeenCalledWith('testUrl', 'testMt', true);
  });

  it('should handle error when get the html file for the task', () => {
   
    repLoaderSpy.getFile.and.returnValue(Promise.reject());
    service.getFile(['testUrl', 'testMt']).then(t => expect(t).toBe(''));
    expect(repLoaderSpy.getFile).toHaveBeenCalledWith('testUrl', 'testMt', true);
  });

  it('should goto a new task', () => {
    service.currentTask.pipe(first()).subscribe(t =>
      expect(t.Language).toEqual('testlanguage')
    );

    service.gotoTask(9, 0);

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/task/9-0']);
  });
});
