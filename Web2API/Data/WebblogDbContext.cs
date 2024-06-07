using Microsoft.EntityFrameworkCore;
using Web2API.Models;

namespace Web2API.Data
{
    public class WebblogDbContext: DbContext
    {
        public WebblogDbContext(DbContextOptions<WebblogDbContext> options) : base(options)
        {
        }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<ContactModel> contacts { get; set; }
       
    }
}
