using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdoptMeNow.Data;
using AdoptMeNow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AdoptMeNow.ViewModels;
using AdoptMeNow.Constants;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore;


namespace AdoptMeNow.Controllers
{
    public class AdoptionController : Controller
    {   
        private readonly UserManager<Users> _userManager;
        private readonly AppDbContext _context;
        public AdoptionController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private bool HasActiveApplication(string userId, int petId)
        {
            return _context.Adoptions.Any(a =>
                a.UserId == userId &&
                a.PetId == petId &&
               (a.Status == "Pending" || a.Status == "Approved"));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AdoptPet(int id)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.PetId == id);

            if (pet == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            
            // Check for ACTIVE application only (Pending or Approved)
            var existingActiveApplication = _context.Adoptions
                .FirstOrDefault(a => a.UserId == userId && a.PetId == id && 
                                    (a.Status == "Pending" || a.Status == "Approved"));
            
            if (existingActiveApplication != null)
            {
                TempData["ErrorMessage"] = $"You have already submitted an application for {pet.Name}. Status: {existingActiveApplication.Status}";
                return RedirectToAction("Index", "Home");
            }

            // Optional: Check for cancelled applications and allow reapplication
            var cancelledApplication = _context.Adoptions
                .FirstOrDefault(a => a.UserId == userId && a.PetId == id && a.Status == "Cancelled");
            
            if (cancelledApplication != null)
            {
                TempData["InfoMessage"] = $"You previously cancelled an application for {pet.Name}. You can apply again.";
            }

            var user = await _userManager.GetUserAsync(User);

            var viewModel = new AdoptPetViewModels
            {
                Pet = pet,
                PetId = pet.PetId, 
                FullName = user.FullName,
                Email = user.Email,
                Age = user.Age,
                Address = user.Address,
                IcNumber = user.IcNumber,
                ContactInfo = user.ContactInfo
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitAdoption(AdoptPetViewModels model)
        {   
            var userId = _userManager.GetUserId(User);

            // Validate UserId
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "You must be logged in to adopt a pet.");
                return RedirectToAction("Login", "Account");
            }
            
             // Check for ACTIVE application only
            var existingActiveApplication = _context.Adoptions
                .FirstOrDefault(a => a.UserId == userId && a.PetId == model.PetId && 
                                    (a.Status == "Pending" || a.Status == "Approved"));
            
            if (existingActiveApplication != null)
            {
                ModelState.AddModelError("", $"You have already submitted an application for this pet. Status: {existingActiveApplication.Status}");
                model.Pet = _context.Pets.FirstOrDefault(p => p.PetId == model.PetId);
                return View("AdoptPet", model);
            }

            //Update the existing cancelled application instead of creating a new one
            var cancelledApplication = _context.Adoptions
                .FirstOrDefault(a => a.UserId == userId && a.PetId == model.PetId && a.Status == "Cancelled");
            
            if (cancelledApplication != null)
            {
                // Update the cancelled application instead of creating a new one
                cancelledApplication.Status = AdoptionStatus.Pending;
                cancelledApplication.ApplicationDate = DateTime.Now;
                cancelledApplication.FullName = model.FullName;
                cancelledApplication.Email = model.Email;
                cancelledApplication.Age = model.Age;
                cancelledApplication.Address = model.Address;
                cancelledApplication.IcNumber = model.IcNumber;
                cancelledApplication.ContactInfo = model.ContactInfo;
                cancelledApplication.HasTimeForPet = model.HasTimeForPet.Value;
                cancelledApplication.IsFinanciallyReady = model.IsFinanciallyReady.Value;
                cancelledApplication.IsEmotionallyReady = model.IsEmotionallyReady.Value;
                cancelledApplication.WillingVetVisits = model.WillingVetVisits.Value;
                cancelledApplication.HasEnoughSpace = model.HasEnoughSpace.Value;
                cancelledApplication.AgreeTerms = model.AgreeTerms;
                
                await _context.SaveChangesAsync();
                return RedirectToAction("Success", new { petId = model.PetId });
            }

            // If no cancelled application, create a new one
            var pet = _context.Pets.FirstOrDefault(p => p.PetId == model.PetId);

            if (pet == null)
                return NotFound();

            if (!ModelState.IsValid)
            {   
                model.Pet = pet;
                return View("AdoptPet", model);
            }

            if (model.HasTimeForPet == null ||
                model.IsFinanciallyReady == null ||
                model.IsEmotionallyReady == null ||
                model.WillingVetVisits == null ||
                model.HasEnoughSpace == null)
            {
                ModelState.AddModelError("", "Please answer all readiness questions.");
                model.Pet = pet;
                return View("AdoptPet", model);
            }

            if (!model.AgreeTerms)
            {
                ModelState.AddModelError("", "You must agree to the terms.");
                model.Pet = pet;
                return View("AdoptPet", model);
            }

            var user = await _userManager.GetUserAsync(User);
            var adoption = new Adoption
            {   
                UserId = userId,
                PetId = model.PetId,
                FullName = model.FullName,
                Email = user.Email,
                Age = model.Age,
                Address = model.Address,
                IcNumber = model.IcNumber,
                ContactInfo = model.ContactInfo,

                HasTimeForPet = model.HasTimeForPet.Value,
                IsFinanciallyReady = model.IsFinanciallyReady.Value,
                IsEmotionallyReady = model.IsEmotionallyReady.Value,
                WillingVetVisits = model.WillingVetVisits.Value,
                HasEnoughSpace = model.HasEnoughSpace.Value,

                AgreeTerms = model.AgreeTerms,
                Status = AdoptionStatus.Pending,
                ApplicationDate = DateTime.Now
            };

            _context.Adoptions.Add(adoption);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success", new { petId = model.PetId });

        }
        
        public IActionResult Success(int petId)
        {   
            System.Diagnostics.Debug.WriteLine("SUBMIT REACHED");            
            var pet = _context.Pets.FirstOrDefault(p => p.PetId == petId);

            if (pet == null)
                return RedirectToAction("Index", "Home");

            return View(pet);
        }

        [HttpGet]
        public IActionResult MyApplication()
        {
            var userId = _userManager.GetUserId(User);

            var model = _context.Adoptions
                .Where(a =>a.UserId == userId)
                .Select(a => new MyApplicationViewModel
                {
                    AdoptionId = a.AdoptionId,
                    Pet = a.Pet,
                    Status = a.Status,
                    ApplicationDate = a.ApplicationDate,
                    PickupDate = a.PickupDate,
                    RejectionReason = a.RejectionReason
                })
                .ToList();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CancelApplication(int id)
        {
            var userId = _userManager.GetUserId(User);

            //find record
            var application = _context.Adoptions
                .Include(a => a.Pet)
                .FirstOrDefault(a=> a.AdoptionId == id && a.UserId == userId);

            if(application == null)
                return NotFound();

            //approved apllication cannot cancel
            if(application.Status != "Pending")
                return BadRequest("Cannot cancel processed application");

            //Remove from database
            //_context.Adoptions.Remove(application);
            //Soft Delete
            application.Status = "Cancelled";
            if (application.Pet != null)
            {
                application.Pet.Status = "Available";
            }
            _context.SaveChanges();

            return RedirectToAction("MyApplication");

        }
    }
}