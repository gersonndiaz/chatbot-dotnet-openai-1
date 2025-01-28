import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModelsAvailableComponent } from './models-available.component';

describe('ModelsAvailableComponent', () => {
  let component: ModelsAvailableComponent;
  let fixture: ComponentFixture<ModelsAvailableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModelsAvailableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ModelsAvailableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
