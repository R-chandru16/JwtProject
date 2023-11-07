using System;
using System.Collections.Generic;

namespace DemoProjectJwtToken.Models
{
    public partial class Trainee
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } = null!;
        public string Phonenumber { get; set; }
    }
}
