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
    public DbSet<CertificadoInscripcion> CertificadoInscripciones { get; set; }
    public DbSet<EventoInscripcion> EventoInscripciones { get; set; }
    public DbSet<Pago> Pagos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Campanas>(entity =>
        {
            // Configura la tabla y la clave primaria
            entity.ToTable("t_campanas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();

            // Configura los nuevos campos
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.Requisitos)
                .IsRequired();

            // Mantén la configuración existente de la relación muchos a muchos
            entity.HasMany(c => c.SubCategorias)
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
        });
        modelBuilder.Entity<EventosInscripciones>(entity =>
        {
            // Configura la tabla y la clave primaria
            entity.ToTable("t_eventosInscripciones");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityColumn();


            entity.Property(e => e.Requisitos)
                .IsRequired();

            // Mantén la configuración existente de la relación muchos a muchos
            entity.HasMany(c => c.SubCategorias)
                .WithMany(s => s.EventosInscripciones)
                .UsingEntity(
                    "EventoSubCategoria",
                    l => l.HasOne(typeof(SubCategoria)).WithMany().HasForeignKey("SubCategoriasIdSubCategoria"),
                    r => r.HasOne(typeof(EventosInscripciones)).WithMany().HasForeignKey("EventosInscripcionesId"),
                    j =>
                    {
                        j.HasKey("EventosInscripcionesId", "SubCategoriasIdSubCategoria");
                        j.ToTable("EventoSubCategoria");
                    });
        });
        modelBuilder.Entity<Certificados>(entity =>
            {
                entity.ToTable("t_certificados");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn(); // Asegúrate de que esté presente

                 entity.Property(e => e.Requisitos)
                .IsRequired();

            // Mantén la configuración existente de la relación muchos a muchos
            entity.HasMany(c => c.SubCategorias)
                .WithMany(s => s.Certificados)
                .UsingEntity(
                    "CertificadoSubCategoria",
                    l => l.HasOne(typeof(SubCategoria)).WithMany().HasForeignKey("SubCategoriasIdSubCategoria"),
                    r => r.HasOne(typeof(Certificados)).WithMany().HasForeignKey("CertificadosId"),
                    j =>
                    {
                        j.HasKey("CertificadosId", "SubCategoriasIdSubCategoria");
                        j.ToTable("CertificadoSubCategoria");
                    });


            });
        // Resto de tus configuraciones existentes
        modelBuilder.Entity<CampanaInscripcion>(entity =>
            {
                entity.ToTable("t_campana_inscripciones");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .UseIdentityAlwaysColumn()
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.NumeroRecibo)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasOne(d => d.Campana)
                    .WithMany()
                    .HasForeignKey(d => d.CampanaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.Property(e => e.FechaInscripcion)
                    .HasColumnType("timestamp with time zone");
            });

        modelBuilder.Entity<Pago>()
            .Property(p => p.NumeroRecibo)
            .HasMaxLength(50);

        modelBuilder.Entity<CertificadoInscripcion>()
            .HasOne(ci => ci.Certificado)
            .WithMany()
            .HasForeignKey(ci => ci.CertificadoId);
    }
}
