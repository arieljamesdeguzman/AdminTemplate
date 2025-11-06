using AdminTemplate.Models;
using AdminTemplate.Repositories;
using AdminTemplate.Services;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AdminTemplate.DTOs;

namespace AdminTemplate.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterAsync(SignupDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                return false; // already exists

            var user = new User
            {
                FullName = $"{dto.FirstName} {dto.LastName}",
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = HashPassword(dto.Password),
                AuthToken = Guid.NewGuid().ToString(),
                // ✅ ADD THESE TWO LINES
                Address = dto.Address,
                Coordinates = dto.Coordinates
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task<UserDto> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            user.AuthToken = Guid.NewGuid().ToString();
            await _userRepository.UpdateUserAsync(user);

            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AuthToken = user.AuthToken,
                // ✅ ADD THESE TWO LINES
                Address = user.Address,
                Coordinates = user.Coordinates
            };
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedInput = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
                return hashedInput == storedHash;
            }
        }
    }
}