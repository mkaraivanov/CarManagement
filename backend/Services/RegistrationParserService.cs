using System.Text.RegularExpressions;
using Backend.DTOs;

namespace Backend.Services;

public class RegistrationParserService
{
    private readonly ILogger<RegistrationParserService> _logger;

    // Regex patterns for various fields
    private static class Patterns
    {
        // VIN: 17 alphanumeric characters (excluding I, O, Q)
        public const string VIN = @"\b[A-HJ-NPR-Z0-9]{17}\b";

        // License plate: varies by state, typically 2-8 alphanumeric
        public const string LicensePlate = @"\b[A-Z0-9]{2,8}\b";

        // Date patterns: MM/DD/YYYY, MM-DD-YYYY, or similar
        public const string Date = @"\b\d{1,2}[/-]\d{1,2}[/-]\d{4}\b";

        // Year: 4 digits between 1900 and current year + 5
        public static string Year = $@"\b(19\d{{2}}|20[0-{DateTime.Now.Year.ToString()[2]}]\d)\b";

        // Registration number: varies, often alphanumeric
        public const string RegistrationNumber = @"\b(REG[\s#]*\d+|[A-Z]{2}\d{{6,10}})\b";

        // Mileage/Odometer: number followed by optional "miles" or "mi"
        public const string Mileage = @"\b(\d{1,3}(?:,\d{3})*|\d+)\s*(?:miles?|mi)?\b";

        // Make: Common car manufacturers
        public static readonly string[] Makes = new[]
        {
            "Toyota", "Honda", "Ford", "Chevrolet", "Chevy", "Nissan", "BMW", "Mercedes-Benz",
            "Mercedes", "Audi", "Volkswagen", "VW", "Hyundai", "Kia", "Mazda", "Subaru",
            "Lexus", "Jeep", "Ram", "GMC", "Dodge", "Chrysler", "Buick", "Cadillac",
            "Tesla", "Volvo", "Porsche", "Land Rover", "Jaguar", "Acura", "Infiniti"
        };

        // Body types
        public static readonly string[] BodyTypes = new[]
        {
            "Sedan", "Coupe", "SUV", "Truck", "Van", "Wagon", "Convertible", "Hatchback",
            "Minivan", "Pickup", "Crossover"
        };

        // Fuel types
        public static readonly string[] FuelTypes = new[]
        {
            "Gasoline", "Gas", "Diesel", "Electric", "Hybrid", "Plug-in Hybrid", "E85", "CNG"
        };
    }

    public RegistrationParserService(ILogger<RegistrationParserService> logger)
    {
        _logger = logger;
    }

    public RegistrationExtractResponse ParseRegistrationText(string ocrText)
    {
        _logger.LogInformation("Parsing OCR text ({Length} characters)", ocrText.Length);

        var response = new RegistrationExtractResponse
        {
            Success = !string.IsNullOrWhiteSpace(ocrText),
            RawText = ocrText,
            ExtractedData = new ExtractedDataDto()
        };

        if (!response.Success)
        {
            response.Message = "No text extracted from document";
            return response;
        }

        // Extract VIN
        response.ExtractedData.VIN = ExtractVIN(ocrText);

        // Extract license plate
        response.ExtractedData.LicensePlate = ExtractLicensePlate(ocrText);

        // Extract dates
        (response.ExtractedData.RegistrationExpiryDate, response.ExtractedData.RegistrationIssueDate)
            = ExtractDates(ocrText);

        // Extract year
        response.ExtractedData.Year = ExtractYear(ocrText);

        // Extract make and model
        (response.ExtractedData.Make, response.ExtractedData.Model) = ExtractMakeModel(ocrText);

        // Extract owner name
        response.ExtractedData.OwnerName = ExtractOwnerName(ocrText);

        // Extract owner address
        response.ExtractedData.OwnerAddress = ExtractOwnerAddress(ocrText);

        // Extract registration number
        response.ExtractedData.RegistrationNumber = ExtractRegistrationNumber(ocrText);

        // Extract body type
        response.ExtractedData.BodyType = ExtractBodyType(ocrText);

        // Extract fuel type
        response.ExtractedData.FuelType = ExtractFuelType(ocrText);

        // Extract color
        response.ExtractedData.Color = ExtractColor(ocrText);

        // Extract mileage
        response.ExtractedData.CurrentMileage = ExtractMileage(ocrText);

        response.Message = "Data extracted successfully";
        _logger.LogInformation("Extraction complete");

        return response;
    }

