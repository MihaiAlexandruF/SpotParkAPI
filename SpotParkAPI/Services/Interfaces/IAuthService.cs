using SpotParkAPI.Models.Requests;

namespace SpotParkAPI.Services.Interfaces
{
    public interface IAuthService
    {  
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
    }
}
