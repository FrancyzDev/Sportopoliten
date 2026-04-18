using Sportopoliten.BLL.DTO.User;
using System.Security.Cryptography;
using System.Text;

namespace Sportopoliten.BLL.Interfaces
{
    public interface IUserService
    {
        Task<LoginUserDTO?> Login(LoginUserDTO loginDto);
        Task<RegisterUserDTO> Register(RegisterUserDTO registerDto);
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserDTO?> GetUserByEmailAsync(string email);
        Task<UserDTO?> GetUserByLoginAsync(string login);
        Task DeleteUserAsync(int id);
        Task UpdateUserProfileAsync(int userId, EditProfileDTO model);
        Task ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsLoginUniqueAsync(string login);
        Task<UserDTO?> GetUserProfileAsync(int id);
    }
}
