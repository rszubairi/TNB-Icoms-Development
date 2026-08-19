namespace TnbIcoms.Application.Mnemonics.Dtos;

public class MnemonicDocumentDto
{
    public int MnemonicDocumentId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string? UploadedByName { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsCurrent { get; set; }
}