    private ExtractedFieldDto<string> ExtractVIN(string text)
    {
        var match = Regex.Match(text, Patterns.VIN, RegexOptions.IgnoreCase);
        if (match.Success)
        {
            var vin = match.Value.ToUpperInvariant();
            // Higher confidence if near "VIN" keyword
            var confidence = text.IndexOf("VIN", StringComparison.OrdinalIgnoreCase) >= 0 ? 0.95 : 0.85;
            return new ExtractedFieldDto<string>(vin, confidence);
        }
        return new ExtractedFieldDto<string>();
    }

    private ExtractedFieldDto<string> ExtractLicensePlate(string text)
    {
        // Look for license plate near keywords
        var keywords = new[] { "license", "plate", "tag", "registration" };
        var bestMatch = FindNearKeyword(text, Patterns.LicensePlate, keywords);

        if (!string.IsNullOrEmpty(bestMatch))
        {
            return new ExtractedFieldDto<string>(bestMatch.ToUpperInvariant(), 0.80);
        }

        return new ExtractedFieldDto<string>();
    }

    private (ExtractedFieldDto<DateTime>, ExtractedFieldDto<DateTime>) ExtractDates(string text)
    {
        var dates = Regex.Matches(text, Patterns.Date)
            .Cast<Match>()
            .Select(m => m.Value)
            .ToList();

        var expiryDate = new ExtractedFieldDto<DateTime>();
        var issueDate = new ExtractedFieldDto<DateTime>();

        foreach (var dateStr in dates)
        {
            if (DateTime.TryParse(dateStr, out var date))
            {
                // Expiration is usually in the future
                if (date > DateTime.Now && expiryDate.Value == default)
                {
                    var confidence = text.IndexOf("expir", StringComparison.OrdinalIgnoreCase) >= 0 ? 0.90 : 0.75;
                    expiryDate = new ExtractedFieldDto<DateTime>(date, confidence);
                }
                // Issue date is usually in the past
                else if (date < DateTime.Now && issueDate.Value == default)
                {
                    var confidence = text.IndexOf("issue", StringComparison.OrdinalIgnoreCase) >= 0 ? 0.90 : 0.70;
                    issueDate = new ExtractedFieldDto<DateTime>(date, confidence);
                }
            }
        }

        return (expiryDate, issueDate);
    }

    private ExtractedFieldDto<int> ExtractYear(string text)
    {
        var match = Regex.Match(text, Patterns.Year);
        if (match.Success && int.TryParse(match.Value, out var year))
        {
            var confidence = text.IndexOf("year", StringComparison.OrdinalIgnoreCase) >= 0 ? 0.95 : 0.85;
            return new ExtractedFieldDto<int>(year, confidence);
        }
        return new ExtractedFieldDto<int>();
    }

    private (ExtractedFieldDto<string>, ExtractedFieldDto<string>) ExtractMakeModel(string text)
    {
        var make = new ExtractedFieldDto<string>();
        var model = new ExtractedFieldDto<string>();

        // Find make
        foreach (var makeName in Patterns.Makes)
        {
            if (text.Contains(makeName, StringComparison.OrdinalIgnoreCase))
            {
                make = new ExtractedFieldDto<string>(makeName, 0.90);
                break;
            }
        }

        // Model extraction is more complex and context-dependent
        // For now, look for capitalized words near "model" keyword
        var modelMatch = FindNearKeyword(text, @"\b[A-Z][a-zA-Z0-9\-]{2,15}\b", new[] { "model" });
        if (!string.IsNullOrEmpty(modelMatch))
        {
            model = new ExtractedFieldDto<string>(modelMatch, 0.75);
        }

        return (make, model);
    }

