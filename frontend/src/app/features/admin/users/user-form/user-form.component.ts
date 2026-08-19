import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { RoleService } from '../../../../core/services/role.service';
import { UserService } from '../../../../core/services/user.service';
import { ZoneService } from '../../../../core/services/zone.service';
import { Role } from '../../../../core/models/role.model';
import { Zone } from '../../../../core/models/zone.model';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.css'
})
export class UserFormComponent {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private roleService = inject(RoleService);
  private zoneService = inject(ZoneService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  roles = signal<Role[]>([]);
  zones = signal<Zone[]>([]);
  gcuStationOptions = signal<string[]>(['Station A', 'Station B', 'Station C', 'Station D']);
  loading = signal(false);
  saving = signal(false);
  errorMessage = signal<string | null>(null);

  tnbId = this.route.snapshot.paramMap.get('id') ?? 'new';
  isEditMode = this.tnbId !== 'new';

  form = this.fb.nonNullable.group({
    tnbId: [this.isEditMode ? this.tnbId : '', [Validators.required]],
    fullName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required]],
    roleId: ['', [Validators.required]],
    zoneId: ['', [Validators.required]],
    organisationId: [''],
    gcuTypeId: [''],
    gcuStationIds: this.fb.nonNullable.control<string[]>([]),
    isActive: [true]
  });

  constructor() {
    this.roleService.list().subscribe({ next: (roles) => this.roles.set(roles), error: () => {} });
    this.zoneService.list().subscribe({ next: (zones) => this.zones.set(zones), error: () => {} });

    if (this.isEditMode) {
      this.loading.set(true);
      this.userService.getById(this.tnbId).subscribe({
        next: (user) => {
          this.form.patchValue({
            tnbId: user.tnbId,
            fullName: user.fullName,
            email: user.email,
            phoneNumber: user.phoneNumber,
            roleId: user.roleId,
            zoneId: user.zoneId,
            organisationId: user.organisationId ?? '',
            gcuTypeId: user.gcuTypeId ?? '',
            gcuStationIds: user.gcuStationIds,
            isActive: user.isActive
          });
          this.form.controls.tnbId.disable();
          this.loading.set(false);
        },
        error: () => {
          this.errorMessage.set('Unable to load user details.');
          this.loading.set(false);
        }
      });
    }
  }

  toggleStation(station: string, checked: boolean): void {
    const current = this.form.controls.gcuStationIds.value;
    if (checked) {
      this.form.controls.gcuStationIds.setValue([...current, station]);
    } else {
      this.form.controls.gcuStationIds.setValue(current.filter((s) => s !== station));
    }
  }

  isStationChecked(station: string): boolean {
    return this.form.controls.gcuStationIds.value.includes(station);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.errorMessage.set(null);
    this.saving.set(true);
    const raw = this.form.getRawValue();
    const payload = {
      tnbId: raw.tnbId,
      fullName: raw.fullName,
      email: raw.email,
      phoneNumber: raw.phoneNumber,
      roleId: raw.roleId,
      zoneId: raw.zoneId,
      organisationId: raw.organisationId || null,
      gcuTypeId: raw.gcuTypeId || null,
      gcuStationIds: raw.gcuStationIds,
      isActive: raw.isActive
    };

    const request$ = this.isEditMode
      ? this.userService.update(this.tnbId, payload)
      : this.userService.create(payload);

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigateByUrl('/admin/users');
      },
      error: (err) => {
        this.saving.set(false);
        this.errorMessage.set(err?.error?.error ?? err?.message ?? 'Unable to save user.');
      }
    });
  }
}
