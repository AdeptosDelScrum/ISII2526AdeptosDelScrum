using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) {

    public DbSet<Compra> Compra { get; set; }
    public DbSet<Bocadillo> Bocadillo { get; set; }
    public DbSet<TipoPan> TipoPan { get; set; }
    public DbSet<CompraBocadillo> CompraBocadillo { get; set; }
    public DbSet<MetodoPago> MetodoPago { get; set; }
    public DbSet<Paypal> Paypal { get; set; }
    public DbSet<GPay> GPay { get; set; }
    public DbSet<Tarjeta> Tarjeta { get; set; }
    public DbSet<Tamanyo> Tamanyos { get; set; }
    public DbSet<Pequenyo> Pequenyos { get; set; }
    public DbSet<Normal> Normales { get; set; }
    public DbSet<Resenya> resenyas { get; set; }
    public DbSet<ResenyaBocadillo> resenyaBocadillos { get; set; }
    public DbSet<TipoBocadillo> TipoBocadillos { get; set; }
    public DbSet<BonoBocadillo> BonoBocadillos { get; set; }
    public DbSet<BonosComprados> BonosComprados { get; set; }
    public DbSet<CompraBono> ComprasBono { get; set; }

    protected override void OnModelCreating(ModelBuilder
builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tamanyo>()
                   .HasDiscriminator<string>("Tamanyos")
                   .HasValue<Pequenyo>("Pequenyo")
                   .HasValue<Normal>("Normal");
    }


}
