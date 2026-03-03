using System;
using System.Collections.Generic;
using System.Text;

namespace Sportopoliten.BLL.DTO
{
    public class RegisterUserDto
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
