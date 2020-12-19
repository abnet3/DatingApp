import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { error } from 'protractor';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {


  @Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();

  // model: any = {};

  registerForm!: FormGroup;

  maxDate!: Date;

  validationErrors:string[] = [];
  

  constructor(private accountService: AccountService, private toastr: 
    ToastrService, private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {

     this.initializeForm();
     this.maxDate = new Date();
     this.maxDate.setFullYear(this.maxDate.getFullYear()-18);

  }

  initializeForm(){

    // this.registerForm = new FormGroup({
    //   username: new FormControl('hello', Validators.required),
    //   password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //   confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')]),

    // })
// using Form builder
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', [Validators.required]],
      knownAs: ['', [Validators.required]],
      dateOfBirth: ['', [Validators.required]],
      city: ['', [Validators.required]],
      country: ['', [Validators.required]],
      password: [ '', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],

    })
  }
  
  
  // matchValues(matchTo: string): ValidatorFn {

  //          return (control: AbstractControl) => {
  //            // @ts-ignore
  //           return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching: true}
          
  //          }
  // }

  matchValues(matchTo: string): ValidatorFn {

    return (control: AbstractControl) => {
      // console.log("==================", control);
      // return null;
      if (control.parent && control.parent.controls) {
        return control.value ===
          (control.parent.controls as { [key: string]: AbstractControl })[
            matchTo
          ].value
          ? null
          : { isMatching: true }
      }

      return null;
    };
  }
    register(){ 

      console.log(this.registerForm.value);

      // this.accountService.register(this.model).subscribe(response => {\
      this.accountService.register(this.registerForm.value).subscribe(response => {

               this.router.navigateByUrl('/members');
              // this.cancel();

      }, error =>{
        console.log(error);   

        this.validationErrors = error;



      })

    }

     cancel(){
        
       this.cancelRegister.emit(false);

    }

}
