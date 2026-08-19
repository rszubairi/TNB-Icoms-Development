-- ============================================================================
-- TNB ICOMS 2.0 DATA MIGRATION & ETL SCRIPT
-- Source: dbOutage (Legacy MSSQL 52.74.111.85)
-- Target: ICOMS 2.0 Schema (auth, config, dbo, handover, audit)
-- ============================================================================

-- 1. Create Schemas
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'auth') EXEC('CREATE SCHEMA auth');
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'config') EXEC('CREATE SCHEMA config');
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'handover') EXEC('CREATE SCHEMA handover');
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'audit') EXEC('CREATE SCHEMA audit');
GO

-- 2. Migrate Zones from Legacy TblGridZone / tblregion_new
PRINT 'Migrating Zones...';
MERGE INTO config.Zones AS target
USING (
    SELECT 
        ID AS ZoneId,
        LTRIM(RTRIM(Zone)) AS ZoneName,
        UPPER(LEFT(LTRIM(RTRIM(Zone)), 4)) AS ZoneAbbr,
        1 AS IsActive,
        GETUTCDATE() AS CreatedAt
    FROM dbOutage.dbo.TblGridZone
    WHERE Zone IS NOT NULL AND LTRIM(RTRIM(Zone)) <> ''
) AS source
ON target.ZoneId = source.ZoneId
WHEN NOT MATCHED THEN
    INSERT (ZoneId, ZoneName, ZoneAbbr, IsActive, CreatedAt)
    VALUES (source.ZoneId, source.ZoneName, source.ZoneAbbr, source.IsActive, source.CreatedAt);
GO

-- 3. Migrate Voltage Levels from TblKV
PRINT 'Migrating Voltage Levels...';
MERGE INTO config.VoltageLevel AS target
USING (
    SELECT 
        ROW_NUMBER() OVER (ORDER BY TRY_CAST(REPLACE(REPLACE(LTRIM(RTRIM(KV)), 'kV', ''), 'KV', '') AS DECIMAL(8,2))) AS VoltageLevelId,
        LTRIM(RTRIM(KV)) AS VoltageName,
        TRY_CAST(REPLACE(REPLACE(LTRIM(RTRIM(KV)), 'kV', ''), 'KV', '') AS DECIMAL(8,2)) AS VoltageKv,
        1 AS IsActive
    FROM dbOutage.dbo.TblKV
    WHERE KV IS NOT NULL
) AS source
ON target.VoltageName = source.VoltageName
WHEN NOT MATCHED THEN
    INSERT (VoltageName, VoltageKv, DisplayOrder, IsActive)
    VALUES (source.VoltageName, ISNULL(source.VoltageKv, 0), source.VoltageLevelId, source.IsActive);
GO

-- 4. Migrate Organisations from tblorganisation_new
PRINT 'Migrating Organisations...';
MERGE INTO config.Organisations AS target
USING (
    SELECT 
        orgid AS OrgId,
        ISNULL(LTRIM(RTRIM(name)), 'N/A') AS OrgName,
        ISNULL(LTRIM(RTRIM(abbrev)), LEFT(name, 10)) AS OrgAbbr,
        1 AS ZoneId, -- Default to Zone 1 if unassigned
        0 AS IsExternal,
        1 AS IsActive,
        GETUTCDATE() AS CreatedAt
    FROM dbOutage.dbo.tblorganisation_new
) AS source
ON target.OrgAbbr = source.OrgAbbr
WHEN NOT MATCHED THEN
    INSERT (OrgName, OrgAbbr, ZoneId, IsExternal, IsActive, CreatedAt)
    VALUES (source.OrgName, source.OrgAbbr, source.ZoneId, source.IsExternal, source.IsActive, source.CreatedAt);
GO

-- 5. Migrate Stations from TblSubstation_new
PRINT 'Migrating Stations...';
MERGE INTO config.Stations AS target
USING (
    SELECT 
        LTRIM(RTRIM(MNEM)) AS StationAbbr,
        LTRIM(RTRIM(Name)) AS StationName,
        ISNULL((SELECT TOP 1 ZoneId FROM config.Zones WHERE ZoneName LIKE '%' + LTRIM(RTRIM(s.Region)) + '%'), 1) AS ZoneId,
        (SELECT TOP 1 OrgId FROM config.Organisations) AS OrgId,
        1 AS IsActive,
        GETUTCDATE() AS CreatedAt
    FROM dbOutage.dbo.TblSubstation_new s
) AS source
ON target.StationAbbr = source.StationAbbr
WHEN NOT MATCHED THEN
    INSERT (StationName, StationAbbr, ZoneId, OrgId, IsActive, CreatedAt)
    VALUES (source.StationName, source.StationAbbr, source.ZoneId, source.OrgId, source.IsActive, source.CreatedAt);
