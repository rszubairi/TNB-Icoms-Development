using TnbIcoms.Domain.Entities.Auth;

namespace TnbIcoms.Domain.Entities.Config;

/// <summary>
/// URS Module 1 §5.2.15: the Mnemonic list PDF, kept up to date by GNM ADMIN and
/// referenced by the GNM Engineer during SLD setup. Every upload is retained as history;
/// the most recent row is the current version.
/// </summary>
public class MnemonicDocument
{
    public int MnemonicDocumentId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty; // Physical file name on disk
    public long FileSizeBytes { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public User? UploadedByUser { get; set; }
}
