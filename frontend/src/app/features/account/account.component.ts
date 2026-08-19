import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account.service';
import { RoleTransferRequestService } from '../../core/services/role-transfer-request.service';
import { RoleService } from '../../core/services/role.service';
import { ZoneService } from '../../core/services/zone.service';
import { AccountProfile } from '../../core/models/account.model';
import { Role } from '../../core/models/role.model';
import { Zone } from '../../core/models/zone.model';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css'
})
export class AccountComponent {
  private accountService = inject(AccountService);
  private roleTransferService = inject(RoleTransferRequestService);
  private roleService = inject(RoleService);
  private zoneService = inject(ZoneService);

  profile = signal<AccountProfile | null>(null);
  loading = signal(true);
  errorMessage = signal<string | null>(null);

  // Profile edit
  editingProfile = signal(false);
  fullName = signal('');
  email = signal('');
  phoneNumber = signal('');
  profileError = signal<string | null>(null);
  savingProfile = signal(false);
  profileSaved = signal(false);

  // Role/Zone change request
  showTransferForm = signal(false);
  roles = signal<Role[]>([]);
  zones = signal<Zone[]>([]);
  requestedRoleId = signal<number | null>(null);
  requestedZoneId = signal<number | null>(null);
  transferReason = signal('');
  transferError = signal<string | null>(null);
  transferSaving = signal(false);
  transferSubmitted = signal(false);

  // Password change
  showPasswordForm = signal(false);
  currentPassword = signal('');
  newPassword = signal('');
  confirmPassword = signal('');
  passwordError = signal<string | null>(null);
  passwordSaving = signal(false);
  passwordChanged = signal(false);

  constructor() {
    this.load();
    this.roleService.list().subscribe({ next: (roles) => this.roles.set(roles), error: () => {} });
    this.zoneService.list().subscribe({ next: (zones) => this.zones.set(zones), error: () => {} });
  }

  private load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.accountService.getMe().subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.fullName.set(profile.fullName);
        this.email.set(profile.email);
        this.phoneNumber.set(profile.phoneNumber ?? '');
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load your account. The backend API may not be running yet.');
        this.loading.set(false);
      }
    });
  }

  // --- Profile ---

  startEditProfile(): void {
    const profile = this.profile();
    if (!profile) return;
    this.fullName.set(profile.fullName);
    this.email.set(profile.email);
    this.phoneNumber.set(profile.phoneNumber ?? '');
    this.profileError.set(null);
    this.profileSaved.set(false);
    this.editingProfile.set(true);
  }

  cancelEditProfile(): void {
    this.editingProfile.set(false);
    this.profileError.set(null);
  }

  saveProfile(): void {
    if (!this.fullName().trim() || !this.email().trim()) {
      this.profileError.set('Name and email are required.');
      return;
    }

    this.profileError.set(null);
    this.savingProfile.set(true);

    this.accountService
      .updateMe({
        fullName: this.fullName().trim(),
        email: this.email().trim(),
        phoneNumber: this.phoneNumber().trim() || null
      })
      .subscribe({
        next: (profile) => {
          this.savingProfile.set(false);
          this.profile.set(profile);
          this.editingProfile.set(false);
          this.profileSaved.set(true);
        },
        error: (err) => {
          this.savingProfile.set(false);
          this.profileError.set(err?.error?.error ?? err?.message ?? 'Unable to save changes.');
        }
      });
  }

  // --- Role/Zone transfer request ---

  openTransferForm(): void {
    this.requestedRoleId.set(null);
    this.requestedZoneId.set(null);
    this.transferReason.set('');
    this.transferError.set(null);
    this.transferSubmitted.set(false);
    this.showTransferForm.set(true);
  }

  closeTransferForm(): void {
    this.showTransferForm.set(false);
    this.transferError.set(null);
  }

  submitTransferRequest(): void {
    if (!this.requestedRoleId() && !this.requestedZoneId()) {
      this.transferError.set('Select a new role, a new zone, or both.');
      return;
    }
    if (!this.transferReason().trim()) {
      this.transferError.set('A request summary is required.');
      return;
    }

    this.transferError.set(null);
    this.transferSaving.set(true);

    this.roleTransferService
      .create({
        requestedRoleId: this.requestedRoleId(),
        requestedZoneId: this.requestedZoneId(),
        reason: this.transferReason().trim()
      })
      .subscribe({
        next: () => {
          this.transferSaving.set(false);
          this.showTransferForm.set(false);
          this.transferSubmitted.set(true);
        },
        error: (err) => {
          this.transferSaving.set(false);
          this.transferError.set(err?.error?.error ?? err?.message ?? 'Unable to submit request.');
        }
      });
  }

  // --- Password change ---

  openPasswordForm(): void {
    this.currentPassword.set('');
    this.newPassword.set('');
    this.confirmPassword.set('');
    this.passwordError.set(null);
    this.passwordChanged.set(false);
    this.showPasswordForm.set(true);
  }

  closePasswordForm(): void {
    this.showPasswordForm.set(false);
    this.passwordError.set(null);
  }

  submitPasswordChange(): void {
    if (!this.currentPassword() || !this.newPassword() || !this.confirmPassword()) {
      this.passwordError.set('All fields are required.');
      return;
    }
    if (this.newPassword() !== this.confirmPassword()) {
      this.passwordError.set('New password and confirmation do not match.');
      return;
    }
    if (this.newPassword().length < 8) {
      this.passwordError.set('New password must be at least 8 characters.');
      return;
    }

    this.passwordError.set(null);
    this.passwordSaving.set(true);

    this.accountService
      .changePassword({
        currentPassword: this.currentPassword(),
        newPassword: this.newPassword(),
        confirmPassword: this.confirmPassword()
      })
      .subscribe({
        next: () => {
          this.passwordSaving.set(false);
          this.showPasswordForm.set(false);
          this.passwordChanged.set(true);
        },
        error: (err) => {
          this.passwordSaving.set(false);
          this.passwordError.set(err?.error?.error ?? err?.message ?? 'Unable to change password.');
        }
      });
  }
}
