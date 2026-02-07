using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace Backend.Tests;

public class RegistrationOcrTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _output;
    private readonly string _testDataPath;

    public RegistrationOcrTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Path to test data - looking for TestData/SampleRegistrations in the backend project
        _testDataPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "..",
            "..",
            "..",
            "backend",
            "TestData",
            "SampleRegistrations"
        );

        _output.WriteLine($"Test data path: {_testDataPath}");
        _output.WriteLine($"Test data path exists: {Directory.Exists(_testDataPath)}");
    }

    private async Task<string> GetAuthTokenAsync()
    {
        // Register a test user and get auth token
        var registerRequest = new RegisterRequest
        {
            Username = $"testuser_{Guid.NewGuid():N}",
            Email = $"test_{Guid.NewGuid():N}@example.com",
            Password = "password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);

        return authResponse?.Token ?? throw new Exception("Failed to get auth token");
    }

    [Fact]
    public async Task ExtractRegistrationData_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "file", "test.jpg");

        // Act
        var response = await _client.PostAsync("/api/vehicle-registration/extract", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        _output.WriteLine("✅ Unauthorized access correctly blocked");
    }

    [Fact]
    public async Task ExtractRegistrationData_WithoutFile_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new MultipartFormDataContent();

        // Act
        var response = await _client.PostAsync("/api/vehicle-registration/extract", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response: {responseContent}");
        _output.WriteLine("✅ Missing file correctly rejected");
    }

    [Fact]
    public async Task ExtractRegistrationData_WithInvalidFileType_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        content.Add(fileContent, "file", "test.txt");

        // Act
        var response = await _client.PostAsync("/api/vehicle-registration/extract", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response: {responseContent}");
        Assert.Contains("Unsupported file format", responseContent);
        _output.WriteLine("✅ Invalid file type correctly rejected");
    }

    [Fact]
    public async Task ExtractRegistrationData_WithSamplePdf_ReturnsExtractedData()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Find the sample PDF
        var pdfPath = Path.Combine(_testDataPath, "ca_dmv_reg343.pdf");

        if (!File.Exists(pdfPath))
        {
            _output.WriteLine($"❌ Sample PDF not found at: {pdfPath}");
            _output.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
            _output.WriteLine($"Looking for files in: {_testDataPath}");

            if (Directory.Exists(_testDataPath))
            {
                var files = Directory.GetFiles(_testDataPath);
                _output.WriteLine($"Files found in test data directory:");
                foreach (var file in files)
                {
                    _output.WriteLine($"  - {file}");
                }
            }

            throw new FileNotFoundException($"Sample registration PDF not found at {pdfPath}");
        }

        _output.WriteLine($"✅ Found sample PDF at: {pdfPath}");
        _output.WriteLine($"File size: {new FileInfo(pdfPath).Length} bytes");

        var fileBytes = await File.ReadAllBytesAsync(pdfPath);
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
        content.Add(fileContent, "file", "ca_dmv_reg343.pdf");

        // Act
        _output.WriteLine("Sending request to /api/vehicle-registration/extract...");
        var response = await _client.PostAsync("/api/vehicle-registration/extract", content);

        // Log response details
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            _output.WriteLine($"❌ Request failed with status {response.StatusCode}");
            _output.WriteLine($"Response body: {responseContent}");
        }

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var extractResponse = JsonSerializer.Deserialize<RegistrationExtractResponse>(responseContent, _jsonOptions);

        Assert.NotNull(extractResponse);
        _output.WriteLine($"Success: {extractResponse.Success}");
        _output.WriteLine($"Message: {extractResponse.Message}");
        _output.WriteLine($"Raw Text Length: {extractResponse.RawText?.Length ?? 0}");

        if (extractResponse.RawText != null)
        {
            _output.WriteLine($"Raw Text Preview (first 500 chars):");
            _output.WriteLine(extractResponse.RawText.Substring(0, Math.Min(500, extractResponse.RawText.Length)));
        }

        if (extractResponse.ExtractedData != null)
        {
            _output.WriteLine("\nExtracted Fields:");
            _output.WriteLine($"  VIN: {extractResponse.ExtractedData.VIN?.Value} (Confidence: {extractResponse.ExtractedData.VIN?.Confidence})");
            _output.WriteLine($"  Registration Number: {extractResponse.ExtractedData.RegistrationNumber?.Value} (Confidence: {extractResponse.ExtractedData.RegistrationNumber?.Confidence})");
            _output.WriteLine($"  Make: {extractResponse.ExtractedData.Make?.Value} (Confidence: {extractResponse.ExtractedData.Make?.Confidence})");
            _output.WriteLine($"  Model: {extractResponse.ExtractedData.Model?.Value} (Confidence: {extractResponse.ExtractedData.Model?.Confidence})");
            _output.WriteLine($"  Year: {extractResponse.ExtractedData.Year?.Value} (Confidence: {extractResponse.ExtractedData.Year?.Confidence})");
        }

        // Should have extracted some text
        Assert.NotNull(extractResponse.RawText);
        Assert.NotEmpty(extractResponse.RawText);

        _output.WriteLine("✅ OCR extraction completed successfully");
    }

    [Fact]
    public async Task ExtractRegistrationData_WithSimpleImage_ReturnsExtractedData()
    {
        // Create a simple test image with text
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create a minimal valid PNG file (1x1 pixel)
        var pngBytes = new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG signature
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52, // IHDR chunk
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, // 1x1 dimensions
            0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
            0x89, 0x00, 0x00, 0x00, 0x0A, 0x49, 0x44, 0x41, // IDAT chunk
            0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
            0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00,
            0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, // IEND chunk
            0x42, 0x60, 0x82
        };

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(pngBytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        content.Add(fileContent, "file", "test.png");

        // Act
        _output.WriteLine("Sending simple PNG to /api/vehicle-registration/extract...");
        var response = await _client.PostAsync("/api/vehicle-registration/extract", content);

        // Log response
        var responseContent = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response Status: {response.StatusCode}");
        _output.WriteLine($"Response Content: {responseContent}");

        // Assert - should succeed but may not extract much text from a 1x1 pixel
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var extractResponse = JsonSerializer.Deserialize<RegistrationExtractResponse>(responseContent, _jsonOptions);
        Assert.NotNull(extractResponse);

        _output.WriteLine($"✅ Simple image processed (may have empty text, which is ok)");
    }
}
