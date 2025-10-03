using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) {
    public DbSet<Resenya> resenyas { get; set; }
    public DbSet<ResenyaBocadillo> resenyaBocadillos { get; set; }
    public DbSet<Bocadillo> bocadillos { get; set; }

    public DbSet<TipoPan> tiposPan { get; set; }
}
