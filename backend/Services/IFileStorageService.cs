namespace Backend.Services;

public interface IFileStorageService
{
    /// <summary>
    /// Saves a file to storage and returns the relative file path
    /// </summary>
    /// <param name="fileStream">The file stream to save</param>
    /// <param name="fileName">The original filename</param>
    /// <param name="userId">The user ID for organizing files</param>
    /// <param name="category">Category folder (e.g., "registrations", "photos")</param>
    /// <returns>Relative file path that can be stored in database</returns>
    Task<string> SaveFileAsync(Stream fileStream, string fileName, Guid userId, string category);

    /// <summary>
    /// Retrieves a file stream from storage
    /// </summary>
    /// <param name="filePath">The relative file path</param>
    /// <returns>File stream or null if not found</returns>
    Task<Stream?> GetFileAsync(string filePath);

    /// <summary>
    /// Deletes a file from storage
    /// </summary>
    /// <param name="filePath">The relative file path</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteFileAsync(string filePath);

    /// <summary>
    /// Checks if a file exists in storage
    /// </summary>
    /// <param name="filePath">The relative file path</param>
    /// <returns>True if file exists</returns>
    Task<bool> FileExistsAsync(string filePath);

    /// <summary>
    /// Gets the public URL for accessing a file
    /// </summary>
    /// <param name="filePath">The relative file path</param>
    /// <returns>Full URL to access the file</returns>
    string GetFileUrl(string filePath);
}
