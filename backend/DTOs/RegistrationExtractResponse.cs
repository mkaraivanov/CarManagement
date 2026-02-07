namespace Backend.DTOs;

public class RegistrationExtractResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ExtractedDataDto ExtractedData { get; set; } = new();
    public string? RawText { get; set; }
}

public class ExtractedDataDto
{
    public ExtractedFieldDto<string> Make { get; set; } = new();
    public ExtractedFieldDto<string> Model { get; set; } = new();
    public ExtractedFieldDto<int> Year { get; set; } = new();
    public ExtractedFieldDto<string> VIN { get; set; } = new();
    public ExtractedFieldDto<string> LicensePlate { get; set; } = new();
    public ExtractedFieldDto<string> Color { get; set; } = new();
    public ExtractedFieldDto<int> CurrentMileage { get; set; } = new();
    public ExtractedFieldDto<string> OwnerName { get; set; } = new();
    public ExtractedFieldDto<string> OwnerAddress { get; set; } = new();
    public ExtractedFieldDto<string> RegistrationNumber { get; set; } = new();
    public ExtractedFieldDto<DateTime> RegistrationExpiryDate { get; set; } = new();
    public ExtractedFieldDto<DateTime> RegistrationIssueDate { get; set; } = new();
    public ExtractedFieldDto<string> BodyType { get; set; } = new();
    public ExtractedFieldDto<string> FuelType { get; set; } = new();
}
