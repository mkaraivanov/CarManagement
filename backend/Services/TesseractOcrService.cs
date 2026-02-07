using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using ImageMagick;
using Docnet.Core;
using Docnet.Core.Models;

namespace Backend.Services;

public class TesseractOcrService : IOcrService
{
    private readonly string _tesseractDataPath;
    private readonly string _tesseractBinaryPath;
    private readonly string _language;
    private readonly ILogger<TesseractOcrService> _logger;

    public TesseractOcrService(
        IConfiguration configuration,
        IWebHostEnvironment environment,
        ILogger<TesseractOcrService> logger)
    {
        var ocrConfig = configuration.GetSection("Ocr");
        var configuredPath = ocrConfig["TesseractDataPath"] ?? "tessdata";
        _language = ocrConfig["Language"] ?? "eng";
        _tesseractBinaryPath = ocrConfig["TesseractBinaryPath"] ?? "/opt/homebrew/bin/tesseract";
        _logger = logger;

        // Resolve path: if relative, make it relative to ContentRootPath
        if (Path.IsPathRooted(configuredPath))
        {
            _tesseractDataPath = configuredPath;
        }
        else
        {
            _tesseractDataPath = Path.Combine(environment.ContentRootPath, configuredPath);
        }

        _logger.LogInformation("Tesseract data path resolved to: {Path}", _tesseractDataPath);
        _logger.LogInformation("Tesseract binary path: {Path}", _tesseractBinaryPath);

        // Verify tesseract binary exists
        if (!File.Exists(_tesseractBinaryPath))
        {
            throw new FileNotFoundException($"Tesseract binary not found at '{_tesseractBinaryPath}'. Install with: brew install tesseract");
        }

        // Verify tessdata directory exists
        if (!Directory.Exists(_tesseractDataPath))
        {
            throw new DirectoryNotFoundException($"Tesseract data path '{_tesseractDataPath}' not found. Content root: {environment.ContentRootPath}");
        }

        var trainedDataFile = Path.Combine(_tesseractDataPath, $"{_language}.traineddata");
        if (!File.Exists(trainedDataFile))
        {
            throw new FileNotFoundException($"Tesseract trained data file '{trainedDataFile}' not found");
        }

        _logger.LogInformation("Tesseract OCR service initialized successfully with language: {Language}", _language);
    }

    public async Task<string> ExtractTextAsync(Stream imageStream, string language = "eng")
    {
        string? tempImagePath = null;

        try
        {
            _logger.LogInformation("Starting text extraction from image stream using Tesseract CLI");

            // Preprocess image for better OCR accuracy
            _logger.LogInformation("Preprocessing image...");
            var preprocessedBytes = await PreprocessImageAsync(imageStream);
            _logger.LogInformation("Image preprocessed, size: {Size} bytes", preprocessedBytes.Length);

            // Save preprocessed image to temporary file
            tempImagePath = Path.Combine(Path.GetTempPath(), $"ocr_{Guid.NewGuid()}.png");
            await File.WriteAllBytesAsync(tempImagePath, preprocessedBytes);
            _logger.LogInformation("Saved preprocessed image to: {Path}", tempImagePath);

            // Call Tesseract CLI
            var extractedText = await RunTesseractCliAsync(tempImagePath, language);

            _logger.LogInformation("OCR completed, text length: {Length}", extractedText?.Length ?? 0);

            return extractedText ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from image. Exception type: {Type}, Message: {Message}", ex.GetType().Name, ex.Message);
            if (ex.InnerException != null)
            {
                _logger.LogError("Inner exception: {Type}, Message: {Message}", ex.InnerException.GetType().Name, ex.InnerException.Message);
            }
            throw new InvalidOperationException("Failed to extract text from image", ex);
        }
        finally
        {
            // Clean up temporary file
            if (tempImagePath != null && File.Exists(tempImagePath))
            {
                try
                {
                    File.Delete(tempImagePath);
                    _logger.LogInformation("Deleted temporary file: {Path}", tempImagePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete temporary file: {Path}", tempImagePath);
                }
            }
        }
    }

    private async Task<string> RunTesseractCliAsync(string imagePath, string language)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _tesseractBinaryPath,
                    Arguments = $"\"{imagePath}\" stdout -l {language} --tessdata-dir \"{_tesseractDataPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            _logger.LogInformation("Running Tesseract CLI: {FileName} {Arguments}", process.StartInfo.FileName, process.StartInfo.Arguments);

            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            var output = await outputTask;
            var error = await errorTask;

            if (process.ExitCode != 0)
            {
                _logger.LogError("Tesseract CLI failed with exit code {ExitCode}. Error: {Error}", process.ExitCode, error);
                throw new InvalidOperationException($"Tesseract CLI failed: {error}");
            }

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("Tesseract CLI warnings: {Error}", error);
            }

