import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';


import { BsDatepickerConfig, BsDatepickerModule } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-dates-picker',
  standalone: true,
  imports: [BsDatepickerModule,NgIf,ReactiveFormsModule],
  templateUrl: './dates-picker.component.html',
  styleUrl: './dates-picker.component.css'
})
export class DatesPickerComponent implements ControlValueAccessor {

  label = input<string>('');
  maxDate =input<Date>();
  bsConfig?:Partial<BsDatepickerConfig>;


  /**
   *
   */
  constructor(@Self() public ngControl:NgControl ) {
    this.ngControl.valueAccessor = this ;
    this.bsConfig = {
      containerClass:'theme-red',
      dateInputFormat:'DD MMMM YYYY'
    }
  }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void {
  }
// to prevent any erro of using form control in our templates
get control():FormControl {
  return this.ngControl.control as FormControl
}

}
