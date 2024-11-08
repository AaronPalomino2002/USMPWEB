using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using USMPWEB.Models;

namespace USMPWEB.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<USMPWEB.Models.RegisterViewModel> DataRegistro { get; set; }
    public DbSet<USMPWEB.Models.Login> DataHome { get; set; }
    public DbSet<USMPWEB.Models.Alumno> DataAlumnos { get; set; }
    public DbSet<USMPWEB.Models.Inscripciones> DataInscripciones { get; set; }
    public DbSet<USMPWEB.Models.Talleres> DataTalleres { get; set; }
    public DbSet<USMPWEB.Models.Certificados> DataCertificados { get; set; }
    public DbSet<USMPWEB.Models.Contacto> DataContacto { get; set; }
    public DbSet<USMPWEB.Models.Carrera> DataCarrera { get; set; }
    public DbSet<USMPWEB.Models.Facultad> DataFacultad { get; set; }
    public DbSet<USMPWEB.Models.Campanas> DataCampanas { get; set; }
    public DbSet<USMPWEB.Models.Categoria> DataCategoria { get; set; }
    public DbSet<USMPWEB.Models.SubCategoria> DataSubCategoria { get; set; }
    public DbSet<USMPWEB.Models.EventosInscripciones> DataEventosInscripciones { get; set; }
    public DbSet<CampanaInscripcion> CampanaInscripciones { get; set; } = null!;
    public DbSet<Pago> Pagos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Campanas>()
            .HasMany(c => c.SubCategorias)
            .WithMany(s => s.Campanas)
            .UsingEntity(
                "CampanaSubCategoria",
                l => l.HasOne(typeof(SubCategoria)).WithMany().HasForeignKey("SubCategoriasIdSubCategoria"),
                r => r.HasOne(typeof(Campanas)).WithMany().HasForeignKey("CampanasId"),
                j =>
                {
                    j.HasKey("CampanasId", "SubCategoriasIdSubCategoria");
                    j.ToTable("CampanaSubCategoria");
                });
        modelBuilder.Entity<CampanaInscripcion>()
            .Property(c => c.NumeroRecibo)
            .HasMaxLength(50);

        modelBuilder.Entity<Pago>()
            .Property(p => p.NumeroRecibo)
            .HasMaxLength(50);        
    }
}
