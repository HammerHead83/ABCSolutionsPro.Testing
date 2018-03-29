using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PureMVC.Data;
using PureMVC.Models;
using PureMVC.Models.MsgsViewModels;

namespace PureMVC.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Services.IEmailSender _sender;

        public MessagesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context,
            Services.IEmailSender sender)
        {
            _context = context;
            _userManager = userManager;
            _sender = sender;
        }
        
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return View(await _context.UserMessages.Where(msg=>msg.ToUser == user.Id).ToListAsync());
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FromUser,ToUser,IsEmail,Body")] UserMessageModel userMessageModel)
        {
            if (ModelState.IsValid)
            {
                userMessageModel.Id = Guid.NewGuid();
                var toEmail = userMessageModel.ToUser;
                userMessageModel.FromUser = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                userMessageModel.ToUser = (await _userManager.FindByNameAsync(userMessageModel.ToUser)).Id;
                // Check wether message doesn't be sent to myself
                if (!userMessageModel.FromUser.Equals(userMessageModel.ToUser))
                {
                    _context.Add(userMessageModel);
                    await _context.SaveChangesAsync();
                    if (userMessageModel.IsEmail && _sender != null)
                    {
                        var bodyBuilder = new System.Text.StringBuilder();
                        bodyBuilder.AppendFormat("<h2>You have new message from {0}</h2>.<br />", User.Identity.Name);
                        bodyBuilder.AppendFormat("<div class=\"message\">{0}</div><br /><br />", userMessageModel.Body);
                        bodyBuilder.AppendFormat("<div class=\"footer\">Pure MVC {0}</div>", DateTime.UtcNow.Year);
                        await _sender.SendEmailAsync(toEmail, "Pure MVC user messages subsystem.", bodyBuilder.ToString());
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userMessageModel);
        }
        
        
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userMessageModel = await _context.UserMessages
                .SingleOrDefaultAsync(m => m.Id == id);
            if (userMessageModel == null)
            {
                return NotFound();
            }

            return View(userMessageModel);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userMessageModel = await _context.UserMessages.SingleOrDefaultAsync(m => m.Id == id);
            _context.UserMessages.Remove(userMessageModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserMessageModelExists(Guid id)
        {
            return _context.UserMessages.Any(e => e.Id == id);
        }
    }
}
