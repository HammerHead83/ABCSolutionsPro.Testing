using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PureMVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;

namespace PureMVC.Controllers
{
    [Authorize] [RequireHttps]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class UsersWebApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersWebApiController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [ProducesResponseType(200)]
        [ActionName("GetUsers")]
        [HttpGet("{uname}", Name = "GetUsers")]
        public IActionResult GetUsers([FromQuery] string uname)
        {
#if DEBUG
            return new ObjectResult(_userManager.Users);
#else
            if (string.IsNullOrEmpty(uname))
                return BadRequest();
            return new ObjectResult(_userManager.Users.SkipWhile(u=>u.Email == uname));
#endif

        }

        [ProducesResponseType(200)]
        [ActionName("SetAdminAsync")]
        [HttpPost(Name = "SetAdminAsync")]
        public async Task<IActionResult> SetAdminAsync([FromBody] ApplicationUser user)
        {
            if (!User.IsInRole("Administrator"))
                return Forbid();
            if (!user.IsAdmin)
            {
                var setAdminResult = await _userManager.AddToRoleAsync(user, "Administrator");
                if (setAdminResult.Succeeded)
                {
                    user.IsAdmin = true;
                    await _userManager.UpdateAsync(user);
                    return Ok();
                }
                else
                    return BadRequest();
            }
            return Ok();
        }

        [ProducesResponseType(200)]
        [ActionName("UnsetAdminAsync")]
        [HttpPost(Name = "UnsetAdminAsync")]
        public async Task<IActionResult> UnsetAdminAsync([FromBody] ApplicationUser user)
        {
            if (!User.IsInRole("Administrator"))
                return Forbid();
            if ((await _userManager.GetUsersInRoleAsync("Administrator")).Count < 2 || !user.IsAdmin)
                return BadRequest();
            var unsetAdminResult = await _userManager.RemoveFromRoleAsync(user, "Administrator");
            if (unsetAdminResult.Succeeded)
            {
                user.IsAdmin = false;
                await _userManager.UpdateAsync(user);
                return Ok();
            }
            return BadRequest();
        }

        [Serializable]
        public sealed class LockData
        {
            public string uid { get; set; }
            public string secs { get; set; }
        }

        [ProducesResponseType(200)]
        [ActionName("LockUser")]
        [HttpPost(Name = "LockUser")]
        public async Task<IActionResult> LockUser([FromBody] LockData data)
        {
            if (data == null || string.IsNullOrEmpty(data.secs) || string.IsNullOrEmpty(data.uid))
            {
                return BadRequest();
            }
            double dSecs = default(double);
            if (!double.TryParse(data.secs, out dSecs))
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(data.uid);
            if (await _userManager.GetLockoutEnabledAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddSeconds(dSecs));
                user.IsLocked = true;
                var updateUserResult = await _userManager.UpdateAsync(user);
                if (updateUserResult.Succeeded)
                    return Ok();
            }
            return BadRequest();
        }

        [ProducesResponseType(200)]
        [ActionName("UnlockUser")]
        [HttpPost(Name = "UnlockUser")]
        public async Task<IActionResult> UnlockUser([FromBody] string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(uid);
            if (await _userManager.GetLockoutEnabledAsync(user) && await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddDays(-1));
                user.IsLocked = false;
                var updateUserResult = await _userManager.UpdateAsync(user);
                if (updateUserResult.Succeeded)
                    return Ok();
            }
            return BadRequest();
        }
    }
}