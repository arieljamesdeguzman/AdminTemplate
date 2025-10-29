using System.Threading.Tasks;
using AdminTemplate.DTOs;

namespace AdminTemplate.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(SignupDto dto);
        Task<UserDto> AuthenticateAsync(string email, string password);
    }
}