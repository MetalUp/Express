import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ArmliteComponent } from './armlite.component';

describe('ArmliteComponent', () => {
  let component: ArmliteComponent;
  let fixture: ComponentFixture<ArmliteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ArmliteComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ArmliteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
