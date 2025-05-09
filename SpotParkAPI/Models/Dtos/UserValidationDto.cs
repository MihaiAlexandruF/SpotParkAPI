public class UserValidationDto
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public decimal Balance { get; set; }
    public List<UserVehicleDto> Vehicles { get; set; } = new List<UserVehicleDto>();
}

public class UserVehicleDto
{
    public int Id { get; set; }
    public string PlateNumber { get; set; }
}
