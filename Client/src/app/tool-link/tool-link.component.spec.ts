import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ToolLinkComponent } from './tool-link.component';

describe('ToolLinkComponent', () => {
  let component: ToolLinkComponent;
  let fixture: ComponentFixture<ToolLinkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ToolLinkComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ToolLinkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
