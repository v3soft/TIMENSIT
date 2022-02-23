import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { SodotochucListComponent } from './so-do-to-chuc-new-list.component';



describe('SodotochucListComponent', () => {
  let component: SodotochucListComponent;
  let fixture: ComponentFixture<SodotochucListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SodotochucListComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SodotochucListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
