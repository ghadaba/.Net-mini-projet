using Microsoft.EntityFrameworkCore;
using tp2_oolab.Models;

namespace tp2_oolab.data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<tp2_oolab.Models.Customer> Customer { get; set; } = default!;
    }
}
