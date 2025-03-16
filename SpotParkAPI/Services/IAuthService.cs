using SpotParkAPI.Models.Requests;

namespace SpotParkAPI.Services
{
    public interface IAuthService
    {  
        Task<string> LoginAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
    }
}
