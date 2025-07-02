using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services.Helpers;

namespace SpotParkAPI.Services.Interfaces
{
    public interface IAuthService
    {
       Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);

        Task<bool> RegisterAsync(RegisterRequest request);
        Task<UserValidationDto> GetUserValidationDtoAsync(int userId);


    }
}
