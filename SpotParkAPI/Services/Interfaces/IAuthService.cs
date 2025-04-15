using SpotParkAPI.Models.Requests;

namespace SpotParkAPI.Services.Interfaces
{
    public interface IAuthService
    {  
        Task<string> LoginAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
    }
}
