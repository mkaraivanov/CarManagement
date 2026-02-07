using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Tesseract;
using ImageMagick;
using Docnet.Core;
using Docnet.Core.Models;

namespace Backend.Services;

public class TesseractOcrService : IOcrService
{
    private readonly string _tesseractDataPath;
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
        try
        {
            _logger.LogInformation("Starting text extraction from image stream");

            // Preprocess image for better OCR accuracy
            _logger.LogInformation("Preprocessing image...");
            var preprocessedBytes = await PreprocessImageAsync(imageStream);
            _logger.LogInformation("Image preprocessed, size: {Size} bytes", preprocessedBytes.Length);

            // Use Tesseract to extract text
            _logger.LogInformation("Initializing Tesseract engine with path: {Path}, language: {Language}", _tesseractDataPath, language);
            using (var engine = new TesseractEngine(_tesseractDataPath, language, EngineMode.Default))
            {
                _logger.LogInformation("Loading image from memory...");
                using (var img = Pix.LoadFromMemory(preprocessedBytes))
                {
                    _logger.LogInformation("Processing image with Tesseract...");
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        var confidence = page.GetMeanConfidence();

                        _logger.LogInformation("OCR completed with confidence: {Confidence}, text length: {Length}", confidence, text?.Length ?? 0);

                        return text ?? string.Empty;
                    }
                }
            }
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
