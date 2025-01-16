import { Component, EventEmitter, inject, input, Input, OnInit, Output, output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { NgIf } from '@angular/common';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { DatesPickerComponent } from "../_forms/dates-picker/dates-picker.component";
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, TextInputComponent, DatesPickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {

  // @Input() userFormHomeComponent:any; 
  // userFormHomeComponent = input.required<any>();
  // @Output() cancelRegister = new EventEmitter();
  cancelRegister = output<boolean>();
  private accountService = inject(AccountService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private toastr = inject(ToastrService);
  registerForm: FormGroup = new FormGroup({});
  maxDate = new Date;
  validationErrors: string[] | undefined;



  ngOnInit(): void {
    this.initializeForm();
    this.MaxDate();
  }

  macthingObg(matchingto: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchingto)?.value ? null : { isMatching: true }
    }
  }

  register() {
   const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
   this.registerForm.patchValue({dateOfBirth:dob })
    console.log(this.registerForm.value)
    this.accountService.Register(this.registerForm.value).subscribe(
      {
        next: _ => this.router.navigateByUrl("/members"),
        error: error => {
          this.validationErrors = error
        }
      }
    )
  }

  cancel(): void {
    this.cancelRegister.emit(false)
  }
  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4)
        , Validators.maxLength(8)]],
      confirmpassword: ['', [Validators.required,
      this.macthingObg('password')]] // we need a custom validator
    });
    // updating the password wouuld update the validity 
    this.registerForm.controls['confirmpassword'].valueChanges.subscribe(
      {
        next: () => this.registerForm.controls['password'].updateValueAndValidity()
      }
    )
  }

  MaxDate() {
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18)
  }

  private getDateOnly(dob:string | undefined){
    if(!dob) return ;
    return new Date(dob).toISOString().slice(0,10);
  }
}
