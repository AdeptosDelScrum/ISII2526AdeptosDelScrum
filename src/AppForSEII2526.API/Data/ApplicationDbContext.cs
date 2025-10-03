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


}
