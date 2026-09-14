import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../auth-service';

@Component({
  imports: [RouterLink, FormsModule],
  selector: 'app-signup',
  styleUrl: './signup.css',
  templateUrl: './signup.html',
})
export class Signup {
  username = '';
  password = '';

  errorMessage = signal('');
  infoMessage = signal('');

  constructor(private authService: AuthService) {}

  signup() {
    if (!this.username || !this.password) {
      this.errorMessage.set('Username and password are required.');
      return;
    }

    this.authService.signup(
      this.username,
      this.password
    ).subscribe({
      next: () => {
        this.errorMessage.set('');
        this.infoMessage.set('Signup successful! Please log in.');
      },

      error: (error) => {
        this.infoMessage.set('');
        console.error('Signup failed:', error);

        if (error.status === 409) {
          this.errorMessage.set(
            error.error?.detail ??
            'Username already exists. -_-'
          );
        } else {
          this.errorMessage.set(
            error.error?.detail ??
            'Signup failed. Please try again.'
          );
        }
      }
    });
  }
}
