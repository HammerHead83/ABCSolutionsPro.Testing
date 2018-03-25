using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PureMVC.Models;

namespace PureMVC.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
#if DEBUG
        public IActionResult Index()
#else
        public async Task<IActionResult> Index()
#endif
        {
            IEnumerable<ApplicationUser> users;
#if DEBUG
            users = _userManager.Users;
#else
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            users = _userManager.Users.Where(u => u.Id != currentUser.Id);
#endif
            return View(users);
        }
    }
}