using Crime_Management_System.DTOs;
using Crime_Management_System.Models;

namespace Crime_Management_System.Services.Interfaces
{
    public interface IUserService
    {

        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
       // Task<UserResponseDto> GetUserByIdAsync(int id);
        Task<User?> GetUserByIdAsync(int id);


        Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto, string createdByAdmin);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task AssignRoleAndClearanceAsync(int id, UserRole role, int clearanceLevel);

        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        bool ValidatePassword(string password, string passwordHash, string salt);


       
    }
}
