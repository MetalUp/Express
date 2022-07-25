import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FunctionDefinitionComponent } from './function-definition.component';

describe('FunctionDefinitionComponent', () => {
  let component: FunctionDefinitionComponent;
  let fixture: ComponentFixture<FunctionDefinitionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FunctionDefinitionComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FunctionDefinitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
