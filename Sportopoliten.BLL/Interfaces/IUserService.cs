using Sportopoliten.BLL.DTO.Category;
using Sportopoliten.BLL.DTO.User;
using Sportopoliten.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Sportopoliten.BLL.Interfaces
{
    public interface IUserService
    {
        Task<LoginUserDTO?> Login(LoginUserDTO loginDto);
        Task<RegisterUserDTO> Register(RegisterUserDTO registerDto);
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsLoginUniqueAsync(string login);
        Task<UserDTO?> GetUserProfileAsync(int id);
    }
}
