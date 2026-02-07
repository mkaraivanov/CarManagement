using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Backend.DTOs;
using Backend.Services;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/vehicle-registration")]
public class VehicleRegistrationController : ControllerBase
{
    private readonly IOcrService _ocrService;
    private readonly RegistrationParserService _parserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<VehicleRegistrationController> _logger;
    private readonly IConfiguration _configuration;

    private readonly string[] _allowedExtensions;
    private readonly long _maxFileSizeBytes;

    public VehicleRegistrationController(
        IOcrService ocrService,
        RegistrationParserService parserService,
        IFileStorageService fileStorageService,
        ILogger<VehicleRegistrationController> logger,
        IConfiguration configuration)
    {
        _ocrService = ocrService;
        _parserService = parserService;
        _fileStorageService = fileStorageService;
        _logger = logger;
        _configuration = configuration;

        var fileStorageConfig = configuration.GetSection("FileStorage");
        _allowedExtensions = fileStorageConfig.GetSection("AllowedExtensions").Get<string[]>()
            ?? new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        _maxFileSizeBytes = (long.Parse(fileStorageConfig["MaxFileSizeMB"] ?? "10") * 1024 * 1024);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Extracts vehicle registration data from an uploaded document image/PDF
    /// </summary>
    /// <param name="file">The registration document file (JPG, PNG, or PDF)</param>
    /// <returns>Extracted registration data with confidence scores</returns>
    [HttpPost("extract")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB
    [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)]
    public async Task<IActionResult> ExtractRegistrationData(IFormFile file)
    {
        try
        {
            // Validate file is provided
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No file provided",
                    errors = new[] { "Please upload a registration document image or PDF" }
                });
            }

            // Validate file size
            if (file.Length > _maxFileSizeBytes)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "File size exceeds limit",
                    errors = new[] { $"File size must be less than {_maxFileSizeBytes / 1024 / 1024}MB" }
                });
            }

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Unsupported file format",
                    errors = new[] { $"Please upload {string.Join(", ", _allowedExtensions)} files only" }
                });
            }

            _logger.LogInformation("Processing registration document upload: {FileName} ({Size} bytes)",
                file.FileName, file.Length);

            string extractedText;

            // Extract text using OCR
            using (var stream = file.OpenReadStream())
            {
                if (extension == ".pdf")
                {
                    extractedText = await _ocrService.ExtractTextFromPdfAsync(stream);
                }
                else
                {
                    extractedText = await _ocrService.ExtractTextAsync(stream);
                }
            }

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                return Ok(new RegistrationExtractResponse
                {
                    Success = false,
                    Message = "No text could be extracted from the document. Please ensure the image is clear and readable.",
                    ExtractedData = new ExtractedDataDto(),
                    RawText = string.Empty
                });
            }

            // Parse extracted text into structured data
            var response = _parserService.ParseRegistrationText(extractedText);

            _logger.LogInformation("Successfully extracted registration data");

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error processing registration document");
            return BadRequest(new
            {
                success = false,
                message = "Failed to process document",
                errors = new[] { ex.Message }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing registration document");
            return StatusCode(500, new
            {
                success = false,
                message = "An unexpected error occurred while processing the document",
                errors = new[] { "Please try again or contact support if the problem persists" }
            });
        }
    }

    /// <summary>
    /// Uploads a registration document for an existing vehicle
    /// </summary>
    /// <param name="vehicleId">The vehicle ID</param>
    /// <param name="file">The registration document file</param>
    /// <returns>Success status and document URL</returns>
    [HttpPost("upload/{vehicleId}")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)]
    public async Task<IActionResult> UploadRegistrationDocument(Guid vehicleId, IFormFile file)
    {
        try
        {
            var userId = GetUserId();

            // Validate file
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file provided" });
            }

            // Validate file size and extension
            if (file.Length > _maxFileSizeBytes)
            {
                return BadRequest(new { success = false, message = "File size exceeds limit" });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return BadRequest(new { success = false, message = "Unsupported file format" });
            }

            // Save file
            string filePath;
            using (var stream = file.OpenReadStream())
            {
                filePath = await _fileStorageService.SaveFileAsync(
                    stream,
                    file.FileName,
                    userId,
                    "registrations"
                );
            }

            var fileUrl = _fileStorageService.GetFileUrl(filePath);

            _logger.LogInformation("Registration document uploaded for vehicle {VehicleId}: {FilePath}",
                vehicleId, filePath);

            return Ok(new
            {
                success = true,
                message = "Registration document uploaded successfully",
                documentUrl = filePath,
                publicUrl = fileUrl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading registration document");
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to upload document",
                error = ex.Message
            });
        }
    }
}
