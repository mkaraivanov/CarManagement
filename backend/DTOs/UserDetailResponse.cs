namespace Backend.DTOs;

public class UserDetailResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserStatistics Statistics { get; set; } = new();
}

public class UserStatistics
{
    public int VehicleCount { get; set; }
    public int ServiceRecordCount { get; set; }
    public int FuelRecordCount { get; set; }
}
