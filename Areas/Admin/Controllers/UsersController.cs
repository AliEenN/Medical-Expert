using MedicalExpert.Constants;
using MedicalExpert.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MedicalExpert.Areas.Admin.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.Manager)]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public UsersController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string UserId = claim.Value;

            var adminRole = _context.Roles.Where(e => e.Name == Roles.Admin).FirstOrDefault().Id;
            var admin = _context.UserRoles.Where(e => e.RoleId == adminRole).FirstOrDefault();

            if (User.IsInRole(Roles.Manager))
            {
                return View(await _context.ApplicationUsers.Where(e => e.Id != UserId && e.Id != admin.UserId).ToListAsync());
            }

            return View(await _context.ApplicationUsers.Where(e => e.Id != UserId).ToListAsync());
        }

        // POST
        public async Task<IActionResult> LockUnLock(string id)
        {
            if (id == null)
                return BadRequest();

            var user = await _context.ApplicationUsers.FindAsync(id);

            if (user == null)
                return NotFound();

            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            else
                user.LockoutEnd = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
