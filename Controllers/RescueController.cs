using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AdoptMeNow.Data;
using AdoptMeNow.Models;
using AdoptMeNow.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AdoptMeNow.Services;


namespace AdoptMeNow.Controllers
{
    [Authorize]
    public class RescueController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RescueController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Report()
        {
            return View();
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reports = _context.RescueReports
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReportedAt)
                .ToList();

            return View(reports);
        }

        [HttpPost]
        public IActionResult Report(RescueReportViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            //Get Rescuer Id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Save image
            string fileName = Path.GetFileNameWithoutExtension(vm.ImageFile.FileName);
            string extension = Path.GetExtension(vm.ImageFile.FileName);
            string newFileName = fileName + "_" + Guid.NewGuid() + extension;

            string path = Path.Combine(_environment.WebRootPath, "uploads/rescue");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fullPath = Path.Combine(path, newFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                vm.ImageFile.CopyTo(stream);
            }

            // Save to database
            var report = new RescueReport
            {
                UserId = userId,
                ImagePath = "/uploads/rescue/" + newFileName,
                Description = vm.Description,
                Location = vm.Location,
                Latitude = vm.Latitude,
                Longitude = vm.Longitude,
                Status = "Pending",
                ReportedAt = DateTime.Now
            };

            _context.RescueReports.Add(report);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var rescue = await _context.RescueReports.FindAsync(id);

            if (rescue == null) return NotFound();

            rescue.Status = "Approved";

            var progress = new RescueProgress
            {
                RescueReportId = rescue.Id,
                Status = "Approved",
                Note = "Rescue request approved.",
                CreatedAt = DateTime.Now
            };
            
            _context.RescueProgresses.Add(progress);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rescue report approved.";
            return RedirectToAction("RescueDetail", "Admin",  new { id = rescue.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id,string rejectReason)
        {
            var rescue = await _context.RescueReports.FindAsync(id);

            if (rescue == null) return NotFound();

            rescue.Status = "Rejected";

            rescue.RejectReason = rejectReason;
            
            var progress = new RescueProgress
            {
                RescueReportId = rescue.Id,
                Status = "Rejected",
                Note = rejectReason,
                CreatedAt = DateTime.Now
            };

            _context.RescueProgresses.Add(progress);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Rescue report rejected.";

            return RedirectToAction("RescueDetail", "Admin", new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(RescueStatusUpdateViewModel vm)
        {
            var rescue = await _context.RescueReports.FindAsync(vm.RescueReportId);
            

            if (rescue == null) 
            {
                return NotFound();
            }

            var currentStatus = rescue.Status;

            var flow = RescueStatusFlow.Flow;

            if (!flow.ContainsKey(currentStatus))
            {
                TempData["Error"] = "Invalid current status.";
                return RedirectToAction("RescueDetail", "Admin", new { id = rescue.Id });
            }

            var allowedNext = flow[currentStatus];

            if (!allowedNext.Contains(vm.Status))
            {
                TempData["Error"] = "You can only move to allowed next status.";
                return RedirectToAction("RescueDetail", "Admin", new { id = rescue.Id });
            }  

             // HANDLE CANCEL FIRST 
            if (vm.Status == "Cancelled")
            {
                rescue.Status = "Cancelled";
                rescue.CancelReason = vm.StatusNote;

                var cancelProgress = new RescueProgress
                {
                    RescueReportId = rescue.Id,
                    Status = "Cancelled",
                    Note = vm.StatusNote,
                    CreatedAt = DateTime.Now
                };

                _context.RescueProgresses.Add(cancelProgress);

                _context.Update(rescue);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Case cancelled successfully.";

                return RedirectToAction("RescueDetail", "Admin", new { id = rescue.Id });
            }

            // Normal flow
            rescue.Status = vm.Status;

            var progress = new RescueProgress
            {
                RescueReportId = rescue.Id,
                Status = vm.Status,
                Note = vm.StatusNote,
                CreatedAt = DateTime.Now
            };
            
            if (vm.StatusImageFile != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.StatusImageFile.FileName);

                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await vm.StatusImageFile.CopyToAsync(stream);
                }

                progress.ImagePath = "/uploads/" + fileName;
            }

            _context.RescueProgresses.Add(progress);
            _context.Update(rescue);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Rescue report status updated successfully.";
            //TempData["HideCurrentImage"] = true;

            return RedirectToAction("RescueDetail", "Admin", new { id = rescue.Id });
        }
        
        [Authorize]
        public IActionResult Progress(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var report = _context.RescueReports
            .Include(r => r.User)
            .Include(r => r.ProgressUpdates)
            .FirstOrDefault(r => r.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }
        
    }
}