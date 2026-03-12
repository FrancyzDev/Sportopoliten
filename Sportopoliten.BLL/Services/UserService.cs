using Sportopoliten.BLL.DTO;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;
using Sportopoliten.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Sportopoliten.BLL.Services
{
    public class UserService : IUserService<RegisterUserDto>
    {
        IUnitOfWork Database { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<RegisterUserDto> Get(int id)
        {
            var user = await Database.Users.GetByIdAsync(id);
            return user != null ? new RegisterUserDto { Email = user.Login } : null;
        }

        public async Task Create(RegisterUserDto userDTO)
        {
            string salt = GenerateSalt();
            string hashedPassword = HashPassword(userDTO.Password, salt);

            var user = new User
            {
                Email = userDTO.Email, 
                PasswordHash = hashedPassword,
                Salt = salt
            };

            await Database.Users.AddAsync(user);
            await Database.SaveChangesAsync();
        }

        //public async Task<RegisterUserDto?> Login(RegisterUserDto userDTO)
        //{
        //    var user = await Database.Users.FindAsync(userDTO.Email);
        //    if (user == null) return null;

        //    var hashedPassword = HashPassword(userDTO.Password, user.Salt);

        //    if (user.Password == hashedPassword)
        //    {
        //        return new RegisterUserDto
        //        {
        //            Email = user.Login
        //        };
        //    }
        //    return null;
        //}
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

        public async Task<bool> IsUnique(string login)
        {
            var users = await Database.Users.GetAll();
            return !users.Any(u => u.Login.ToLower() == login.ToLower());
        }
    }
}
