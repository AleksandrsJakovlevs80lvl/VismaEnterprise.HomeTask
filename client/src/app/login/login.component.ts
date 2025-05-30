import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  loginError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
        this.router.navigate(['/catalogue']);
    }
  }

  onLogin() {
    this.loginError = null;
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value)
        .subscribe({
          next: (response) => {
            if (response.status === 200) {
              this.router.navigate(['/catalogue']);
            } else {
              this.loginError = `Unexpected status code: ${response.status}`;
            }
          },
          error: (err) => {
            if (err.status === 0) {
              this.loginError = 'Could not connect to the server. Please try again later.';
            } else if (err.error && err.error.errorMessage && typeof err.error.errorMessage === 'string') {
              this.loginError = err.error.errorMessage;
            } else {
              this.loginError = 'Login failed. Please check your credentials and try again.';
            }
          }
        });
    } else {
      this.loginForm.markAllAsTouched();
    }
  }

  onRegister() {
    this.loginError = null;
    if (this.loginForm.valid) {
      this.authService.register(this.loginForm.value)
        .subscribe({
          next: (response) => {
            if (response.status === 200) {
              this.onLogin();
            } else {
              this.loginError = `Registration failed with status: ${response.status}`;
            }
          },
          error: (err) => {
            if (err.status === 0) {
              this.loginError = 'Could not connect to the server. Please try again later.';
            } else if (err.error && err.error.errorMessage && typeof err.error.errorMessage === 'string') {
              this.loginError = err.error.errorMessage;
            } else {
              this.loginError = 'Registration failed. Please try again.';
            }
          }
        });
    } else {
      this.loginForm.markAllAsTouched();
    }
  }
}
