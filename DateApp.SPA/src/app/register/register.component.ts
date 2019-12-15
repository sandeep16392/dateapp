import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { UserForRegistration } from '../models/UserForRegistration';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder
} from '@angular/forms';
import { CustomValidators } from '../validators/custom-validators';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../models/User';
import { Route, Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  bsConfig: Partial<BsDatepickerConfig>;
  user: User;
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  customValidator = new CustomValidators();

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    };
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male'],
        username: [
          '',
          [
            Validators.required,
            Validators.minLength(7),
            Validators.maxLength(15)
          ]
        ],
        knownAs: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(7),
            Validators.maxLength(15)
          ]
        ],
        confirmPassword: ['', Validators.required]
      },
      {
        validator: this.customValidator.passwordMatchCheck
      }
    );
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(
        () => {
          this.alertify.success('Registration Successful!');
        },
        error => {
          this.alertify.error(error);
        },
        () => {
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/members']);
          });
        }
      );
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
