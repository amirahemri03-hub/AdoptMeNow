using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdoptMeNow.Data;
using AdoptMeNow.Models;
using AdoptMeNow.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AdoptMeNow.Controllers
{
    public class SavedPetsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public SavedPetsController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Toggle([FromBody] SavedPetsDto request) //3)frm body read json, asp.net auto convert json to c#
        {
            var userId = _userManager.GetUserId(User);

            if(userId == null)
            {
                return Unauthorized();
            }

            var existing = await _context.SavedPets
                .FirstOrDefaultAsync(s => s.UserId == userId && s.PetId == request.PetId);

            if(existing != null)
            {
                _context.SavedPets.Remove(existing);
                await _context.SaveChangesAsync();

                return Json(new { saved = false});
            }

            var save = new SavedPets
            {
                UserId = userId,
                PetId = request.PetId
            };

            _context.SavedPets.Add(save);
            await _context.SaveChangesAsync();

            return Json(new { saved = true});
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

             // get saved pets + include Pet data
            var saved = await _context.SavedPets
                .Where(s => s.UserId == userId)
                .Include(s => s.Pet)
                .ToListAsync();

            // get adoption info (for status display)
            var userAdoptions = await _context.Adoptions
                .Where(a => a.UserId == userId)
                .ToListAsync();

            // convert to your existing ViewModel
            var model = saved.Select(s => new PetViewModel
            {
                Pet = s.Pet,
                IsSaved = true, // always true here

                HasPendingApplication = userAdoptions
                    .Any(a => a.PetId == s.Pet.PetId && a.Status == "Pending"),

                ApplicationStatus = userAdoptions
                    .Where(a => a.PetId == s.Pet.PetId)
                    .OrderByDescending(a => a.ApplicationDate)
                    .Select(a => a.Status)
                    .FirstOrDefault()
            }).ToList();

            return View(model);
        }
    }
}