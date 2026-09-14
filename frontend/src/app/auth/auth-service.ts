import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private api = 'https://localhost:7163/api/users';
    
    constructor(private http: HttpClient) { }

    login(username: string, password: string) {
        return this.http.post(`${this.api}/login`, {
            username,
            password
        });
    }

    signup(username: string, password: string) {
        return this.http.post(`${this.api}/signup`, {
            username,
            password
        });
    }

    saveToken(token: string) {
        localStorage.setItem('token', token);
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }

    logout() {
        localStorage.removeItem('token');
    }

    isLoggedIn(): boolean {
        return this.getToken() !== null;
    }

    getUsername(): string | null {
        const token = this.getToken()
        
        if (!token) {
            console.log('No token found');
            return null;
        }

        try {
            const payload = JSON.parse(atob(token.split('.')[1]));

            console.log('Decoded payload:', payload);

            return payload.username ?? null;
        } catch(error) {
            console.error('JWT decode error:', error);
            return null;
        }
    }
}
