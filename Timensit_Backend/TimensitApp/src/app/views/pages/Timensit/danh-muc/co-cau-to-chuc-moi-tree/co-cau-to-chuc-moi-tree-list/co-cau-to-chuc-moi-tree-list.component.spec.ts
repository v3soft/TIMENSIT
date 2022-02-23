import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { cocautochucmoitreeComponent } from './co-cau-to-chuc-moi-tree-list.component';

describe('cocautochucmoitreeComponent', () => {
  let component: cocautochucmoitreeComponent;
  let fixture: ComponentFixture<cocautochucmoitreeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ cocautochucmoitreeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(cocautochucmoitreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