            return output;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running Tesseract CLI");
            throw;
        }
    }

    public async Task<string> ExtractTextFromPdfAsync(Stream pdfStream)
    {
        try
        {
            var extractedTexts = new List<string>();

            // Convert PDF stream to byte array
            using (var memoryStream = new MemoryStream())
            {
                await pdfStream.CopyToAsync(memoryStream);
                var pdfBytes = memoryStream.ToArray();

                // Process PDF using Docnet
                using (var library = DocLib.Instance)
                {
                    using (var docReader = library.GetDocReader(pdfBytes, new PageDimensions(1080, 1920)))
                    {
                        var pageCount = docReader.GetPageCount();
                        _logger.LogInformation("Processing PDF with {PageCount} pages", pageCount);

                        // Process first page only for performance (registration docs are usually 1 page)
                        var pagesToProcess = Math.Min(pageCount, 1);

                        for (int i = 0; i < pagesToProcess; i++)
                        {
                            using (var pageReader = docReader.GetPageReader(i))
                            {
                                var rawBytes = pageReader.GetImage();
                                var width = pageReader.GetPageWidth();
                                var height = pageReader.GetPageHeight();

                                // Convert raw bytes to image stream
                                using (var imageStream = ConvertRawBytesToImageStream(rawBytes, width, height))
                                {
                                    var pageText = await ExtractTextAsync(imageStream, _language);
                                    extractedTexts.Add(pageText);
                                }
                            }
                        }
                    }
                }
            }

            return string.Join("\n\n", extractedTexts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from PDF");
            throw new InvalidOperationException("Failed to extract text from PDF", ex);
        }
    }

    public async Task<byte[]> PreprocessImageAsync(Stream imageStream)
    {
        try
        {
            _logger.LogInformation("Starting image preprocessing with ImageMagick");
            using (var image = new MagickImage(imageStream))
            {
                _logger.LogInformation("Image loaded: {Width}x{Height}, Format: {Format}", image.Width, image.Height, image.Format);

                // Convert to grayscale for better OCR accuracy
                image.Grayscale();

                // Increase contrast
                image.Contrast();
                image.Normalize();

                // Enhance sharpness
                image.Sharpen();

                // Ensure DPI is appropriate for OCR (300 DPI is ideal)
                image.Density = new Density(300, 300);

                // Convert to byte array
                var result = await Task.FromResult(image.ToByteArray(MagickFormat.Png));
                _logger.LogInformation("Image preprocessing completed, result size: {Size} bytes", result.Length);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error preprocessing image (Type: {Type}, Message: {Message}), using original", ex.GetType().Name, ex.Message);

            // If preprocessing fails, return original image
            using (var memoryStream = new MemoryStream())
            {
                imageStream.Position = 0;
                await imageStream.CopyToAsync(memoryStream);
                var result = memoryStream.ToArray();
                _logger.LogInformation("Returning original image, size: {Size} bytes", result.Length);
                return result;
            }
        }
    }

    private Stream ConvertRawBytesToImageStream(byte[] rawBytes, int width, int height)
    {
        // Docnet returns raw BGRA bytes, convert to image format
        using (var image = new MagickImage(rawBytes, new MagickReadSettings
        {
            Width = (uint)width,
            Height = (uint)height,
            Format = MagickFormat.Bgra,
            Depth = 8
        }))
        {
            var memoryStream = new MemoryStream();
            image.Write(memoryStream, MagickFormat.Png);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
