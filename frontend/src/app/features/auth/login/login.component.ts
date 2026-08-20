import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  submitting = signal(false);
  errorMessage = signal<string | null>(null);
  adLoginEnabled = environment.adLoginEnabled;

  form = this.fb.nonNullable.group({
    identifier: ['', [Validators.required]],
    password: ['', [Validators.required]],
    rememberMe: [false]
  });

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.errorMessage.set(null);
    this.submitting.set(true);
    this.authService.login(this.form.getRawValue()).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigateByUrl('/admin/users');
      },
      error: (err) => {
        this.submitting.set(false);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Invalid credentials. Please try again.');
      }
    });
  }

  onCorporateSso(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.errorMessage.set(null);
    this.submitting.set(true);
    this.authService.loginWithAd(this.form.getRawValue()).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigateByUrl('/admin/users');
      },
      error: (err) => {
        this.submitting.set(false);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'AD sign-in failed. Please try again.');
      }
    });
  }
}
