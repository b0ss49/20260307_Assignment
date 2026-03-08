import { Component, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import {MatButtonModule} from '@angular/material/button';
import { Auth } from '../../services/auth/auth';

@Component({
  selector: 'app-login-page',
  imports: [MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule,ReactiveFormsModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage {

  loginForm : FormGroup
  showSuccessModal = signal(false);
  showErrorModal = signal(false);
  errorMessage = signal('');

  constructor(private router: Router, private authService: Auth) {
    this.loginForm = new FormGroup({
      username : new FormControl(''),
      password : new FormControl('')
    })

    this.onInit();
  }

  onInit() {
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    const { username, password } = this.loginForm.value;
    this.authService.login({ username, password }).subscribe({
      next: () => this.showSuccessModal.set(true),
      error: (err) => {
        this.errorMessage.set(err?.error?.message ?? 'Login failed. Please try again.');
        this.showErrorModal.set(true);
      },
    });
  }

  closeSuccessModal() {
    this.showSuccessModal.set(false);
    this.router.navigate(['/dashboard']);
  }

  closeErrorModal() {
    this.showErrorModal.set(false);
  }

  onRegister() {
    this.router.navigate(['/register']);
  }
}
