
﻿using System;
using System.Collections.Generic;
using System.Text;

﻿namespace Sportopoliten.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Login { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
        public Cart? Cart { get; set; }
    }
}
