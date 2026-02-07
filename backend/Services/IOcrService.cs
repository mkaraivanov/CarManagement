namespace Backend.Services;

public interface IOcrService
{
    /// <summary>
    /// Extracts text from an image stream using OCR
    /// </summary>
    /// <param name="imageStream">The image stream to process</param>
    /// <param name="language">Language for OCR (default: "eng")</param>
    /// <returns>Extracted text from the image</returns>
    Task<string> ExtractTextAsync(Stream imageStream, string language = "eng");

    /// <summary>
    /// Extracts text from a PDF document by converting pages to images
    /// </summary>
    /// <param name="pdfStream">The PDF stream to process</param>
    /// <returns>Extracted text from all pages</returns>
    Task<string> ExtractTextFromPdfAsync(Stream pdfStream);

    /// <summary>
    /// Preprocesses an image to improve OCR accuracy (grayscale, contrast, etc.)
    /// </summary>
    /// <param name="imageStream">The image stream to preprocess</param>
    /// <returns>Preprocessed image as byte array</returns>
    Task<byte[]> PreprocessImageAsync(Stream imageStream);
}
