import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpressionEvaluationComponent } from './expression-evaluation.component';

describe('ExpressionEvaluationComponent', () => {
  let component: ExpressionEvaluationComponent;
  let fixture: ComponentFixture<ExpressionEvaluationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExpressionEvaluationComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExpressionEvaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
