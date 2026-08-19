import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RoleTransferRequestService } from '../../../../core/services/role-transfer-request.service';
import { RoleTransferRequest } from '../../../../core/models/role-transfer-request.model';

@Component({
  selector: 'app-role-transfer-requests',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './role-transfer-requests.component.html',
  styleUrl: './role-transfer-requests.component.css'
})
export class RoleTransferRequestsComponent {
  private service = inject(RoleTransferRequestService);

  requests = signal<RoleTransferRequest[]>([]);
  loading = signal(true);
  notAvailable = signal(false);
  processingId = signal<number | null>(null);

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.notAvailable.set(false);
    this.service.list().subscribe({
      next: (requests) => {
        this.requests.set(requests);
        this.loading.set(false);
      },
      error: () => {
        this.notAvailable.set(true);
        this.loading.set(false);
      }
    });
  }

  approve(id: number): void {
    this.processingId.set(id);
    this.service.approve(id).subscribe({
      next: () => this.load(),
      error: () => this.processingId.set(null)
    });
  }

  reject(id: number): void {
    this.processingId.set(id);
    this.service.reject(id).subscribe({
      next: () => this.load(),
      error: () => this.processingId.set(null)
    });
  }
}
