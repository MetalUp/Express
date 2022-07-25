import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectedLanguageComponent } from './selected-language.component';

describe('SelectedLanguageComponent', () => {
  let component: SelectedLanguageComponent;
  let fixture: ComponentFixture<SelectedLanguageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelectedLanguageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SelectedLanguageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
