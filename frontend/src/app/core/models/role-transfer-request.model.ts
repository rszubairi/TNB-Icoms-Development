export interface RoleTransferRequest {
  id: string;
  tnbId: string;
  fullName: string;
  currentRoleName: string;
  requestedRoleName: string;
  requestedAt: string;
  status: 'Pending' | 'Approved' | 'Rejected';
}
