namespace Backend.DTOs;

public class CarMakeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<CarModelDto> Models { get; set; } = new List<CarModelDto>();
}

public class CarModelDto
{
    public int Id { get; set; }
    public int MakeId { get; set; }
    public string Name { get; set; } = string.Empty;
}
