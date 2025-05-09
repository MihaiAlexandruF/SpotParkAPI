using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;

namespace SpotParkAPI.Services.Interfaces
{
    public interface IAuthService
    {  
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<UserValidationDto> GetUserValidationDtoAsync(int userId);


    }
}
