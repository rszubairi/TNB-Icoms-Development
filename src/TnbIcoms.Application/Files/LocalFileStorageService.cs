using Microsoft.AspNetCore.Hosting;

namespace TnbIcoms.Application.Files;

/// <summary>
/// Local-disk implementation of <see cref="IFileStorageService"/>, storing files under
/// {ContentRoot}/App_Data/uploads/{subFolder}. Swap for an Azure Blob implementation in
/// production by registering a different IFileStorageService - nothing above this
/// interface needs to change.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _rootPath = Path.Combine(environment.ContentRootPath, "App_Data", "uploads");
    }

    public async Task<string> SaveAsync(Stream content, string subFolder, string originalFileName)
    {
        var folder = Path.Combine(_rootPath, subFolder);
        Directory.CreateDirectory(folder);

        var extension = Path.GetExtension(originalFileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(folder, storedFileName);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fileStream);

        return storedFileName;
    }

    public Stream? OpenRead(string subFolder, string storedFileName)
    {
        var fullPath = Path.Combine(_rootPath, subFolder, storedFileName);
        return File.Exists(fullPath) ? new FileStream(fullPath, FileMode.Open, FileAccess.Read) : null;
    }

    public void Delete(string subFolder, string storedFileName)
    {
        var fullPath = Path.Combine(_rootPath, subFolder, storedFileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
