import { Injectable, inject, signal } from '@angular/core';
import { Observable, catchError, finalize, from, map, mergeMap, of, tap, timeout } from 'rxjs';
import { AccountService } from './account.service';
import { AuthorisationPersonnelService } from './authorisation-personnel.service';
import { ChangeRequestService } from './change-request.service';
import { ChangeRequestSettingsService } from './change-request-settings.service';
import { CommissioningMemoService } from './commissioning-memo.service';
import { ConflictingLineService } from './conflicting-line.service';
import { DropdownValueService } from './dropdown-value.service';
import { EmailLogService } from './email-log.service';
import { EmailTemplateService } from './email-template.service';
import { EquipmentService } from './equipment.service';
import { EquipmentTypeService } from './equipment-type.service';
import { ErrorLogService } from './error-log.service';
import { GncService } from './gnc.service';
import { HandoverService } from './handover.service';
import { LinkingLineService } from './linking-line.service';
import { MnemonicService } from './mnemonic.service';
import { OrganisationService } from './organisation.service';
import { OutageScheduleWindowService } from './outage-schedule-window.service';
import { OutageService } from './outage.service';
import { OutageTypeRuleService } from './outage-type-rule.service';
import { ProjectService } from './project.service';
import { ReportService } from './report.service';
import { RoleService } from './role.service';
import { RoleTransferRequestService } from './role-transfer-request.service';
import { SldService } from './sld.service';
import { StationService } from './station.service';
import { StatisticsService } from './statistics.service';
import { TransmissionLineService } from './transmission-line.service';
import { UserService } from './user.service';
import { VoltageLevelService } from './voltage-level.service';
import { ZoneService } from './zone.service';

export type HealthCheckStatus = 'pending' | 'running' | 'passed' | 'failed';

export interface HealthCheckResult {
  id: string;
  group: string;
  label: string;
  status: HealthCheckStatus;
  durationMs?: number;
  error?: string;
}

interface HealthCheckDefinition {
  id: string;
  group: string;
  label: string;
  run: () => Observable<unknown>;
}

const CHECK_TIMEOUT_MS = 10000;
const CONCURRENCY = 4;

@Injectable({ providedIn: 'root' })
export class HealthCheckService {
  private userService = inject(UserService);
  private roleService = inject(RoleService);
  private roleTransferRequestService = inject(RoleTransferRequestService);
  private organisationService = inject(OrganisationService);
  private stationService = inject(StationService);
  private voltageLevelService = inject(VoltageLevelService);
  private equipmentTypeService = inject(EquipmentTypeService);
  private equipmentService = inject(EquipmentService);
  private dropdownValueService = inject(DropdownValueService);
  private transmissionLineService = inject(TransmissionLineService);
  private conflictingLineService = inject(ConflictingLineService);
  private linkingLineService = inject(LinkingLineService);
  private mnemonicService = inject(MnemonicService);
  private zoneService = inject(ZoneService);
  private outageService = inject(OutageService);
  private changeRequestService = inject(ChangeRequestService);
  private projectService = inject(ProjectService);
  private outageTypeRuleService = inject(OutageTypeRuleService);
  private outageScheduleWindowService = inject(OutageScheduleWindowService);
  private authorisationPersonnelService = inject(AuthorisationPersonnelService);
  private changeRequestSettingsService = inject(ChangeRequestSettingsService);
  private gncService = inject(GncService);
  private statisticsService = inject(StatisticsService);
  private reportService = inject(ReportService);
  private handoverService = inject(HandoverService);
  private sldService = inject(SldService);
  private commissioningMemoService = inject(CommissioningMemoService);
  private errorLogService = inject(ErrorLogService);
  private emailLogService = inject(EmailLogService);
  private emailTemplateService = inject(EmailTemplateService);
  private accountService = inject(AccountService);

  results = signal<HealthCheckResult[]>([]);
  running = signal(false);