GO

-- 6. Migrate Users from TblUserProfile
PRINT 'Migrating Users...';
MERGE INTO auth.Users AS target
USING (
    SELECT 
        LTRIM(RTRIM(UserId)) AS TnbId,
        LTRIM(RTRIM(FullName)) AS FullName,
        ISNULL(LTRIM(RTRIM(Email)), LTRIM(RTRIM(UserId)) + '@tnb.com.my') AS Email,
        LTRIM(RTRIM(MobileNo)) AS PhoneNumber,
        1 AS AuthType, -- 1 = AD Internal
        1 AS RoleId,
        ISNULL((SELECT TOP 1 ZoneId FROM config.Zones WHERE ZoneName LIKE '%' + LTRIM(RTRIM(u.Region)) + '%'), 1) AS ZoneId,
        1 AS IsActive,
        GETUTCDATE() AS CreatedAt,
        0 AS IsDeleted
    FROM dbOutage.dbo.TblUserProfile u
) AS source
ON target.TnbId = source.TnbId
WHEN NOT MATCHED THEN
    INSERT (TnbId, FullName, Email, PhoneNumber, AuthType, RoleId, ZoneId, IsActive, CreatedAt, IsDeleted)
    VALUES (source.TnbId, source.FullName, source.Email, source.PhoneNumber, source.AuthType, source.RoleId, source.ZoneId, source.IsActive, source.CreatedAt, source.IsDeleted);
GO

-- 7. Migrate Outage Requests from TblTxOutRequest
PRINT 'Migrating Historical Outage Requests...';
INSERT INTO dbo.Outages (
    OutageNumber, OutageCode, OutageTypeCode, OutageClass, WorkTypeCode,
    ZoneId, StationId, VoltageLevelId, PrimaryEquipmentId, JobTypeId,
    PlannedStartAt, PlannedEndAt, Description, Justification,
    RequestorStatus, PlannerStatus, GnmStatus, GncStatus,
    CreatedAt, CreatedBy, IsDeleted
)
SELECT 
    'ICOMS-LEGACY-' + CAST(o.requestid AS VARCHAR(20)) AS OutageNumber,
    ISNULL(LEFT(LTRIM(RTRIM(o.outagecode)), 1), 'P') AS OutageCode,
    CASE 
        WHEN LTRIM(RTRIM(o.outagecode)) LIKE 'P%' THEN 'Planned'
        WHEN LTRIM(RTRIM(o.outagecode)) LIKE 'U%' THEN 'Unplanned'
        WHEN LTRIM(RTRIM(o.outagecode)) LIKE 'E%' THEN 'Emergency'
        WHEN LTRIM(RTRIM(o.outagecode)) LIKE 'F%' THEN 'Forced'
        ELSE 'Planned'
    END AS OutageTypeCode,
    CASE WHEN o.jobtype LIKE '%Maintenance%' THEN 'Maintenance' ELSE 'Project' END AS OutageClass,
    ISNULL(LTRIM(RTRIM(o.WorkType)), 'Dead') AS WorkTypeCode,
    ISNULL((SELECT TOP 1 ZoneId FROM config.Zones WHERE ZoneName LIKE '%' + LTRIM(RTRIM(o.region)) + '%'), 1) AS ZoneId,
    ISNULL((SELECT TOP 1 StationId FROM config.Stations WHERE StationAbbr = LTRIM(RTRIM(o.station))), 1) AS StationId,
    ISNULL((SELECT TOP 1 VoltageLevelId FROM config.VoltageLevel WHERE VoltageName = LTRIM(RTRIM(o.kv))), 1) AS VoltageLevelId,
    1 AS PrimaryEquipmentId,
    1 AS JobTypeId,
    ISNULL(o.datestart, GETUTCDATE()) AS PlannedStartAt,
    ISNULL(o.dateend, DATEADD(HOUR, 8, GETUTCDATE())) AS PlannedEndAt,
    ISNULL(o.description, 'Legacy Migration Record') AS Description,
    o.justification,
    CASE WHEN o.Confirmed = 1 THEN 'Confirmed' ELSE 'Pending' END AS RequestorStatus,
    'Agreed' AS PlannerStatus,
    CASE WHEN o.nldcstat = 1 THEN 'Approved' ELSE 'Pending' END AS GnmStatus,
    NULL AS GncStatus,
    ISNULL(o.dateregion, GETUTCDATE()) AS CreatedAt,
    1 AS CreatedBy,
    0 AS IsDeleted
FROM dbOutage.dbo.TblTxOutRequest o
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Outages WHERE OutageNumber = 'ICOMS-LEGACY-' + CAST(o.requestid AS VARCHAR(20))
);
GO

PRINT 'Data Migration from dbOutage completed successfully!';
