import { Injectable } from '@angular/core';
import { ErrorWrapper } from '@nakedobjects/services';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  constructor() { }

  public static NoError = new Object();

  get currentError() {
    return this.currentErrorAsSubject;
  }

  private currentErrorAsSubject = new BehaviorSubject<ErrorWrapper | Object>(ErrorService.NoError);

  private errorHistory: ErrorWrapper[] = [];

  addError(error: ErrorWrapper) {
    this.currentErrorAsSubject.next(error);
    this.errorHistory.push(error);
  }

  clearError() {
    this.currentErrorAsSubject.next(ErrorService.NoError);
  }
}
