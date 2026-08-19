export interface RoleTransferRequest {
  id: number;
  tnbId: string;
  fullName: string;
  currentRoleName: string | null;
  requestedRoleName: string | null;
  currentZoneName: string | null;
  requestedZoneName: string | null;
  reason: string | null;
  requestedAt: string;
  status: 'Pending' | 'Approved' | 'Rejected';
  rejectionReason: string | null;
}

export interface CreateRoleTransferRequest {
  requestedRoleId: number | null;
  requestedZoneId: number | null;
  reason: string;
}
