using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Files;
using TnbIcoms.Application.Mnemonics.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Mnemonics;

public class MnemonicService : IMnemonicService
{
    private const string SubFolder = "mnemonic";

    private readonly AppDbContext _dbContext;
    private readonly IFileStorageService _fileStorage;

    public MnemonicService(AppDbContext dbContext, IFileStorageService fileStorage)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
    }

    public async Task<ApiResponse<List<MnemonicDocumentDto>>> ListAsync()
    {
        var documents = await _dbContext.MnemonicDocuments
            .Include(d => d.UploadedByUser)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();

        var currentId = documents.FirstOrDefault()?.MnemonicDocumentId;

        return ApiResponse<List<MnemonicDocumentDto>>.Ok(
            documents.Select(d => Map(d, d.MnemonicDocumentId == currentId)).ToList());
    }

    public async Task<ApiResponse<MnemonicDocumentDto>> UploadAsync(Stream content, string originalFileName, int uploadedByUserId)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            return ApiResponse<MnemonicDocumentDto>.Fail("A file is required.");
        }

        if (!originalFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return ApiResponse<MnemonicDocumentDto>.Fail("Only PDF files are accepted.");
        }

        var storedFileName = await _fileStorage.SaveAsync(content, SubFolder, originalFileName);

        var document = new MnemonicDocument
        {
            OriginalFileName = originalFileName,
            StoredFileName = storedFileName,
            FileSizeBytes = content.Length,
            UploadedBy = uploadedByUserId,
            UploadedAt = DateTime.UtcNow
        };

        _dbContext.MnemonicDocuments.Add(document);
        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(document).Reference(d => d.UploadedByUser).LoadAsync();

        return ApiResponse<MnemonicDocumentDto>.Ok(Map(document, isCurrent: true));
    }

    public async Task<(Stream? Content, string? FileName)> OpenCurrentAsync()
    {
        var current = await _dbContext.MnemonicDocuments
            .OrderByDescending(d => d.UploadedAt)
            .FirstOrDefaultAsync();

        if (current is null)
        {
            return (null, null);
        }

        return (_fileStorage.OpenRead(SubFolder, current.StoredFileName), current.OriginalFileName);
    }

    public async Task<(Stream? Content, string? FileName)> OpenAsync(int mnemonicDocumentId)
    {
        var document = await _dbContext.MnemonicDocuments.FirstOrDefaultAsync(d => d.MnemonicDocumentId == mnemonicDocumentId);
        if (document is null)
        {
            return (null, null);
        }

        return (_fileStorage.OpenRead(SubFolder, document.StoredFileName), document.OriginalFileName);
    }

    private static MnemonicDocumentDto Map(MnemonicDocument document, bool isCurrent)
    {
        return new MnemonicDocumentDto
        {
            MnemonicDocumentId = document.MnemonicDocumentId,
            OriginalFileName = document.OriginalFileName,
            FileSizeBytes = document.FileSizeBytes,
            UploadedByName = document.UploadedByUser?.FullName,
            UploadedAt = document.UploadedAt,
            IsCurrent = isCurrent
        };
    }
}
