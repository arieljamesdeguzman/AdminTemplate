using AdminTemplate.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AdminTemplate.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task SaveChangesAsync();
        Task<User> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);

        // ✅ ADD THIS METHOD
        Task<List<User>> GetAllUsersWithCoordinatesAsync();
    }
}