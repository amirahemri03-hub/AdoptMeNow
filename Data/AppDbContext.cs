using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AdoptMeNow.Models; 

namespace AdoptMeNow.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }= null!;

        public DbSet<Adoption> Adoptions { get; set; }

        public DbSet<RescueReport> RescueReports { get; set; }
        public DbSet<RescueProgress> RescueProgresses { get; set; }
        public DbSet<SavedPets> SavedPets {get; set;}


    }
}
