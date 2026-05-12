using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdoptMeNow.Data;
using AdoptMeNow.Models;
using AdoptMeNow.Services;
using AdoptMeNow.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;

namespace AdoptMeNow.Controllers
{
    [Authorize(Roles  = "Admin")]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        public AdminController(AppDbContext context, IWebHostEnvironment environment, EmailService emailService)
        {
            _context = context;
            _environment = environment;
            _emailService = emailService;
        }

        public IActionResult Dashboard()
        {
            var pets = _context.Pets;
            var adoptions = _context.Adoptions;
            var rescues = _context.RescueReports
                .Where(r=> r.Latitude !=null && r.Longitude !=null)
                .ToList();


            var  now = DateTime.Now;

            var rawAdoptions = adoptions
                .GroupBy(a => new { a.ApplicationDate.Year, a.ApplicationDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Total = g.Count()
                })
                .ToList();


            var model = new DashboardViewModel
            {
                TotalPets = pets.Count(),
                AvailablePets = pets.Count(p => p.Status == "Available"),
                AdoptedPets = pets.Count(p => p.Status == "Adopted"),
                PendingApplications = adoptions.Count(a => a.Status == "Pending"),
                TotalAdoptions = adoptions.Count(),
                TotalRescueReports = rescues.Count(),
                RescueReports = rescues,

                /*AdoptionsThisMonth = adoptions
                    .Count(a => 
                    a.ApplicationDate.Month == now.Month && 
                    a.ApplicationDate.Year == now.Year ),*/

                MonthlyAdoptions = rawAdoptions
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .Select(x => new MonthlyData
                    {
                        Label = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy"),
                        Total = x.Total
                    })
                    .ToList() 

                /*MonthlyAdoptions = new List<MonthlyData>
                {
                    new MonthlyData { Label = "Jan 2026", Total = 2 },
                    new MonthlyData { Label = "Feb 2026", Total = 5 },
                    new MonthlyData { Label = "Mar 2026", Total = 3 },
                    new MonthlyData { Label = "Apr 2026", Total = 8 },
                    new MonthlyData { Label = "May 2026", Total = 4 },
                }*/
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Export(ExportRequest request)
        {
            var from = request.FromDate ?? DateTime.Now.AddMonths(-1); //start date
            var to = (request.ToDate ?? DateTime.Now).Date.AddDays(1); //end date
            
            var data = _context.Adoptions
                .Include(a => a.Pet)
                .Where(a => a.ApplicationDate >= from && a.ApplicationDate <= to)
                .ToList();

            if (request.Format == "PDF")
                return ExportAdoptionPdf(data, from, to);
            
            if(request.Format == "Excel")
                return ExportAdoptionExcel(data);

            return BadRequest(); 
        }

        public IActionResult ExportAdoptionExcel(List<Adoption> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Adoptions");
            
            
            worksheet.SheetView.FreezeRows(1);

            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Cell(1, 1).Value = "Adopter Name";
            worksheet.Cell(1, 2).Value = "Email";
            worksheet.Cell(1, 3).Value = "Contact Number";
            worksheet.Cell(1, 4).Value = "IC Number";
            worksheet.Cell(1, 5).Value = "Address";
            worksheet.Cell(1, 6).Value = "Pet Name";
            worksheet.Cell(1, 7).Value = "Status";
            worksheet.Cell(1, 8).Value = "Application Date";
            worksheet.Cell(1, 9).Value = "Pickup Date";
            
            var header = worksheet.Row(1);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.DarkGray;
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = data[i].FullName;
                worksheet.Cell(i + 2, 2).Value = data[i].Email;
                worksheet.Cell(i + 2, 3).Value = data[i].ContactInfo;
                worksheet.Cell(i + 2, 4).Value = data[i].IcNumber;
                worksheet.Cell(i + 2, 5).Value = data[i].Address;
                worksheet.Cell(i + 2, 6).Value = data[i].Pet?.Name ?? "-";
                worksheet.Cell(i + 2, 7).Value = data[i].Status;
                worksheet.Cell(i + 2, 8).Value = data[i].ApplicationDate;
                worksheet.Cell(i + 2, 8).Style.DateFormat.Format = "dd MMM yyyy";
                worksheet.Cell(i + 2, 9).Value = data[i].PickupDate;
                worksheet.Cell(i + 2, 9).Style.DateFormat.Format = "dd MMM yyyy";
            }

            //colum resize
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "AdoptionReport.xlsx");

        }


