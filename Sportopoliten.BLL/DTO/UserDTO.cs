using System;
using System.Collections.Generic;
using System.Text;

namespace Sportopoliten.BLL.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }


        public int CartId { get; set; }
        public CartDTO Cart { get; set; }
    }
}
