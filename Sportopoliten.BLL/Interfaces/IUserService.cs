using Sportopoliten.BLL.DTO.User;

namespace Sportopoliten.BLL.Interfaces
{
    public interface IUserService
    {
        Task<LoginUserDTO?> Login(LoginUserDTO loginDto);
        Task<RegisterUserDTO> Register(RegisterUserDTO registerDto);
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserDTO?> GetUserByEmailAsync(string email);
        Task<UserDTO?> GetUserByLoginAsync(string login);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsLoginUniqueAsync(string login);
        Task<UserDTO?> GetUserProfileAsync(int id);
    }
}