        private IActionResult ExportAdoptionPdf(List<Adoption> data, DateTime from, DateTime to)
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    // ===== HEADER =====
                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text("ADOPT ME NOW")
                            .FontSize(18).Bold();

                        col.Item().AlignCenter().Text("Adoption Report")
                            .FontSize(12);

                        col.Item().PaddingTop(5).LineHorizontal(1);
                    });

                    // ===== CONTENT =====
                    page.Content().Column(col =>
                    {
                        col.Spacing(12);


                        // ===== REPORT INFO BOX =====
                        col.Item().Background("#F2F2F2").Padding(10).Column(info =>
                        {
                            info.Item().Text($"Report Period: {from:dd MMM yyyy} - {to:dd MMM yyyy}");
                            info.Item().Text($"Generated on: {DateTime.Now:dd MMM yyyy}");
                        });

                        // ===== SUMMARY =====
                        int total = data.Count;
                        int approved = data.Count(x => x.Status == "Approved");
                        int pending = data.Count(x => x.Status == "Pending");
                        int rejected = data.Count(x => x.Status == "Rejected");

                        col.Item().Text("Summary").Bold().FontSize(12);

                        col.Item().Row(row =>
                        {
                            row.Spacing(10);

                            void SummaryBox(IContainer container, string title, string value)
                            {
                                container.Background("#EAEAEA")
                                    .Padding(10)
                                    .Column(c =>
                                    {
                                        c.Item().Text(title).FontSize(10);
                                        c.Item().Text(value).Bold().FontSize(14);
                                    });
                            }

                            row.RelativeItem().Element(c => SummaryBox(c, "Total", total.ToString()));
                            row.RelativeItem().Element(c => SummaryBox(c, "Approved", approved.ToString()));
                            row.RelativeItem().Element(c => SummaryBox(c, "Pending", pending.ToString()));
                            row.RelativeItem().Element(c => SummaryBox(c, "Rejected", rejected.ToString()));
                        });


                        // ===== DETAILS TABLE =====
                        col.Item().PaddingTop(15).Text("Adoption Details")
                            .Bold().FontSize(12);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(25);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // HEADER
                            table.Header(header =>
                            {
                                void HeaderCell(string text) =>
                                    header.Cell().Background("#D6D6D6").Padding(5).Text(text).Bold();

                                HeaderCell("#");
                                HeaderCell("Name");
                                HeaderCell("Pet");
                                HeaderCell("Status");
                                HeaderCell("Application");
                                HeaderCell("Pickup");
                            });

                            int index = 1;

                            foreach (var item in data)
                            {
                                table.Cell().Padding(5).Text(index++.ToString());
                                table.Cell().Padding(5).Text(item.FullName);
                                table.Cell().Padding(5).Text(item.Pet?.Name ?? "-");
                                table.Cell().Padding(5).Text(item.Status);
                                table.Cell().Padding(5).Text(item.ApplicationDate.ToString("dd MMM yyyy"));
                                table.Cell().Padding(5).Text(item.PickupDate.HasValue
                                    ? item.PickupDate.Value.ToString("dd MMM yyyy")
                                    : "-");
                            }
                        });

                        // ===== TOTAL / FOOTER BOX =====
                        col.Item().PaddingTop(10).Background("#F5F5F5").Padding(10).Row(row =>
                        {
                            row.RelativeItem().Text("Ending Balance Style Summary")
                                .Bold();

                            row.RelativeItem().AlignRight().Text($"{total} Applications")
                                .Bold();
                        });

                        // ===== NOTES =====
                        col.Item().PaddingTop(10).Text("Additional Notes")
                            .Bold().FontSize(11);

                        col.Item().Text("• This report summarizes adoption activity within the selected period.")
                            .FontSize(10);

                        col.Item().Text("• Data includes approved, pending, and rejected applications.")
                            .FontSize(10);
                    });

                    // ===== FOOTER =====
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.DefaultTextStyle(t => t.FontSize(10));

                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();

            Response.Headers.Add("Content-Disposition", "inline; filename=AdoptionReport.pdf");
            return File(pdf, "application/pdf");
        }

        public IActionResult ManagePets()
        {
            var pets = _context.Pets.OrderBy(pet => pet.PetId).ToList();
            return View(pets);
        }

        public IActionResult AddPet()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(PetDto petDto)
        {
            if (!ModelState.IsValid)
            {
                return View("AddPet", petDto);
            }

            string imagePath = "";

            if(petDto.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "Images/pets");

                if(!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(petDto.ImageFile.FileName);

                string filePath = Path.Combine(uploadsFolder,fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    petDto.ImageFile.CopyTo(stream);
                }

                imagePath = "/Images/pets/" + fileName;
            }
            //Add New pet
            var pet = new Pet
            {
                Name = petDto.Name,
                Breed = petDto.Breed,
                Category = petDto.Category,
                Age = petDto.Age,
                Gender = petDto.Gender,
                Description = petDto.Description,
                ImageUrl = imagePath,
                Status = "Available"
            };
            
            _context.Pets.Add(pet);
            _context.SaveChanges();

            return RedirectToAction("ManagePets");
        }

        [HttpGet]
        public IActionResult EditPet(int id)
        {
           var pet = _context.Pets.Find(id);

           if(pet == null)
            {
                return RedirectToAction("ManagePets");
            }

             var petDto = new PetDto
            {
                PetId =pet.PetId,
                Name = pet.Name,
                Breed = pet.Breed,
                Category = pet.Category,
                Age = pet.Age,
                Gender = pet.Gender,
                Description = pet.Description,
                ImageUrl = pet.ImageUrl
            };

            return View(petDto);

        }
        
        [HttpPost]
        public IActionResult SaveEdit(int id, PetDto petDto)
        {   
            
            var pet = _context.Pets.Find(petDto.PetId);//2
            if (pet == null)
            {
                return RedirectToAction("ManagePets");
            }

            if (!ModelState.IsValid)
            {
                return View("EditPet", petDto);
            }

                //Save New pet data
                pet.Name = petDto.Name;
                pet.Breed = petDto.Breed;
                pet.Category = petDto.Category;
                pet.Age = petDto.Age;
                pet.Gender = petDto.Gender;
                pet.Description = petDto.Description;

                //ONLY update image if new file uploaded
                if(petDto.ImageFile !=null)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Images/pets");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(petDto.ImageFile.FileName);
                string filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    petDto.ImageFile.CopyTo(stream);
                }

                pet.ImageUrl = "/Images/pets/" + fileName;

                }
        
            _context.SaveChanges();
        
            return RedirectToAction("ManagePets");
        }

        public IActionResult Delete(int id)
        {
            var pet = _context.Pets.Find(id);
            if(pet != null)
            {
                _context.Pets.Remove(pet);
                _context.SaveChanges();
            }

            return RedirectToAction("ManagePets");
        }

        public IActionResult AdoptionProgress()
        {
            var data = _context.Adoptions
            .AsNoTracking()
            .Include(a => a.Pet)
            .Where(a => a.Status != "Cancelled"
                    && a.UserId != null 
                    && a.UserId != ""  // Filter out empty UserId
                    && a.UserId.Length > 10)  // Only valid GUIDs)
            .OrderByDescending(a => a.ApplicationDate)
            .ToList()
            .GroupBy(a => new { a.UserId, a.PetId })
            .Select(g => g.OrderByDescending(a => a.ApplicationDate).First())
            .Select(a => new AdoptionProgressViewModels
            {
                AdoptionId = a.AdoptionId,
                FullName = a.FullName,
                ContactInfo = a.ContactInfo,
                PetName = a.Pet.Name,
                Age = a.Age,
                Address = a.Address,
                IcNumber = a.IcNumber,
                ApplicationDate = a.ApplicationDate,
                PickupDate = a.PickupDate,
                Status = a.Status
            })
            .ToList();
        
        return View(data);
        }

        public IActionResult AdoptionDetail(int id)
        {
            var data = _context.Adoptions
                .Include(a => a.Pet)
                .FirstOrDefault(a => a.AdoptionId == id);
            
            if (data == null) return NotFound();

            var vm = new AdoptionDetailViewModel
            {
                AdoptionId = data.AdoptionId,
                FullName = data.FullName,
                ContactInfo = data.ContactInfo,
                PetName = data.Pet.Name,
                Age = data.Age,
                Address = data.Address,
                IcNumber = data.IcNumber,
                ApplicationDate = data.ApplicationDate,

                // assessment
                HasTimeForPet = data.HasTimeForPet ,
                IsFinanciallyReady = data.IsFinanciallyReady ,
                IsEmotionallyReady = data.IsEmotionallyReady ,
                WillingVetVisits = data.WillingVetVisits ,
                HasEnoughSpace = data.HasEnoughSpace ,

                // management
                Status = data.Status,
                RejectionReason = data.RejectionReason,
                PickupDate = data.PickupDate
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> SearchAdoptions(string search)
        {
            var keyword = search?.ToLower();

            var query = _context.Adoptions
                .AsNoTracking()
                .Include(a => a.Pet)
                .Where(a => a.Status != "Cancelled");

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a =>
                    a.FullName.ToLower().Contains(keyword) ||
                    a.ContactInfo.ToLower().Contains(keyword) ||
                    a.Pet.Name.ToLower().Contains(keyword) ||
                    a.Status.ToLower().Contains(keyword)
                );
            }

            var data = await query
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();

            var model = data
                .GroupBy(a => new { a.UserId, a.PetId })
                .Select(g => g.OrderByDescending(a => a.ApplicationDate).First())
                .Select(a => new AdoptionProgressViewModels
                {
                    AdoptionId = a.AdoptionId,
                    FullName = a.FullName,
                    ContactInfo = a.ContactInfo,
                    PetName = a.Pet.Name,
                    Age = a.Age,
                    Address = a.Address,
                    IcNumber = a.IcNumber,
                    ApplicationDate = a.ApplicationDate,
                    PickupDate = a.PickupDate,
                    Status = a.Status
                })
                .ToList();

            return PartialView("_PetTable", model);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAdoption(int id)
        {
            var adoption = _context.Adoptions.Find(id);

            if (adoption != null)
            {
                _context.Adoptions.Remove(adoption);
                _context.SaveChanges();
            }

            return RedirectToAction("AdoptionProgress");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(AdoptionDetailViewModel model)
        {
            var data = _context.Adoptions
            .Include(a => a.Pet)
            .FirstOrDefault(a => a.AdoptionId == model.AdoptionId);

            if (data == null) return NotFound();

            // Validate approve
            if (model.Status == "Approved" && model.PickupDate == null)
            {
                ModelState.AddModelError("PickupDate", "Pickup date is required when approving an application.");

                // RELOAD SAME VIEW WITH DATA (NOT REDIRECT)
                var vm = new AdoptionDetailViewModel
                {
                    AdoptionId = data.AdoptionId,
                    FullName = data.FullName,
                    ContactInfo = data.ContactInfo,
                    PetName = data.Pet.Name,
                    Age = data.Age,
                    Address = data.Address,
                    IcNumber = data.IcNumber,
                    ApplicationDate = data.ApplicationDate,

                    HasTimeForPet = data.HasTimeForPet,
                    IsFinanciallyReady = data.IsFinanciallyReady,
                    IsEmotionallyReady = data.IsEmotionallyReady,
                    WillingVetVisits = data.WillingVetVisits,
                    HasEnoughSpace = data.HasEnoughSpace,

                    Status = data.Status,
                    RejectionReason = data.RejectionReason,
                    PickupDate = data.PickupDate
                };

                return View("AdoptionDetail", vm);
            }

            data.Status = model.Status;

           //Rejected Status
             if (model.Status == "Rejected")
            {
                data.PickupDate = null;

                // Save reason 
                data.RejectionReason = model.RejectionReason;

                _emailService.SendEmail(
                    data.Email,
                    "Adoption Update ❌",
                    $"Hello {data.FullName},<br/><br/>" +
                    $"We are sorry to inform you that your application for <b>{data.Pet.Name}</b> was not approved.<br/><br/>" +
                    $"Reason: {data.RejectionReason}<br/><br/>" +
                    $"Thank you for your interest in adopting a pet 🐾"
                );
            }

            //APPROVED STATUS
            else if (model.Status == "Approved")
            {
                data.PickupDate = model.PickupDate;
                data.RejectionReason = null;

                data.Pet.Status = "Adopted";

                var others = _context.Adoptions
                    .Where(a => a.PetId == data.PetId && a.AdoptionId != data.AdoptionId);

                foreach (var app in others)
                {
                    app.Status = "Rejected";
                    app.PickupDate = null;
                    app.RejectionReason = "Another application was approved for this pet.";
                }

                _emailService.SendEmail(
                    data.Email,
                    "Adoption Approved 🎉",
                    $"Hello {data.FullName},<br/><br/>" +
                    $"Great news! Your adoption request for <b>{data.Pet.Name}</b> has been <b>approved</b>.<br/><br/>" +
                    $"Pickup Date: {data.PickupDate:dd/MM/yyyy}<br/><br/>" +
                    $"Thank you for giving a pet a loving home 🐾"
                );
            }

            //Calcelled/Other status
            else if (model.Status == "Cancelled")
            {
                data.PickupDate = null;
                data.Pet.Status = "Available";
            }

            _context.SaveChanges();

            return RedirectToAction("AdoptionProgress");
        }

        public IActionResult ManageRescue()
        {
            var reports = _context.RescueReports
            .Include(r => r.User)
            .Include(r => r.ProgressUpdates)
            .OrderByDescending(x => x.ReportedAt)
            .ToList();

            return View(reports);
        }

        [HttpGet]
        public IActionResult GetRescueStats()
        {
            var reports = _context.RescueReports.ToList();

            var totalReports = reports.Count;

            var pending = reports.Count(r =>
                r.Status == "Pending"
            );

            var active = reports.Count(r =>
                r.Status == "Approved" ||
                r.Status == "Rescued" ||
                r.Status == "Recovering"
            );

            var adoption = reports.Count(r =>
                r.Status == "For Adoption"
            );

            var closed = reports.Count(r =>
                r.Status == "Cancelled" ||
                r.Status == "Rejected"
            );

            return Json(new
            {
                totalReports,
                pending,
                active,
                adoption,
                closed
            });
        }

       public IActionResult RescueDetail(int id)
        {
            var report = _context.RescueReports
                .Include(r => r.User)
                .Include(r => r.ProgressUpdates)
                .FirstOrDefault(r => r.Id == id);

            if (report == null)
                return NotFound();

            var status = report.Status?.Trim();

            ViewBag.StatusFlow =
                RescueStatusFlow.Flow.ContainsKey(status)
                ? RescueStatusFlow.Flow[status]
                : new List<string>();

            return View(report);
        }

        

    }
}