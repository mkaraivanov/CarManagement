using Microsoft.Extensions.Configuration;
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

    public TesseractOcrService(IConfiguration configuration, ILogger<TesseractOcrService> logger)
    {
        var ocrConfig = configuration.GetSection("Ocr");
        _tesseractDataPath = ocrConfig["TesseractDataPath"] ?? "tessdata";
        _language = ocrConfig["Language"] ?? "eng";
        _logger = logger;

        // Verify tessdata directory exists
        if (!Directory.Exists(_tesseractDataPath))
        {
            throw new DirectoryNotFoundException($"Tesseract data path '{_tesseractDataPath}' not found");
        }

        var trainedDataFile = Path.Combine(_tesseractDataPath, $"{_language}.traineddata");
        if (!File.Exists(trainedDataFile))
        {
            throw new FileNotFoundException($"Tesseract trained data file '{trainedDataFile}' not found");
        }
    }

    public async Task<string> ExtractTextAsync(Stream imageStream, string language = "eng")
    {
        try
        {
            // Preprocess image for better OCR accuracy
            var preprocessedBytes = await PreprocessImageAsync(imageStream);

            // Use Tesseract to extract text
            using (var engine = new TesseractEngine(_tesseractDataPath, language, EngineMode.Default))
            {
                using (var img = Pix.LoadFromMemory(preprocessedBytes))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        var confidence = page.GetMeanConfidence();

                        _logger.LogInformation("OCR completed with confidence: {Confidence}", confidence);

                        return text ?? string.Empty;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from image");
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
                    using (var docReader = library.GetDocReader(pdfBytes, new PageDimensions(1920, 1080)))
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
            using (var image = new MagickImage(imageStream))
            {
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
                return await Task.FromResult(image.ToByteArray(MagickFormat.Png));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error preprocessing image, using original");

            // If preprocessing fails, return original image
            using (var memoryStream = new MemoryStream())
            {
                imageStream.Position = 0;
                await imageStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    private Stream ConvertRawBytesToImageStream(byte[] rawBytes, int width, int height)
    {
        // Docnet returns raw BGRA bytes, convert to image format
        using (var image = new MagickImage(rawBytes, new MagickReadSettings
        {
            Width = width,
            Height = height,
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
