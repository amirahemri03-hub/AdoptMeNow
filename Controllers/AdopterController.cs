using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdoptMeNow.Data;
using AdoptMeNow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdoptMeNow.Controllers
{
    public class AdopterController: Controller
    {
        private readonly UserManager<Users> _userManager;
        public AdopterController(AppDbContext context, UserManager<Users> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account") ;
            }

            return View(user);
        }

        //Edit Profile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Users model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Update the user's profile information
            user.FullName = model.FullName;
            user.Age = model.Age;
            user.Address = model.Address;
            user.IcNumber = model.IcNumber;
            user.ContactInfo = model.ContactInfo;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }
    }
}