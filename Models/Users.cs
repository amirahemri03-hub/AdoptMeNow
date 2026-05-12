using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace AdoptMeNow.Models
{
    public class Users: IdentityUser
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        public string? Address { get; set; }
        public string? IcNumber { get; set; }
        public string? ContactInfo { get; set; }
    }
}