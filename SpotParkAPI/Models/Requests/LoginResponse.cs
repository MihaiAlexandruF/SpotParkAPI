using SpotParkAPI.Models.Dtos;

namespace SpotParkAPI.Models.Requests
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }

}
