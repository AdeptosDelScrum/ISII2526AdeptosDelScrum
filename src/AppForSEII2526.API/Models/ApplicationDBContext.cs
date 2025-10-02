using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Models
{
    public class ApplicationDBContext: DbContext
    {
        public DbSet<Resenya> Resenyas { get; set; }
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
                : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);


        }
    }
}