  private buildDefinitions(): HealthCheckDefinition[] {
    return [
      { id: 'account-session', group: 'Authentication', label: 'Session / Account Profile', run: () => this.accountService.getMe() },

      { id: 'users', group: 'Administration', label: 'User Management', run: () => this.userService.list() },
      { id: 'roles', group: 'Administration', label: 'Roles & Permissions', run: () => this.roleService.list() },
      { id: 'role-transfer-requests', group: 'Administration', label: 'Role Transfer Requests', run: () => this.roleTransferRequestService.list() },
      { id: 'organisations', group: 'Administration', label: 'Organisations & Stations', run: () => this.organisationService.list() },
      { id: 'stations', group: 'Administration', label: 'Stations', run: () => this.stationService.list() },

      { id: 'voltage-levels', group: 'Asset Configuration', label: 'Voltage Levels', run: () => this.voltageLevelService.list() },
      { id: 'equipment-types', group: 'Asset Configuration', label: 'Equipment Types', run: () => this.equipmentTypeService.list() },
      { id: 'equipment', group: 'Asset Configuration', label: 'Equipment Directory', run: () => this.equipmentService.list() },
      { id: 'off-points', group: 'Asset Configuration', label: 'Off-Point Management', run: () => this.equipmentService.list({ isOffPoint: true }) },
      { id: 'dropdown-values', group: 'Asset Configuration', label: 'Dropdown Management', run: () => this.dropdownValueService.listCategories() },
      { id: 'transmission-lines', group: 'Asset Configuration', label: 'Transmission Lines', run: () => this.transmissionLineService.list() },
      { id: 'conflicting-lines', group: 'Asset Configuration', label: 'Conflicting Lines', run: () => this.conflictingLineService.list() },
      { id: 'linking-lines', group: 'Asset Configuration', label: 'Linking Lines', run: () => this.linkingLineService.list() },
      { id: 'mnemonic', group: 'Asset Configuration', label: 'Mnemonic List', run: () => this.mnemonicService.list() },

      { id: 'zones', group: 'Outage Management', label: 'Zones', run: () => this.zoneService.list() },
      { id: 'outages', group: 'Outage Management', label: 'Outage Data Repository', run: () => this.outageService.list() },
      { id: 'change-requests-pending', group: 'Outage Management', label: 'Change Request Review', run: () => this.changeRequestService.listPending() },
      { id: 'projects', group: 'Outage Management', label: 'Project Management', run: () => this.projectService.list() },
      { id: 'outage-type-rules', group: 'Outage Management', label: 'Outage Type Configuration', run: () => this.outageTypeRuleService.list() },
      { id: 'outage-schedule-windows', group: 'Outage Management', label: 'Outage Scheduling', run: () => this.outageScheduleWindowService.list() },
      { id: 'authorisation-personnel', group: 'Outage Management', label: 'Authorisation Personnel', run: () => this.authorisationPersonnelService.list() },
      { id: 'change-request-settings', group: 'Outage Management', label: 'Change Request Settings', run: () => this.changeRequestSettingsService.get() },

      { id: 'gnc-scheduled', group: 'Grid Network Control', label: 'GNC: Scheduled Outage', run: () => this.gncService.listScheduled() },
      { id: 'gnc-active', group: 'Grid Network Control', label: 'GNC: Active Outages', run: () => this.gncService.listActive() },
      { id: 'gnc-authorisation-in-force', group: 'Grid Network Control', label: 'GNC: Authorisation in Force', run: () => this.gncService.listAuthorisationInForce() },

      { id: 'statistics', group: 'Reports & Analytics', label: 'Statistics Dashboard', run: () => this.statisticsService.getDashboard(new Date().getFullYear(), null) },
      { id: 'reports-favourites', group: 'Reports & Analytics', label: 'Customised Reporting', run: () => this.reportService.listFavourites() },

      { id: 'handover-categories', group: 'Operations Tools', label: 'Shift Handover', run: () => this.handoverService.listCategories() },
      { id: 'sld', group: 'Operations Tools', label: 'Single Line Diagrams', run: () => this.sldService.list() },
      { id: 'commissioning-memos', group: 'Operations Tools', label: 'Commissioning Memos', run: () => this.commissioningMemoService.list() },

      { id: 'error-logs', group: 'System Logs', label: 'Error Logs', run: () => this.errorLogService.list() },
      { id: 'email-logs', group: 'System Logs', label: 'Email Logs', run: () => this.emailLogService.list() },
      { id: 'email-templates', group: 'System Logs', label: 'Email Templates', run: () => this.emailTemplateService.list() }
    ];
  }

  runAll(): Observable<HealthCheckResult[]> {
    const definitions = this.buildDefinitions();
    this.running.set(true);
    this.results.set(
      definitions.map((d) => ({ id: d.id, group: d.group, label: d.label, status: 'pending' as HealthCheckStatus }))
    );

    return from(definitions).pipe(
      mergeMap((definition) => this.runOne(definition), CONCURRENCY),
      finalize(() => this.running.set(false)),
      map(() => this.results())
    );
  }

  private runOne(definition: HealthCheckDefinition): Observable<HealthCheckResult> {
    this.patchResult(definition.id, { status: 'running' });
    const startedAt = performance.now();

    return definition.run().pipe(
      timeout(CHECK_TIMEOUT_MS),
      map((): HealthCheckResult => ({
        id: definition.id,
        group: definition.group,
        label: definition.label,
        status: 'passed',
        durationMs: Math.round(performance.now() - startedAt)
      })),
      catchError((err) =>
        of<HealthCheckResult>({
          id: definition.id,
          group: definition.group,
          label: definition.label,
          status: 'failed',
          durationMs: Math.round(performance.now() - startedAt),
          error: this.describeError(err)
        })
      ),
      tap((result) => this.patchResult(definition.id, result))
    );
  }

  private describeError(err: unknown): string {
    if (err instanceof Error) {
      if (err.name === 'TimeoutError') {
        return `No response within ${CHECK_TIMEOUT_MS / 1000}s`;
      }
      return err.message;
    }
    if (typeof err === 'object' && err !== null) {
      const httpErr = err as { status?: number; statusText?: string; error?: { error?: string } };
      if (httpErr.status !== undefined) {
        const detail = httpErr.error?.error;
        return `HTTP ${httpErr.status}${httpErr.statusText ? ' ' + httpErr.statusText : ''}${detail ? ' — ' + detail : ''}`;
      }
    }
    return 'Unknown error';
  }

  private patchResult(id: string, patch: Partial<HealthCheckResult>): void {
    this.results.update((current) => current.map((r) => (r.id === id ? { ...r, ...patch } : r)));
  }
}
