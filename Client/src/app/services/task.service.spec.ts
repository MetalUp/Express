import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { TaskService } from './task.service';
import { of } from 'rxjs';
import { ITask } from './task';
import { first, throwError } from 'rxjs';
import { ConfigService, RepLoaderService } from '@nakedobjects/services';
import { DomainObjectRepresentation, EntryType, IHateoasModel, PropertyMember } from '@nakedobjects/restful-objects';

describe('TaskService', () => {
  let service: TaskService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;
  let routerSpy: jasmine.SpyObj<Router>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;
  let configServiceSpy: jasmine.SpyObj<ConfigService>;

  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get', 'post']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    configServiceSpy = jasmine.createSpyObj('ConfigService', [], { config: { appPath: 'testPath' } });
    repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['populate'])


    httpClientSpy.get.and.returnValue(of({ Language: 'testlanguage' } as unknown as ITask))

    TestBed.configureTestingModule({});
    service = new TaskService(httpClientSpy, routerSpy, configServiceSpy, repLoaderSpy)
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get the task', () => {
    service.currentTask.pipe(first()).subscribe(t =>
      expect(t.Language).toEqual('testlanguage')
    );

    const object = new DomainObjectRepresentation();
    object.hateoasUrl = `testPath/objects/Model.Types.Task/testTask`;
    const pm = new PropertyMember({value : 'testlanguage'} as any, object, 'language');
    pm.entryType = () => EntryType.FreeForm;
    pm.isScalar = () => true;

    object.propertyMembers = () => ({'language': pm});
    
    const promise = Promise.resolve(object);

    repLoaderSpy.populate.and.returnValue(promise);

    service.loadTask('testTask');

    expect(repLoaderSpy.populate).toHaveBeenCalledWith(jasmine.objectContaining({hateoasUrl: `testPath/objects/Model.Types.Task/testTask`}), true);
  });

  it('should load empty task if task not found', () => {
    service.currentTask.pipe(first()).subscribe(t =>
      expect(t.Language).toEqual('')
    );
    
    repLoaderSpy.populate.and.returnValue(Promise.reject({ status: 404 }));

    service.loadTask('testTask');

    expect(repLoaderSpy.populate).toHaveBeenCalledWith(jasmine.objectContaining({hateoasUrl: `testPath/objects/Model.Types.Task/testTask`}), true);
  });

  it('should get the html file for the task', () => {

    service.getHtml('testHtmlFile.html');

    const parms = { withCredentials: true, responseType: 'text' as const };

    expect(httpClientSpy.get).toHaveBeenCalledWith('content/testHtmlFile.html', parms as any);
  });

  it('should goto a new task', () => {
    service.currentTask.pipe(first()).subscribe(t =>
      expect(t.Language).toEqual('testlanguage')
    );

    service.gotoTask('task/newTask');

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/task/newTask']);
  });
});
