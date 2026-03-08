import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { Auth } from '../../services/auth/auth';

@Component({
  selector: 'app-dashboard',
  imports: [MatButtonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {

  welcomeMessage = signal('');
  showUnauthModal = signal(false);

  constructor(private authService: Auth, private router: Router) {
    this.onInit();
  }

  onInit() {
    this.authService.welcome().subscribe({
      next: (value) => {
        console.log('User is authenticated, Value from welcome:', value);
        this.welcomeMessage.set(value?.message);
        console.log('Welcome message set to:', this.welcomeMessage);
      },
      error: (err) => {
        this.showUnauthModal.set(true);
      },
    });
  }

  goToLogin(): void {
    this.showUnauthModal.set(false);
    this.router.navigate(['/login']);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
