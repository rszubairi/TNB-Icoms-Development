namespace TnbIcoms.Application.TransmissionLines.Dtos;

public class GeneratedNameDto
{
    public int StationId { get; set; }
    public string StationAbbr { get; set; } = string.Empty;
    public string GeneratedName { get; set; } = string.Empty;
}

public class TransmissionLineDto
{
    public int TransmissionLineId { get; set; }
    public int VoltageLevelId { get; set; }
    public string? VoltageLevelName { get; set; }
    public int EquipmentTypeId { get; set; }
    public string? EquipmentTypeName { get; set; }
    public int NamingInteger { get; set; }
    public int LineNumber { get; set; }
    public string LineFilterType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<GeneratedNameDto> GeneratedNames { get; set; } = new();
    public List<int> OwnerZoneIds { get; set; } = new();
    public List<string> OwnerZoneNames { get; set; } = new();
}

/// <summary>Computes the naming preview before commit, and is reused as the save payload.</summary>
public class TransmissionLineRequestDto
{
    public int VoltageLevelId { get; set; }
    public int EquipmentTypeId { get; set; }
    public int NamingInteger { get; set; }
    public int LineNumber { get; set; }
    public List<int> StationIdsInOrder { get; set; } = new(); // 2-4 stations, first/last = ends
}

public class AddOwnerZoneRequestDto
{
    public int ZoneId { get; set; }
}
