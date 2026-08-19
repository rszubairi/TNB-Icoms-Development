namespace TnbIcoms.Application.Files;

public interface IFileStorageService
{
    /// <summary>Saves a stream under the given sub-folder with a generated unique file name, returning that name.</summary>
    Task<string> SaveAsync(Stream content, string subFolder, string originalFileName);

    /// <summary>Opens a previously-saved file for reading. Returns null if it no longer exists on disk.</summary>
    Stream? OpenRead(string subFolder, string storedFileName);

    void Delete(string subFolder, string storedFileName);
}
