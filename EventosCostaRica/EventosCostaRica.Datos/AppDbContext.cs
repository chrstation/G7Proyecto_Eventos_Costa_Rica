using EventosCostaRica.Entidades;
using Microsoft.EntityFrameworkCore;

namespace EventosCostaRica.Datos;

/// <summary>
/// Contexto de Entity Framework Core para la base de datos SQLite de Eventos Costa Rica.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Evento> Eventos => Set<Evento>();

    public DbSet<Entrada> Entradas => Set<Entrada>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entidad =>
        {
            entidad.HasKey(u => u.Id);
            entidad.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
            entidad.Property(u => u.Correo).IsRequired().HasMaxLength(150);
            entidad.HasIndex(u => u.Correo).IsUnique();
            entidad.Property(u => u.ContrasenaHash).IsRequired().HasMaxLength(300);
        });

        modelBuilder.Entity<Evento>(entidad =>
        {
            entidad.HasKey(e => e.Id);
            entidad.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entidad.Property(e => e.Descripcion).HasMaxLength(1000);
            entidad.Property(e => e.Lugar).IsRequired().HasMaxLength(150);
            entidad.Property(e => e.BannerUrl).HasMaxLength(500);
            entidad.Ignore(e => e.EntradasVendidas);
            entidad.Ignore(e => e.AsientosDisponibles);
            entidad.Ignore(e => e.Agotado);
            entidad.Ignore(e => e.Finalizado);
        });

        modelBuilder.Entity<Entrada>(entidad =>
        {
            entidad.HasKey(e => e.Id);
            entidad.Ignore(e => e.Etiqueta);

            // Un asiento de un evento solo puede venderse una vez (evita compras duplicadas).
            entidad.HasIndex(e => new { e.EventoId, e.Fila, e.Numero }).IsUnique();

            entidad.HasOne(e => e.Evento)
                   .WithMany(ev => ev.Entradas)
                   .HasForeignKey(e => e.EventoId)
                   .OnDelete(DeleteBehavior.Restrict);

            entidad.HasOne(e => e.Usuario)
                   .WithMany(u => u.Entradas)
                   .HasForeignKey(e => e.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
