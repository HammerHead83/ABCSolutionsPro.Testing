using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PureMVC.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MidName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLocked { get; set; }
        public override string UserName { get =>
                string.IsNullOrWhiteSpace((FirstName + LastName + MidName).Trim()) ?
                (FirstName + " " + LastName + " " + MidName).Trim() : Email;
        }

    }
}
