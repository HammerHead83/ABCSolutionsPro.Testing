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
            /*** ITS VERY VERY BAD PRACTICE TO RAISE EXCEPTIONS IN CONSTRUCTOR STUBS.
             * IN THIS CASE RUN METHOD IN EACH ACTION. ***/
            /*
             * CheckUserManager();
             */
        }

        private void CheckUserManager()
        {
            if (_userManager == null)
                throw new ApplicationException(@"Oups! UserManager is null, but Controller is mark by [AuthorizeAttribute].
                    System maybe attacking by hackers. WTF!?!?");
        }

        [ProducesResponseType(200)]
        [ActionName("GetUsers")]
        [HttpGet(Name = "GetUsers")]
        public IActionResult GetUsers([FromQuery] string uname)
        {
            CheckUserManager();
            return new ObjectResult(_userManager.Users);
        }

        [ProducesResponseType(200)]
        [ActionName("SetAdminAsync")]
        [HttpPost(Name = "SetAdminAsync")]
        public async Task<IActionResult> SetAdminAsync([FromBody] UID uid)
        {
            CheckUserManager();
            if (!User.IsInRole("Administrator"))
                return Forbid();
            var user = await _userManager.FindByIdAsync(uid.uid);
            if (user != null && !user.IsAdmin)
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
        public async Task<IActionResult> UnsetAdminAsync([FromBody] UID uid)
        {
            CheckUserManager();
            if (!User.IsInRole("Administrator"))
                return Forbid();
            if (uid == null)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(uid.uid);
            if (user == null)
                return BadRequest();
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
        public sealed class UID
        {
            public string uid { get; set; }
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
            CheckUserManager();
            if (!User.IsInRole("Administrator"))
                return Forbid();
            if (data == null || string.IsNullOrEmpty(data.secs) || string.IsNullOrEmpty(data.uid))
                return BadRequest();
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
        public async Task<IActionResult> UnlockUser([FromBody] UID uid)
        {
            CheckUserManager();
            if (!User.IsInRole("Administrator"))
                return Forbid();
            if (uid == null)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(uid.uid);
            if (user == null)
                return BadRequest();
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