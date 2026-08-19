namespace TnbIcoms.Application.Equipment.Dtos;

public class EquipmentListItemDto
{
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public string? ZoneName { get; set; }
    public int StationId { get; set; }
    public string? StationName { get; set; }
    public int VoltageLevelId { get; set; }
    public string? VoltageLevelName { get; set; }
    public int EquipmentTypeId { get; set; }
    public string? EquipmentTypeName { get; set; }
    public int? MvaRatingId { get; set; }
    public string? MvaRatingLabel { get; set; }
    public byte Position { get; set; }
    public bool IsOffPoint { get; set; }
    public string? OffPointRemark { get; set; }
    public bool IsActive { get; set; }
}

public class CreateEquipmentRequestDto
{
    public int StationId { get; set; }
    public int VoltageLevelId { get; set; }
    public int EquipmentTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? MvaRatingId { get; set; }
    public bool IsOpen { get; set; }
    public bool IsOffPoint { get; set; }
    public string? OffPointRemark { get; set; }
}

public class UpdateEquipmentRequestDto
{
    public string Name { get; set; } = string.Empty;
    public int? MvaRatingId { get; set; }
    public bool IsOpen { get; set; }
    public bool IsOffPoint { get; set; }
    public string? OffPointRemark { get; set; }
    public bool IsActive { get; set; } = true;
}
