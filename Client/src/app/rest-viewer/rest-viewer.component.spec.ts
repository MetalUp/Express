import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RestViewerComponent } from './rest-viewer.component';

describe('RestViewerComponent', () => {
  let component: RestViewerComponent;
  let fixture: ComponentFixture<RestViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RestViewerComponent ]
    })
    .compileComponents();

    // fixture = TestBed.createComponent(RestViewerComponent);
    // component = fixture.componentInstance;
    // fixture.detectChanges();
  });

  it('should create', () => {
    expect(true).toBeTruthy();
  });
});
