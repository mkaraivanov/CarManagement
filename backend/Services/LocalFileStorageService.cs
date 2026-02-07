using Microsoft.Extensions.Configuration;

namespace Backend.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly long _maxFileSizeBytes;
    private readonly string[] _allowedExtensions;
    private readonly string _baseUrl;

    public LocalFileStorageService(IConfiguration configuration)
    {
        var fileStorageConfig = configuration.GetSection("FileStorage");
        _basePath = fileStorageConfig["BasePath"] ?? "uploads";
        _maxFileSizeBytes = (long.Parse(fileStorageConfig["MaxFileSizeMB"] ?? "10") * 1024 * 1024);
        _allowedExtensions = fileStorageConfig.GetSection("AllowedExtensions").Get<string[]>()
            ?? new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        _baseUrl = fileStorageConfig["BaseUrl"] ?? "/files";

        // Create base upload directory if it doesn't exist
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, Guid userId, string category)
    {
        // Validate file size
        if (fileStream.Length > _maxFileSizeBytes)
        {
            throw new InvalidOperationException($"File size exceeds maximum allowed size of {_maxFileSizeBytes / 1024 / 1024}MB");
        }

        // Validate file extension
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");
        }

        // Create user/category folder structure
        var userFolder = Path.Combine(_basePath, userId.ToString());
        var categoryFolder = Path.Combine(userFolder, category);
        Directory.CreateDirectory(categoryFolder);

        // Generate unique filename
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(categoryFolder, uniqueFileName);

        // Save file
        using (var fileStreamOutput = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(fileStreamOutput);
        }

        // Return relative path for database storage
        var relativePath = Path.Combine(userId.ToString(), category, uniqueFileName);
        return relativePath.Replace("\\", "/"); // Normalize path separators
    }

    public async Task<Stream?> GetFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath);

        if (!File.Exists(fullPath))
        {
            return null;
        }

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(memoryStream);
        }
        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath);

        if (!File.Exists(fullPath))
        {
            return Task.FromResult(false);
        }

        try
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    public string GetFileUrl(string filePath)
    {
        // Return URL that will be served by static file middleware
        return $"{_baseUrl}/{filePath.Replace("\\", "/")}";
    }
}
