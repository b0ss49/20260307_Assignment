import { Component, signal } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth/auth';

@Component({
  selector: 'app-register-page',
  imports: [MatButtonModule, MatFormFieldModule, MatInputModule, MatIconModule,ReactiveFormsModule],
  templateUrl: './register-page.html',
  styleUrl: './register-page.css',
})
export class RegisterPage {

  registerForm: FormGroup
  showErrorModal = signal(false);
  errorMessage = signal('');
  showSuccessModal = signal(false);

  constructor(private router: Router, private authService: Auth) {
    this.registerForm = new FormGroup({
      username: new FormControl(''),
      password: new FormControl(''),
      confirmPassword : new FormControl('')
    })
  }

  onSubmit() {
    const { password, confirmPassword } = this.registerForm.value;
    if (password !== confirmPassword) {
      this.errorMessage.set('Passwords do not match.');
      this.showErrorModal.set(true);
      return;
    }
    this.authService.register(this.registerForm.value).subscribe({
      next: () => {
        console.log('Registration successful');
        this.openSuccessModal();
      },
      error: (err) => {
        this.errorMessage.set(err?.error?.message ?? 'Registration failed. Please try again.');
        this.showErrorModal.set(true);
      },
    });
  }

  openSuccessModal() {
    this.showSuccessModal.set(true);
    console.log('Success modal opened');
    console.log('Current state of showSuccessModal:', this.showSuccessModal());
  }

  closeErrorModal() {
    this.showErrorModal.set(false);
  }

  closeSuccessModal() {
    this.showSuccessModal.set(false);
    this.router.navigate(['/login']);
  }

  backToLogin()
  {
    this.router.navigate(['/login']);
  }
}