    private ExtractedFieldDto<string> ExtractOwnerName(string text)
    {
        // Look for name near "owner" or "name" keywords
        var namePattern = @"\b[A-Z][a-z]+\s+[A-Z][a-z]+(?:\s+[A-Z][a-z]+)?\b";
        var name = FindNearKeyword(text, namePattern, new[] { "owner", "name", "registered" });

        if (!string.IsNullOrEmpty(name))
        {
            return new ExtractedFieldDto<string>(name, 0.70);
        }

        return new ExtractedFieldDto<string>();
    }

    private ExtractedFieldDto<string> ExtractOwnerAddress(string text)
    {
        // Look for address near keywords
        // Address pattern: number + street name + city + state + zip
        var addressPattern = @"\d+\s+[A-Za-z\s]+(?:Street|St|Avenue|Ave|Road|Rd|Drive|Dr|Lane|Ln|Boulevard|Blvd).*?(?:\d{5}(?:-\d{4})?)";
        var match = Regex.Match(text, addressPattern, RegexOptions.IgnoreCase);

        if (match.Success)
        {
            return new ExtractedFieldDto<string>(match.Value.Trim(), 0.65);
        }

        return new ExtractedFieldDto<string>();
    }

    private ExtractedFieldDto<string> ExtractRegistrationNumber(string text)
    {
        var match = Regex.Match(text, Patterns.RegistrationNumber, RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return new ExtractedFieldDto<string>(match.Value, 0.85);
        }
        return new ExtractedFieldDto<string>();
    }

    private ExtractedFieldDto<string> ExtractBodyType(string text)
    {
        foreach (var bodyType in Patterns.BodyTypes)
        {
            if (text.Contains(bodyType, StringComparison.OrdinalIgnoreCase))
            {
                return new ExtractedFieldDto<string>(bodyType, 0.85);
            }
        }
        return new ExtractedFieldDto<string>();
    }

    private ExtractedFieldDto<string> ExtractFuelType(string text)
    {
        foreach (var fuelType in Patterns.FuelTypes)
        {
            if (text.Contains(fuelType, StringComparison.OrdinalIgnoreCase))
            {
                return new ExtractedFieldDto<string>(fuelType, 0.85);
            }
        }
        return new ExtractedFieldDto<string>();
    }

    private ExtractedFieldDto<string> ExtractColor(string text)
    {
        var commonColors = new[] {
            "Black", "White", "Silver", "Gray", "Grey", "Red", "Blue", "Green", "Yellow",
            "Orange", "Brown", "Tan", "Beige", "Gold", "Purple", "Maroon"
        };

        foreach (var color in commonColors)
        {
            if (text.Contains(color, StringComparison.OrdinalIgnoreCase))
            {
                var confidence = text.IndexOf("color", StringComparison.OrdinalIgnoreCase) >= 0 ? 0.85 : 0.70;
                return new ExtractedFieldDto<string>(color, confidence);
            }
        }

        return new ExtractedFieldDto<string>();
    }

    private ExtractedFieldDto<int> ExtractMileage(string text)
    {
        // Look for numbers near "mileage", "odometer", or "miles" keywords
        var matches = Regex.Matches(text, Patterns.Mileage, RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            var numStr = Regex.Replace(match.Groups[1].Value, @"[,\s]", "");
            if (int.TryParse(numStr, out var mileage) && mileage > 0 && mileage < 1000000)
            {
                var hasKeyword = text.IndexOf("mileage", StringComparison.OrdinalIgnoreCase) >= 0
                    || text.IndexOf("odometer", StringComparison.OrdinalIgnoreCase) >= 0;
                var confidence = hasKeyword ? 0.80 : 0.60;

                return new ExtractedFieldDto<int>(mileage, confidence);
            }
        }

        return new ExtractedFieldDto<int>();
    }

    private string? FindNearKeyword(string text, string pattern, string[] keywords)
    {
        var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            foreach (var keyword in keywords)
            {
                var keywordIndex = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
                if (keywordIndex >= 0)
                {
                    // Check if match is within 50 characters of keyword
                    var distance = Math.Abs(match.Index - keywordIndex);
                    if (distance < 50)
                    {
                        return match.Value;
                    }
                }
            }
        }

        return null;
    }
}
