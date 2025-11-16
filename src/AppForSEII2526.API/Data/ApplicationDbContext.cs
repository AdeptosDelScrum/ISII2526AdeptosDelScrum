using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.Models; // ✅ apunta al namespace correcto de tus modelos

namespace AppForSEII2526.API.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Compra> Compra { get; set; }
        public DbSet<Bocadillo> Bocadillo { get; set; }
        public DbSet<TipoPan> TipoPan { get; set; }
        public DbSet<CompraBocadillo> CompraBocadillo { get; set; }
        public DbSet<MetodoPago> MetodoPago { get; set; }
        public DbSet<Paypal> Paypal { get; set; }
        public DbSet<GPay> GPay { get; set; }
        public DbSet<Tarjeta> Tarjeta { get; set; }
        public DbSet<Resenya> Resenyas { get; set; }
        public DbSet<ResenyaBocadillo> ResenyaBocadillos { get; set; }
        public DbSet<TipoBocadillo> TipoBocadillos { get; set; }
        public DbSet<BonoBocadillo> BonoBocadillos { get; set; }
        public DbSet<BonosComprados> BonosComprados { get; set; }
        public DbSet<CompraBono> ComprasBono { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // --------- Relaciones CU Bonos ---------
            builder.Entity<BonoBocadillo>()
                .HasOne(b => b.TipoBocadillo)
                .WithMany(t => t.Bonos)
                .HasForeignKey(b => b.IdTipo)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BonosComprados>(b =>
            {
                b.HasKey(x => new { x.CompraId, x.BonoId });
                b.Property(x => x.PrecioBono).HasColumnType("decimal(10,2)");

                b.HasOne(x => x.Bono)
                 .WithMany(bono => bono.BonosComprados)
                 .HasForeignKey(x => x.BonoId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Compra)
                 .WithMany(compra => compra.BonosComprados)
                 .HasForeignKey(x => x.CompraId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<CompraBono>()
                .HasOne(c => c.MetodoPago)
                .WithMany() // o .WithMany(mp => mp.Compras) si tu entidad lo soporta
                .HasForeignKey(c => c.MetodoPagoId)
                .OnDelete(DeleteBehavior.Restrict);

            // --------- Herencia Tamanyo ---------
            base.OnModelCreating(builder);

            
        }
    }
}

