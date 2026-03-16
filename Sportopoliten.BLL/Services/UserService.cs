using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.DTO.User;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Sportopoliten.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ShopDbContext _context;

        public UserService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterUserDTO> Register(RegisterUserDTO registerDto)
        {
            if (!await IsEmailUniqueAsync(registerDto.Email))
                throw new InvalidOperationException("Email уже используется");

            if (!await IsLoginUniqueAsync(registerDto.Login))
                throw new InvalidOperationException("Логин уже используется");

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(registerDto.Password, salt);

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Login = registerDto.Login,
                Phone = registerDto.Phone,
                PasswordHash = hashedPassword,
                Salt = salt,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow,
                Cart = new Cart() // Создаем корзину для нового пользователя
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterUserDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                Login = user.Login,
                Phone = user.Phone
            };
        }

        public async Task<LoginUserDTO?> Login(LoginUserDTO loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Login == loginDto.LoginOrEmail ||
                    u.Email == loginDto.LoginOrEmail);

            if (user == null)
                return null;

            var hashedPassword = HashPassword(loginDto.Password, user.Salt ?? string.Empty);

            if (user.PasswordHash == hashedPassword)
            {
                return new LoginUserDTO
                {
                    LoginOrEmail = user.Login ?? user.Email,
                    Password = loginDto.Password,
                    RememberMe = loginDto.RememberMe
                };
            }

            return null;
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Cart)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return null;

            return MapToDTO(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Cart)
                .ToListAsync();

            return users.Select(MapToDTO);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsLoginUniqueAsync(string login)
        {
            return !await _context.Users.AnyAsync(u => u.Login == login);
        }

        public async Task<UserDTO?> GetUserProfileAsync(int id)
        {
            return await GetUserByIdAsync(id);
        }

        public async Task ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("Пользователь не найден");

            var oldPasswordHash = HashPassword(oldPassword, user.Salt ?? string.Empty);

            if (user.PasswordHash != oldPasswordHash)
                throw new InvalidOperationException("Неверный старый пароль");

            user.Salt = GenerateSalt();
            user.PasswordHash = HashPassword(newPassword, user.Salt);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Cart)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException("Пользователь не найден");

            // Удаляем корзину пользователя
            if (user.Cart != null)
            {
                _context.Carts.Remove(user.Cart);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            return user != null ? MapToDTO(user) : null;
        }

        public async Task<UserDTO?> GetUserByLoginAsync(string login)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            return user != null ? MapToDTO(user) : null;
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
            return Convert.ToHexString(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(salt + password);
            byte[] byteHash = MD5.HashData(bytes);
            return Convert.ToHexString(byteHash);
        }

        private UserDTO MapToDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Phone = user.Phone,
                Login = user.Login,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                CreatedAt = user.CreatedAt,
                CartId = user.Cart.Id
            };
        }
    }
}