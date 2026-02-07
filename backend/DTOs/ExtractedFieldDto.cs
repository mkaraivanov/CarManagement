namespace Backend.DTOs;

public class ExtractedFieldDto<T>
{
    public T? Value { get; set; }
    public double Confidence { get; set; }

    public ExtractedFieldDto(T? value, double confidence)
    {
        Value = value;
        Confidence = Math.Max(0, Math.Min(1, confidence)); // Ensure between 0 and 1
    }

    public ExtractedFieldDto() : this(default, 0)
    {
    }
}
