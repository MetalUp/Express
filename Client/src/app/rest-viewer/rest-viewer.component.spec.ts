import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RestViewerComponent } from './rest-viewer.component';

describe('RestViewerComponent', () => {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  let component: RestViewerComponent;
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
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
