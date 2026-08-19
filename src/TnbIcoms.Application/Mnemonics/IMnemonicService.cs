using TnbIcoms.Application.Common;
using TnbIcoms.Application.Mnemonics.Dtos;

namespace TnbIcoms.Application.Mnemonics;

public interface IMnemonicService
{
    Task<ApiResponse<List<MnemonicDocumentDto>>> ListAsync();
    Task<ApiResponse<MnemonicDocumentDto>> UploadAsync(Stream content, string originalFileName, int uploadedByUserId);
    Task<(Stream? Content, string? FileName)> OpenCurrentAsync();
    Task<(Stream? Content, string? FileName)> OpenAsync(int mnemonicDocumentId);
}
