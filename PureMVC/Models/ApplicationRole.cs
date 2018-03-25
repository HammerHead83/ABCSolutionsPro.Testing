using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PureMVC.Models
{
    public class ApplicationRole : IdentityRole
    {
        private static readonly ApplicationRole Implementation = null;
        static ApplicationRole()
        {
            Implementation = new ApplicationRole();
            
        }
    }
}
