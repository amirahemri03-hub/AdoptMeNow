using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdoptMeNow.Models;
using Microsoft.AspNetCore.Authorization;
using AdoptMeNow.Data; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AdoptMeNow.ViewModels;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;

namespace AdoptMeNow.Controllers;

public class HomeController : Controller
{   
    private readonly AppDbContext _context;
    private readonly UserManager<Users> _userManager;
    public HomeController(AppDbContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
 
    public async Task<IActionResult> Index()
    {   
        if(User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Admin"))
        {
            return RedirectToAction("Dashboard","Admin");
        }
        var pets = await _context.Pets
            .Take(4) //only first 6
            .ToListAsync();

        var totalPets = await _context.Pets.CountAsync();
        var availablePets = await _context.Pets.CountAsync(p => p.Status == "Available");
        var adoptedPets = await _context.Pets.CountAsync(p => p.Status == "Adopted");

        var userId = _userManager.GetUserId(HttpContext.User);

        var userAdoptions = userId == null
            ? new List<Adoption>()
            : await _context.Adoptions
                .Where(a => a.UserId == userId)
                .ToListAsync();

        var savedPetIds = userId == null
        ? new List<int>()
        : await _context.SavedPets
            .Where(s => s.UserId == userId)
            .Select(s => s.PetId)
            .ToListAsync();
    
        var petViewModels = pets.Select(p => new PetViewModel 
        {
            Pet = p,

            IsSaved = savedPetIds.Contains(p.PetId),
            ApplicationStatus = userAdoptions
                .Where(a => a.PetId == p.PetId)
                .OrderByDescending(a => a.ApplicationDate)
                .Select(a => a.Status)
                .FirstOrDefault(),

            HasPendingApplication = userAdoptions
                .Any(a => a.PetId == p.PetId && a.Status == "Pending")  

        }).ToList();

        var viewModel = new HomeViewModel
        {
            Pets = petViewModels,
            TotalPets = totalPets,
            AvailablePets = availablePets,
            AdoptedPets = adoptedPets
        };
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> SearchPets(string search, string status = "all", string category = "all", int page = 1, int pageSize = 4)
    {   
        var query = _context.Pets.AsQueryable();

        //search
        if(!string.IsNullOrEmpty(search))
        {
            var keyword = search.ToLower();

            query = query.Where(p =>
                (p.Name != null && p.Name.ToLower().Contains(keyword)) ||
                (p.Breed != null && p.Breed.ToLower().Contains(keyword)) ||
                (p.Category != null && p.Category.ToLower().Contains(keyword))

            );
        }

        //status filter
        if(!string.IsNullOrEmpty(status) && status != "all")
        {
           query = query.Where(p =>
            p.Status != null &&
            p.Status.ToLower() == status.ToLower()
        );
        }

        //category filter
        if(!string.IsNullOrEmpty(category) && category != "all")
        {
            query = query.Where(p => 
            p.Category !=null &&
            p.Category.ToLower() == category.ToLower());
        }

        var totalCount = await query.CountAsync();

        //Load More
        var pets = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        //ViewBag.HasMore = (page * pageSize) < totalCount;

        //get all pets frm db first:This loads EVERYTHING first (slow)
        //var pets = await _context.Pets.ToListAsync();

        /*get all pets frm db and filter:faster
        var keyword = search?.ToLower();
         var pets = await _context.Pets
            .Where(p =>
                string.IsNullOrEmpty(keyword)||
                p.Name.ToLower().Contains(keyword) ||
                p.Breed.ToLower().Contains(keyword) ||
                p.Category.ToLower().Contains(keyword)
            )
            .ToListAsync();*/

        //get current user ID
        var userId = _userManager.GetUserId(User); 

        var userAdoptions = userId == null
        ? new List<Adoption>()
        : await _context.Adoptions
            .Where(a => a.UserId == userId)
            .ToListAsync();

       
        var savedPetIds = userId == null
            ? new List<int>()
            : await _context.SavedPets
                .Where(s => s.UserId == userId)
                .Select(s => s.PetId)
                .ToListAsync();

        var filtered = pets
       /* no need since aready filter in db:
       .Where(p =>
            string.IsNullOrEmpty(search) || //if empty show all pets
            p.Name.ToLower().Contains(search.ToLower()) || //Does pet name contain the search word?
            p.Breed.ToLower().Contains(search.ToLower()) //Tolower makes search case-insensitive
        )*/
        //How each pet should be displayed:status:pending/available/rejected
        .Select(p => new PetViewModel
        {
            Pet = p,

            IsSaved = savedPetIds.Contains(p.PetId),
            HasPendingApplication = userAdoptions.Any(a => a.PetId == p.PetId && a.Status == "Pending"),
            //Get the latest status for this pet
            ApplicationStatus = userAdoptions
                .Where(a => a.PetId == p.PetId)
                .OrderByDescending(a => a.ApplicationDate)
                .Select(a => a.Status)
                .FirstOrDefault()
        })
        .ToList();

        return PartialView("_PetCards", filtered);

    }


    public IActionResult Details(int id)
    {
        var pet = _context.Pets.FirstOrDefault(p => p.PetId == id);

        if(pet==null)
        {
            return NotFound();
        }
        var userId = _userManager.GetUserId(HttpContext.User);

        var model = new PetViewModel
        {
            Pet = pet,
           ApplicationStatus = userId != null
            ? _context.Adoptions
                .Where(a => a.PetId == id && a.UserId == userId)
                .OrderByDescending(a => a.ApplicationDate)
                .Select(a => a.Status)
                .FirstOrDefault()
            : null,
        };
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /*[HttpGet]
    public IActionResult LoadMorePets(int page = 1, int pageSize = 6)
    {
        var pets = _context.Pets
            ,Where(p => p.Sta)
    }*/

}
