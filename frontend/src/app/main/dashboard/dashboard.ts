import { Component } from '@angular/core';
import { AuthService } from '../../auth/auth-service';

@Component({
  selector: 'app-dashboard',
  styleUrl: './dashboard.css',
  templateUrl: './dashboard.html',
})
export class Dashboard {
  username: string | null = null;

  constructor(private AuthService: AuthService) {
    this.username = this.AuthService.getUsername();
  }
}
